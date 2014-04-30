using System;
using System.Linq;
using lib12.Collections;
using nex.DirectoryView;
using nex.FileSystem;

namespace nex.Operations
{
    /// <summary>
    /// Represents the delete operation
    /// </summary>
    [Serializable]
    public class DeleteOperation : MultiFileOperation
    {
        public override bool CanUndo
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Executes the delete operation
        /// </summary>
        protected override void Execute()
        {
            double progressMeter = 1.0 / Items.Length;
            foreach (IDirectoryViewItem item in Items)
            {
                if (IsCanceled)
                    break;

                CurrentItem = item;
                OperationName = "Usuwanie " + CurrentItem.Name;
                FileSystem.Delete(item);
                Progress += progressMeter;
            }

            OperationName = Items.ContainsOneElement() ? "Usuwanie " + Items.First().Name : string.Format("Usuwanie {0} obiektów", Items.Length);
            if (!IsCanceled)
                OnFinished();
        }

        public DeleteOperation(FileSystemBase fileSystem, IDirectoryViewItem[] items)
            : base(items, fileSystem)
        {
        }
    }
}