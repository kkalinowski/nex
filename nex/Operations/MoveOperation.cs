using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using lib12.Collections;
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
    /// Class represents a move operation
    /// </summary>
    [Serializable]
    public class MoveOperation : MultiFileOperation
    {
        #region Fields
        private readonly long SizeInBytes;
        private long movedBytes = 0;
        private bool overrideAll = false;
        #endregion

        #region Props
        /// <summary>
        /// Get or set where to move files
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
        public MoveOperation(IDirectoryViewItem[] items, string destPath, FileSystemBase sourceFileSystem, FileSystemBase destFileSystem)
            : base(items, sourceFileSystem)
        {
            DestinationPath = destPath;
            DestFileSystem = destFileSystem;
            ProcessedItems = new List<IDirectoryViewItem>(items.Length);
            SizeInBytes = ItemsSize.ToBytes();
        }

        /// <summary>
        /// Executes the move operation
        /// </summary>
        protected override void Execute()
        {
            double progressMeter = 1.0 / Items.Length;
            Progress = 0;

            foreach (IDirectoryViewItem item in Items)
            {
                if (IsCanceled)
                    break;

                CurrentItem = item;
                OperationName = "Przenoszenie " + CurrentItem.Name;

                //check if file exist in destination directory
                if (DestFileSystem.CheckIfObjectExist(PathExt.Combine(DestinationPath, item.Name, DestFileSystem.IsWindowsFileSystem)) && !overrideAll)
                {
                    var dialog = new FileExistDialog(item, DestinationPath);
                    dialog.ShowModalDialog();

                    if (dialog.Result == FileExistDalogResult.Override)
                    {
                        CommenceMove();
                        Progress += progressMeter;
                    }
                    else if (dialog.Result == FileExistDalogResult.OverrideAll)
                    {
                        overrideAll = true;
                        CommenceMove();
                        Progress += progressMeter;
                    }
                    else if (dialog.Result == FileExistDalogResult.CancelOperation)
                    {
                        IsCanceled = true;
                    }
                    //else if(result==FileExistDalogResult.DontOverride) - easy go to next item
                }
                else
                {
                    CommenceMove();
                    Progress += progressMeter;
                }
            }

            OperationName = Items.ContainsOneElement() ? "Przenoszenie " + Items.First().Name : string.Format("Przenoszenie {0} obiektów", Items.Length);
            if (!IsCanceled)
                OnFinished();
            else
                Rollback();
        }

        protected override void Rollback()
        {
            FileInfo file;
            DirectoryInfo dir;

            foreach (IDirectoryViewItem item in ProcessedItems)
            {
                string path = Path.Combine(DestinationPath, item.Name);

                if ((item.IsDirectory && !Directory.Exists(path)) || !File.Exists(path))
                {
                    MessageBox.Show("Obiekt " + item.Name + " został usunięty. Nie mogę cofnąć zmian.");
                    continue;
                }

                if ((item.IsDirectory && Directory.Exists(item.FullName)) || File.Exists(item.FullName))
                {
                    MessageBox.Show("Obiekt " + item.Name + " istnieje w poprzednim katalogu. Nie mogę cofnąć zmian.");
                    continue;
                }

                if (!item.IsDirectory)
                {
                    file = new FileInfo(path);
                    file.MoveTo(item.FullName);
                }
                else
                {
                    dir = new DirectoryInfo(path);
                    dir.MoveTo(item.FullName);
                }
            }
        }

        public bool IsWindowsMove()
        {
            return SourceFileSystem.IsWindowsFileSystem && DestFileSystem.IsWindowsFileSystem;
        }

        private void CommenceMove()
        {
            if (IsWindowsMove())
                CurrentItem.MoveTo(DestinationPath, this);
            else if (SourceFileSystem.IsWindowsFileSystem && !DestFileSystem.IsWindowsFileSystem)
            {
                ((FTPFileSystem)DestFileSystem).Upload((WindowsFile)CurrentItem);
                FileSystem.Delete(CurrentItem);
            }
            else if (!SourceFileSystem.IsWindowsFileSystem && DestFileSystem.IsWindowsFileSystem)
            {
                ((FTPFileSystem)SourceFileSystem).Download((FTPFile)CurrentItem, DestinationPath);
                FileSystem.Delete(CurrentItem);
            }
            else
                System.Diagnostics.Debug.Assert(true, "Not supported move");

            ProcessedItems.Add(CurrentItem);
        }

        public CopyFileCallbackAction MovedPieceOfFile(FileInfo source, FileInfo destination, object state,
            long totalFileSize, long totalBytesTransferred)
        {
            if (totalBytesTransferred > 0)
            {
                long totalOperationBytesForNow = movedBytes + totalBytesTransferred;
                Progress = (totalOperationBytesForNow / (double)SizeInBytes);
                Duration = DateTime.Now - Started;
                Speed = FileSize.CreateFromBytes((long)(totalOperationBytesForNow / Duration.TotalSeconds));
                if (Speed.ToBytes() > 0)
                    EstimatedEnd = TimeSpan.FromSeconds(SizeInBytes / Speed.ToBytes()) - Duration;
                if (totalFileSize == totalBytesTransferred)
                    movedBytes += totalBytesTransferred;
            }

            return IsCanceled ? CopyFileCallbackAction.Cancel : CopyFileCallbackAction.Continue;
        }
        #endregion
    }
}