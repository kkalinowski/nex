using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using nex.DirectoryView;

namespace nex.FileSystem.Windows
{
    [Serializable]
    public sealed class WindowsFileSystem : FileSystemBase
    {
        #region Fields
        private FileSystemRoot root;
        #endregion

        #region Props
        public override FileSystemRoot Root
        {
            get
            {
                return root;
            }
            protected set
            {
                root = value;
                OnPropertyChanged("Root");
            }
        }

        /// <summary>
        /// Get the drive letter of current place
        /// </summary>
        public string DriveLetter
        {
            get
            {
                return PathExt.GetDriveLetter(FullPath).Substring(0, 1);
            }
        }

        public override bool IsWindowsFileSystem
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region ctor
        public WindowsFileSystem()
        {

        }

        public WindowsFileSystem(string directory)
        {
            Root = new WindowsDrive(PathExt.GetDriveLetter(directory));
            LoadDirectory(directory);
        }
        #endregion

        public override IEnumerable<IDirectoryViewItem> GetDirectoryContent(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] infos = di.GetFileSystemInfos();

            int lengthWithoutHidden = infos.Count(inf => (inf.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);
            int addUp = Convert.ToInt32(!PathExt.IsDriveRoot(path));

            IDirectoryViewItem[] adapters = new IDirectoryViewItem[lengthWithoutHidden + addUp];

            if (addUp == 1)
                adapters[0] = new MoveUpObject();

            int i = addUp;
            foreach (FileSystemInfo info in infos)
            {
                if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    adapters[i] = new WindowsFile(info);
                    i++;
                }
            }

            CurrentPlace = new WindowsFile(di);
            return adapters.Cast<IDirectoryViewItem>();
        }

        public override void LoadDirectory(string path)
        {
            Items.Clear();
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] infos = di.GetFileSystemInfos();
            int addUp = Convert.ToInt32(!PathExt.IsDriveRoot(path));
            if (addUp == 1)
                Items.Add(new MoveUpObject());
            foreach (FileSystemInfo info in infos)
                if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)//OPTIONS: Show hidden files
                    Items.Add(new WindowsFile(info));

            CurrentPlace = new WindowsFile(di);

            //check if drive has changed
            if (Root == null || Path.GetPathRoot(FullPath) != Root.Path)
                Root = new WindowsDrive(DriveLetter);
        }

        public override bool CheckIfObjectExist(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public override FileSystemBase GetCopy()
        {
            return new WindowsFileSystem(this.FullPath);
        }

        [OnDeserialized]
        public void InitializeAfterDeserialization(StreamingContext context)
        {
            Root = new WindowsDrive(PathExt.GetDriveLetter(CurrentPlace.FullName));
            LoadDirectory(CurrentPlace.FullName);
        }

        #region Operations
        public override IDirectoryViewItem CreateNewDirectory(string path)
        {
            return new WindowsFile(Directory.CreateDirectory(path));
        }

        public override void Delete(IDirectoryViewItem toDelete)
        {
            if (toDelete.IsDirectory)
                Directory.Delete(toDelete.FullName, true);
            else
                File.Delete(toDelete.FullName);
        }

        public override void Rename(string from, string to, bool isDirectory)
        {
            if (isDirectory)
                Directory.Move(from, to);
            else
                File.Move(from, to);
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
        }
        #endregion
    }
}