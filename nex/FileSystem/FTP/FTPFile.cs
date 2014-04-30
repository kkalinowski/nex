using System;
using System.IO;
using System.Net;
using System.Windows.Media;
using lib12.DependencyInjection;
using nex.Accounts;
using nex.DirectoryView;
using nex.Utilities;
using edtFTPFile = EnterpriseDT.Net.Ftp.FTPFile;

namespace nex.FileSystem.FTP
{
    [Serializable]
    public class FTPFile : IDirectoryViewItem
    {
        private string name;
        private string fullName;//QSTN: Can this be source of troubles when directory changes locations?
        [NonSerialized]
        private static ImageSource directoryIcon;
        [NonSerialized]
        private static ImageSource fileIcon;

        public int AccountId { get; private set; }

        #region sctor
        static FTPFile()
        {
            directoryIcon = Utility.LoadImageSource("DirectoryIcon");
            fileIcon = Utility.LoadImageSource("FileIcon");
        } 
        #endregion

        #region Start
        private FTPFile(int accountId)
        {
            AccountId = accountId;
        }

        public static FTPFile CreateFromServerCall(edtFTPFile fileInfo, string baseDir, int accountId)
        {
            var file = new FTPFile(accountId);
            file.LastModifiedTime = fileInfo.LastModified;
            file.IsDirectory = fileInfo.Dir;
            file.Size = FileSize.CreateFromBytes(fileInfo.Size);
            file.name = fileInfo.Name;
            file.fullName = PathExt.Combine(baseDir, fileInfo.Name, false);

            return file;
        }

        public static FTPFile CreateFromPath(string path, int accountId)
        {
            FTPFile file = new FTPFile(accountId);
            file.fullName = path;
            file.name = PathExt.GetName(path);
            return file;
        }
        #endregion

        public FileAttributes Attributes
        {
            get
            {
                return FileAttributes.Normal;
            }
        }

        public ImageSource FileIcon
        {
            get
            {
                return IsDirectory ? directoryIcon : fileIcon;
            }
        }

        public bool Exists
        {
            get
            {
                return true;
            }
        }

        public DateTime LastModifiedTime
        {
            //TODO: This is really LastModifiedTime or smth, add it to IDirectoryView
            get;
            set;
        }

        public FileSize Size { get; private set; }

        public string Name
        {
            get
            {
                return name;
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
                return fullName;
            }
        }

        public bool IsDirectory { get; private set; }

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
                return false;
            }
        }

        public bool? IsMatchingCriteria { get; set; }

        public void CopyTo(string destDir, nex.Operations.CopyOperation operation)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(string destDir, nex.Operations.MoveOperation operation)
        {
            throw new NotImplementedException();
        }

        public void CountFolderSize()
        {
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            return fullName;
        }

        public FileSystemBase CreateFileSystem()
        {
            Account acc = Instances.Get<AccountManager>().GetAccountById(AccountId);
            return new FTPFileSystem(acc, FullName);
        }
    }
}