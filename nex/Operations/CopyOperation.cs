using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using lib12.Collections;
using lib12.Extensions;
using nex.Controls.Dialogs.OperationErrors;
using nex.Dialogs.FileExistDialog;
using nex.DirectoryView;
using nex.FileSystem;
using nex.FileSystem.FTP;
using nex.FileSystem.Windows;
using nex.Utilities;

namespace nex.Operations
{
    /// <summary>
    /// Class represents a copy operation
    /// </summary>
    [Serializable]
    public class CopyOperation : MultiFileOperation
    {
        #region Fields
        private readonly long SizeInBytes;
        private long copiedBytes = 0;
        private bool overrideAll = false;
        #endregion

        #region Props
        /// <summary>
        /// Get or set where to copy files
        /// </summary>
        public string DestinationPath { get; private set; }

        /// <summary>
        /// Get or set list of processed items
        /// </summary>
        public List<IDirectoryViewItem> ProcessedItems { get; set; }

        public override bool CanUndo
        {
            get
            {
                return true;
            }
        }

        public FileSystemBase DestFileSystem { get; private set; }

        public FileSystemBase SourceFileSystem
        {
            get
            {
                return FileSystem;
            }
        }
        #endregion

        #region Methods
        public CopyOperation(IDirectoryViewItem[] items, string destPath, FileSystemBase sourceFileSystem, FileSystemBase destFileSystem)
            : base(items, sourceFileSystem)
        {
            DestinationPath = destPath;
            DestFileSystem = destFileSystem;
            ProcessedItems = new List<IDirectoryViewItem>(items.Length);
            SizeInBytes = ItemsSize.ToBytes();
        }

        /// <summary>
        /// Executes the copy operation
        /// </summary>
        protected override void Execute()
        {
            Progress = 0;
            foreach (IDirectoryViewItem item in Items)
            {
                if (IsCanceled)
                    break;

                CurrentItem = item;
                OperationName = "Kopiowanie " + CurrentItem.Name;

                //check if file exist in destination directory
                if (DestFileSystem.CheckIfObjectExist(PathExt.Combine(DestinationPath, item.Name, DestFileSystem.IsWindowsFileSystem)) && !overrideAll)
                {
                    var dialog = new FileExistDialog(item, DestinationPath);
                    dialog.ShowModalDialog();

                    if (dialog.Result == FileExistDalogResult.Override)
                    {
                        CommenceCopy();
                    }
                    else if (dialog.Result == FileExistDalogResult.OverrideAll)
                    {
                        overrideAll = true;
                        CommenceCopy();
                    }
                    else if (dialog.Result == FileExistDalogResult.CancelOperation)
                    {
                        IsCanceled = true;
                    }
                    //else if(dialog.Result==FileExistDalogResult.DontOverride) - easy go to next item     
                }
                else
                {
                    CommenceCopy();
                }
            }

            OperationName = Items.ContainsOneElement() ? "Kopiowanie " + Items.First().Name : string.Format("Kopiowanie {0} obiektów", Items.Length);
            if (!IsCanceled)
                OnFinished();
            else
                Rollback();
        }

        protected override void Rollback()
        {
            //TODO: Make Undo using DeleteOperation
            FileInfo file;
            DirectoryInfo dir;

            try
            {
                foreach (IDirectoryViewItem item in ProcessedItems)
                {
                    if (!item.Exists)
                        continue;

                    if (!item.IsDirectory)
                    {
                        file = new FileInfo(Path.Combine(DestinationPath, item.Name));
                        file.Delete();
                    }
                    else
                    {
                        dir = new DirectoryInfo(Path.Combine(DestinationPath, item.Name));
                        dir.Delete(true);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Wycofanie operacji się nie powiodło");
            }
        }

        public bool IsWindowsCopy()
        {
            return SourceFileSystem.IsWindowsFileSystem && DestFileSystem.IsWindowsFileSystem;
        }

        private void CommenceCopy()
        {
            if (IsWindowsCopy())
                CurrentItem.CopyTo(DestinationPath, this);
            else if (SourceFileSystem.IsWindowsFileSystem && !DestFileSystem.IsWindowsFileSystem)
                ((FTPFileSystem)DestFileSystem).Upload((WindowsFile)CurrentItem);
            else if (!SourceFileSystem.IsWindowsFileSystem && DestFileSystem.IsWindowsFileSystem)
                ((FTPFileSystem)SourceFileSystem).Download((FTPFile)CurrentItem, DestinationPath);
            else
                System.Diagnostics.Debug.Assert(true, "Not supported copy");

            ProcessedItems.Add(CurrentItem);
        }

        public CopyFileCallbackAction CopiedPieceOfFile(FileInfo source, FileInfo destination, object state,
            long totalFileSize, long totalBytesTransferred)
        {
            if (totalBytesTransferred > 0)
            {
                long totalOperationBytesForNow = copiedBytes + totalBytesTransferred;
                Progress = (totalOperationBytesForNow / (double)SizeInBytes);
                Duration = DateTime.Now - Started;
                Speed = FileSize.CreateFromBytes((long)(totalOperationBytesForNow / Duration.TotalSeconds));
                if (Speed.ToBytes() > 0)
                    EstimatedEnd = TimeSpan.FromSeconds(SizeInBytes / Speed.ToBytes()) - Duration;
                if (totalFileSize == totalBytesTransferred)
                    copiedBytes += totalFileSize;
            }

            return IsCanceled ? CopyFileCallbackAction.Cancel : CopyFileCallbackAction.Continue;
        }
        #endregion
    }
}