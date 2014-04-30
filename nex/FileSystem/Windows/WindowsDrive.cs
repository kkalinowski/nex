using System;
using System.IO;
using System.Windows.Media;

namespace nex.FileSystem.Windows
{
    /// <summary>
    /// Adapt DriveInfo - describes disk partition
    /// </summary>
    [Serializable]
    public class WindowsDrive : FileSystemRoot
    {
        private readonly DriveInfo adapted;

        #region Props
        public override string Path
        {
            get
            {
                return adapted.RootDirectory.FullName;
            }
        }

        public override string Label
        {
            get
            {
                return adapted.VolumeLabel;
            }
        }

        public override double FreePercent
        {
            get
            {
                return 100.0 * (double)adapted.AvailableFreeSpace / (double)adapted.TotalSize;
            }
        }

        public override ImageSource Icon
        {
            get
            {
                return WindowsFileSystemApi.GetFileImage(Path);
            }
        }
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="_adapted">DriveInfo to adapt</param>
        public WindowsDrive(DriveInfo _adapted)
        {
            adapted = _adapted;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="driveLetter">Letter of drive to adapt</param>
        public WindowsDrive(string driveLetter)
        {
            adapted = new DriveInfo(driveLetter);
        }
    }
}