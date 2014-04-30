using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using nex.Dialogs.MultiRenameDialog;
using nex.DirectoryView;
using nex.FileSystem;

namespace nex.Operations
{
    [Serializable]
    public class MultiRenameOperation : MultiFileOperation
    {
        public IEnumerable<MultiRenameItem> MRItems { get; private set; }
        /// <summary>
        /// Get or set list of processed items
        /// </summary>
        public List<IDirectoryViewItem> ProcessedItems { get; set; }

        //OPT: Make this ctor in more elegant way
        public MultiRenameOperation(IEnumerable<MultiRenameItem> mrItems, IDirectoryViewItem[] dvItems, FileSystemBase fileSystem)
            : base(dvItems, fileSystem)
        {
            MRItems = mrItems;
            OperationName = string.Format("Zmiana nazwy {0} obiektów", mrItems.Count());
        }

        public override bool CanUndo
        {
            get
            {
                return true;
            }
        }

        protected override void Execute()
        {
            foreach (MultiRenameItem item in MRItems)
            {
                var newPath = PathExt.Combine(PathExt.GetDirectoryName(item.Item.FullName, item.Item.IsWindowsFile), item.NewName, item.Item.IsWindowsFile);
                if (FileSystem.CheckIfObjectExist(newPath))
                {
                    MessageBox.Show("Obiekt o nazwie " + item.NewName + " już isnieje. Nie mogę przeprowadzić operacji.");
                    IsCanceled = true;
                }
                else
                {
                    FileSystem.Rename(item.Item.FullName, newPath, item.Item.IsDirectory);
                }
            }

            OnFinished();
        }

        protected override void Rollback()
        {
            foreach (MultiRenameItem item in MRItems)
            {
                var oldPath = PathExt.Combine(PathExt.GetDirectoryName(item.Item.FullName, item.Item.IsWindowsFile), item.OldName, item.Item.IsWindowsFile);
                var newPath = PathExt.Combine(PathExt.GetDirectoryName(item.Item.FullName, item.Item.IsWindowsFile), item.NewName, item.Item.IsWindowsFile);
                if (FileSystem.CheckIfObjectExist(oldPath))
                {
                    MessageBox.Show("Obiekt o nazwie " + item.OldName + " już isnieje. Nie mogę cofnąć operacji.");
                }
                else
                {
                    FileSystem.Rename(newPath, oldPath, item.Item.IsDirectory);
                }
            }
        }
    }
}