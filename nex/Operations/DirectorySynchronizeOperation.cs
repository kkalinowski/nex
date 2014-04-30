using System;
using nex.Dialogs.DirectorySynchronizeDialog;
using nex.DirectoryView;
using nex.FileSystem;
using nex.FileSystem.FTP;
using nex.FileSystem.Windows;

namespace nex.Operations
{
    [Serializable]
    public class DirectorySynchronizeOperation : MultiFileOperation
    {
        #region Props
        public DirectorySynchronizeResult SyncResult { get; private set; }
        public FileSystemBase LeftSystem { get; private set; }
        public FileSystemBase RightSystem { get; private set; }
        #endregion

        public DirectorySynchronizeOperation(DirectorySynchronizeResult syncRes, IDirectoryViewItem[] items, FileSystemBase leftSystem, FileSystemBase rightSystem) : base(items, leftSystem)
        {
            SyncResult = syncRes;
            LeftSystem = leftSystem;
            RightSystem = rightSystem;
            OperationName = "Synchronizacja katalogów";
        }

        public override bool CanUndo
        {
            get
            {
                return false;
            }
        }

        protected override void Execute()
        {
            Progress = 0;

            foreach (DirectoryComparison dc in SyncResult.Comparison)
            {
                if (IsCanceled)
                    break;

                //copy to right
                if (dc.Result == DirectoryComparisonResult.LeftNewer)
                {
                    CurrentItem = dc.Left;
                    CommenceCopy(SyncResult.RightDir, LeftSystem, RightSystem);
                }
                else //copy to left
                {
                    CurrentItem = dc.Right;
                    CommenceCopy(SyncResult.LeftDir, RightSystem, LeftSystem);
                }
            }

            if (!IsCanceled)
                OnFinished();
            else
                Rollback();
        }

        public bool IsWindowsCopy()
        {
            return LeftSystem.IsWindowsFileSystem && RightSystem.IsWindowsFileSystem;
        }

        private void CommenceCopy(string destPath, FileSystemBase sourceFS, FileSystemBase destFS)
        {
            if (IsWindowsCopy())
                ((WindowsFile)CurrentItem).SimpleCopyTo(destPath);
            else if (sourceFS.IsWindowsFileSystem && !destFS.IsWindowsFileSystem)
                ((FTPFileSystem)destFS).Upload((WindowsFile)CurrentItem);
            else if (!sourceFS.IsWindowsFileSystem && destFS.IsWindowsFileSystem)
                ((FTPFileSystem)sourceFS).Download((FTPFile)CurrentItem, destPath);
            else
                System.Diagnostics.Debug.Assert(true, "Not supported copy");
        }
    }
}