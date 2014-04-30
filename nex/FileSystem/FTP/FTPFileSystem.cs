using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using EnterpriseDT.Net.Ftp;
using nex.Accounts;
using nex.DirectoryView;
using nex.FileSystem.Windows;

namespace nex.FileSystem.FTP
{
    /// <summary>
    /// Class represents virtual FTP FileSystem
    /// </summary>
    [Serializable]
    public class FTPFileSystem : FileSystemBase
    {
        #region Const
        private const int Timeout = 10000;
        #endregion

        #region Fields
        private FTPConnection connection;
        private Account account;
        private bool isDisposed = false;
        #endregion

        #region Props
        public override FileSystemRoot Root { get; protected set; }

        public override bool IsWindowsFileSystem
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region ctor
        public FTPFileSystem(Account account)
        {
            Root = new FTPRoot(account);
            this.account = account;

            Connect();
            LoadDefaultDirectory();
        }

        public FTPFileSystem(Account account, string path)
        {
            Root = new FTPRoot(account);
            this.account = account;

            Connect();
            LoadDirectory(path);
        }
        #endregion

        #region Connect
        private void Connect()
        {
            if (connection != null && connection.IsConnected)
                return;

            connection = new FTPConnection();
            connection.Timeout = Timeout;
            connection.ServerAddress = account.Url;
            connection.UserName = account.UserName;
            connection.Password = account.Password;

            connection.Connect();
        }
        #endregion

        #region Logic
        public override IEnumerable<IDirectoryViewItem> GetDirectoryContent(string dirPath)
        {
            var content = new List<IDirectoryViewItem>();
            if (!IsRootPath(dirPath))
                content.Add(new MoveUpObject());

            var infos = connection.GetFileInfos(dirPath);
            var ftpFiles = infos.Where(x => x.Name != "." && x.Name != "..").Select(x => FTPFile.CreateFromServerCall(x, dirPath, account.Id)).OrderByDescending(x => x.IsDirectory);
            content.AddRange(ftpFiles);

            connection.ChangeWorkingDirectory(dirPath);
            return content;
        }

        public override void LoadDirectory(string dirPath)
        {
            Items.Clear();
            Items.AddRange(GetDirectoryContent(dirPath));
            CurrentPlace = FTPFile.CreateFromPath(dirPath, account.Id);
        }

        public bool CheckIfFileExist(string path)
        {
            return connection.Exists(path);
        }

        public bool CheckIfDirectoryExist(string path)
        {
            return connection.DirectoryExists(path);
        }

        public override bool CheckIfObjectExist(string path)
        {
            return CheckIfFileExist(path) || CheckIfDirectoryExist(path);
        }

        public void Upload(WindowsFile file)
        {
            connection.UploadFile(file.FullName, file.Name);
        }

        public void Download(FTPFile file, string destination)
        {
            connection.DownloadFile(destination, file.Name);
        }

        public override FileSystemBase GetCopy()
        {
            return new FTPFileSystem(account, FullPath);
        }
        #endregion

        #region Operations
        public override IDirectoryViewItem CreateNewDirectory(string path)
        {
            connection.CreateDirectory(path);
            return null;//TODO: fix
        }

        public override void Delete(IDirectoryViewItem toDelete)
        {
            if (toDelete.IsDirectory)
                connection.DeleteDirectory(toDelete.FullName);
            else
                connection.DeleteFile(toDelete.FullName);
        }

        public override void Rename(string from, string to, bool isDirectory)
        {
            if (isDirectory)
                return;
            else
                connection.RenameFile(from, to);
        }
        #endregion

        #region Serialization
        [OnDeserialized]
        public void InitializeAfterDeserialization(StreamingContext context)
        {
            LoadDirectory(CurrentPlace.FullName);
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //managed resources
                }
                //unmanaged resources
                connection.Dispose();
            }

            isDisposed = true;
        }

        ~FTPFileSystem()
        {
            Dispose(false);
        }
        #endregion
    }
}