using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Media;
using lib12.WPF.Core;
using nex.DirectoryView;
using nex.Operations;

namespace nex.FileSystem.Windows
{
    [Serializable]
    public class WindowsFile : NotifyingObject, IDirectoryViewItem, ISerializable
    {
        #region Fields
        private readonly FileSystemInfo adapted;
        private readonly bool isDir;
        #endregion

        #region Props
        public bool Exists
        {
            get
            {
                return adapted.Exists;
            }
        }

        public string Name
        {
            get
            {
                return adapted.Name;
            }
        }

        public string NameWithoutExt
        {
            get
            {
                return IsDirectory ? Name : Path.GetFileNameWithoutExtension(Name);
            }
        }

        public string Ext
        {
            get
            {
                return IsDirectory ? string.Empty : PathExt.GetExtensionWithoutDot(Name);
            }
        }

        public string FullName
        {
            get
            {
                return adapted.FullName;
            }
        }

        public bool IsMoveUp
        {
            get
            {
                return false;
            }
        }

        public bool IsWindowsFile
        {
            get
            {
                return true;
            }
        }

        public FileAttributes Attributes
        {
            get
            {
                return adapted.Attributes;
            }
        }

        public DateTime LastModifiedTime
        {
            get
            {
                return adapted.LastWriteTime;
            }
            set
            {
                adapted.LastWriteTime = value;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return isDir;
            }
        }

        private ImageSource fileIcon;
        public ImageSource FileIcon
        {
            get
            {
                return fileIcon ?? (fileIcon = WindowsFileSystemApi.GetFileImage(FullName));
            }
        }

        public FileSize Size { get; private set; }

        private bool? isMatchingCriteria;
        public bool? IsMatchingCriteria
        {
            get { return isMatchingCriteria; }
            set
            {
                isMatchingCriteria = value;
                OnPropertyChanged("IsMatchingCriteria");
            }
        }
        #endregion

        public WindowsFile(FileSystemInfo toAdapt)
        {
            adapted = toAdapt;
            isDir = toAdapt is DirectoryInfo;

            if (toAdapt == null)
                Size = FileSize.Empty;
            else if (isDir)
                //TODO: Make computing directory size
                //Size = FileSize.CreateForDirectory(FullName);
                Size = FileSize.Empty;
            else
                Size = FileSize.CreateFromBytes(((FileInfo)toAdapt).Length);
        }

        /// <summary>
        /// Creates new WindowsFile from path, discover whether object is file or directory
        /// </summary>
        /// <param name="path">Path to file or directory</param>
        /// <returns>New WindowsFile object</returns>
        public static WindowsFile CreateFromPath(string path)
        {
            if (File.Exists(path))
                return new WindowsFile(new FileInfo(path));
            else
                return new WindowsFile(new DirectoryInfo(path));
        }

        #region Simple .Net Functions

        /// <summary>
        /// Simple .net copy
        /// </summary>
        /// <param name="destDir">Destination directory</param>
        public void SimpleCopyTo(string destDir)
        {
            if (isDir)
                SimpleCopyDirectory((DirectoryInfo)adapted, destDir);
            else
                ((FileInfo)adapted).CopyTo(Path.Combine(destDir, Name));
        }

        /// <summary>
        /// My own simple function to copy directories, because framework dont have any
        /// </summary>
        /// <param name="dir">Directory to copy</param>
        /// <param name="destination">Destination directory</param>
        private void SimpleCopyDirectory(DirectoryInfo dir, string destination)
        {
            string copyDir = Path.Combine(destination, dir.Name);
            Directory.CreateDirectory(copyDir);

            foreach (FileInfo file in dir.GetFiles())
                file.CopyTo(Path.Combine(copyDir, file.Name));

            foreach (DirectoryInfo di in dir.GetDirectories())
                SimpleCopyDirectory(di, copyDir);
        }

        #endregion

        public void CopyTo(string destDir, CopyOperation operation)
        {
            if (!isDir)
                WindowsFileSystemApi.CopyFile((FileInfo)adapted, new FileInfo(Path.Combine(destDir, Name)), CopyFileOptions.None, operation.CopiedPieceOfFile);
            else
                CopyDirectory((DirectoryInfo)adapted, destDir, operation);
        }

        /// <summary>
        /// Copy directory
        /// </summary>
        /// <param name="dir">Directory to copy</param>
        /// <param name="destination">Destination directory</param>
        /// <param name="operation">CopyOperation to report changes</param>
        private void CopyDirectory(DirectoryInfo dir, string destination, CopyOperation operation)
        {
            string copyDir = Path.Combine(destination, dir.Name);
            Directory.CreateDirectory(copyDir);

            foreach (FileInfo file in dir.GetFiles())
                WindowsFileSystemApi.CopyFile(file, new FileInfo(Path.Combine(copyDir, file.Name)), CopyFileOptions.None, operation.CopiedPieceOfFile);

            foreach (DirectoryInfo di in dir.GetDirectories())
                CopyDirectory(di, copyDir, operation);
        }

        public void MoveTo(string destDir, MoveOperation operation)
        {
            if (!isDir)
                WindowsFileSystemApi.MoveFile((FileInfo)adapted, new FileInfo(Path.Combine(destDir, Name)), MoveFileOptions.CopyAllowed | MoveFileOptions.ReplaceExisting, operation.MovedPieceOfFile);
            else
                MoveDirectory((DirectoryInfo)adapted, destDir, operation);
        }

        /// <summary>
        /// Moved directory with content to another place
        /// </summary>
        /// <param name="dir">Directory to move</param>
        /// <param name="destination">Destination path</param>
        /// <param name="operation">MoveOperation to report progress</param>
        private void MoveDirectory(DirectoryInfo dir, string destination, MoveOperation operation)
        {
            //TODO: Check if is WinAPI function for moving directories
            string moveDir = Path.Combine(destination, dir.Name);
            Directory.CreateDirectory(moveDir);

            foreach (FileInfo file in dir.GetFiles())
                WindowsFileSystemApi.MoveFile(file, new FileInfo(Path.Combine(moveDir, file.Name)), MoveFileOptions.CopyAllowed, operation.MovedPieceOfFile);

            foreach (DirectoryInfo di in dir.GetDirectories())
                MoveDirectory(di, moveDir, operation);

            dir.Delete(true);
        }

        public void Encrypt()
        {
            if (!IsDirectory)
            {
                ((FileInfo)adapted).Encrypt();
            }
        }

        public void Decrypt()
        {
            if (!IsDirectory)
            {
                ((FileInfo)adapted).Decrypt();
            }
        }

        /// <summary>
        /// Computes folder size
        /// </summary>
        public void CountFolderSize()
        {
            if (IsDirectory)
                Size = FileSize.CreateForDirectory(FullName);
        }

        #region Cast
        public static explicit operator FileInfo(WindowsFile file)
        {
            Debug.Assert(!file.IsDirectory, "Only files!");
            return (FileInfo)file.adapted;
        }

        public static explicit operator FileSystemInfo(WindowsFile file)
        {
            return file.adapted;
        } 
        #endregion

        public override bool Equals(object obj)
        {
            return obj is WindowsFile && ((WindowsFile)obj).FullName == FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #region ISerializable Members

        /// <summary>
        /// Supports serialization
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FullName", FullName);
            info.AddValue("IsDir", isDir);
        }

        /// <summary>
        /// Deserialization ctor
        /// </summary>
        public WindowsFile(SerializationInfo info, StreamingContext context)
        {
            isDir = info.GetBoolean("IsDir");
            string path = (string)info.GetValue("FullName", typeof(string));
            if (isDir)
                adapted = new DirectoryInfo(path);
            else
                adapted = new FileInfo(path);

            if (isDir)
                //TODO: Improve it
                //Size = FileSize.CreateForDirectory(FullName);
                Size = FileSize.Empty;
            else if (adapted.Exists)
                Size = FileSize.CreateFromBytes(((FileInfo)adapted).Length);
            else
                Size = FileSize.CreateFromBytes(0);
        }

        #endregion

        public FileSystemBase CreateFileSystem()
        {
            return new WindowsFileSystem(FullName);
        }
    }
}