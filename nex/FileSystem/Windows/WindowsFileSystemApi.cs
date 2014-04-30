using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace nex.FileSystem.Windows
{
    public class WindowsFileSystemApi
    {
        #region Const
        private const uint ICON = 0x100;
        private const uint SMALLICON = 0x0;
        private const uint LARGEICON = 0x0;
        #endregion

        #region PInvoke
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData, ref bool pbCancel, int dwCopyFlags);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool MoveFileWithProgress(string lpExistingFileName, string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine, IntPtr lpData, MoveFileOptions dwCopyFlags);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        #endregion

        #region AuxillaryClasses
        private class CopyProgressData
        {
            private readonly FileInfo _source = null;
            private readonly FileInfo _destination = null;
            private readonly CopyFileCallback _callback = null;
            private readonly object _state = null;

            public CopyProgressData(FileInfo source, FileInfo destination, CopyFileCallback callback, object state)
            {
                _source = source;
                _destination = destination;
                _callback = callback;
                _state = state;
            }

            public int CallbackHandler(long totalFileSize, long totalBytesTransferred, long streamSize,
                long streamBytesTransferred, int streamNumber, int callbackReason,
                IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
            {
                return (int)_callback(_source, _destination, _state, totalFileSize, totalBytesTransferred);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        #endregion

        #region Delegate
        private delegate int CopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize,
            long streamBytesTransferred, int streamNumber, int callbackReason,
            IntPtr sourceFile, IntPtr destinationFile, IntPtr data);
        #endregion

        public static void CopyFile(FileInfo source, FileInfo destination,
            CopyFileOptions options, CopyFileCallback callback)
        {
            CopyFile(source, destination, options, callback, null);
        }

        public static void CopyFile(FileInfo source, FileInfo destination,
            CopyFileOptions options, CopyFileCallback callback, object state)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if ((options & ~CopyFileOptions.All) != 0)
                throw new ArgumentOutOfRangeException("options");

            new FileIOPermission(FileIOPermissionAccess.Read, source.FullName).Demand();
            new FileIOPermission(FileIOPermissionAccess.Write, destination.FullName).Demand();

            CopyProgressRoutine cpr = (callback == null ? null : new CopyProgressRoutine(new CopyProgressData(source, destination, callback, state).CallbackHandler));

            bool cancel = false;
            if (!CopyFileEx(source.FullName, destination.FullName, cpr, IntPtr.Zero, ref cancel, (int)options))
            {
                //throw new IOException(new Win32Exception().Message);
            }
        }

        public static void MoveFile(FileInfo source, FileInfo destination,
            MoveFileOptions options, CopyFileCallback callback)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");

            new FileIOPermission(FileIOPermissionAccess.Read, source.FullName).Demand();
            new FileIOPermission(FileIOPermissionAccess.Write, destination.FullName).Demand();

            CopyProgressRoutine cpr = (callback == null ? null : new CopyProgressRoutine(new CopyProgressData(source, destination, callback, null).CallbackHandler));

            if (!MoveFileWithProgress(source.FullName, destination.FullName, cpr, IntPtr.Zero, options))
            {
                throw new IOException(new Win32Exception().Message);
            }
        }

        private static Icon GetFileIcon(string filename)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(filename, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), ICON | SMALLICON);

            return shinfo.hIcon != IntPtr.Zero ? Icon.FromHandle(shinfo.hIcon) : null;
        }

        public static BitmapSource GetFileImage(string filename)
        {
            var fileIcon = GetFileIcon(filename);
            if (fileIcon == null)
                return null;

            IntPtr hBitmap = fileIcon.ToBitmap().GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);//to avoid memory leaks
            }
        }
    }
}