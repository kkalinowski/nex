using System.IO;
using System.Windows;
using lib12.DependencyInjection;
using nex.DirectoryView;
using nex.FileSystem.Windows;
using nex.Operations;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ClipboardPasteCommand : OperationCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            //get data from clipboard
            var data = Clipboard.GetDataObject();

            //files
            var paths = (string[])data.GetData(DataFormats.FileDrop, true);

            if (paths == null)
            {
                MessageBox.Show("Schowek jest pusty");
                return;
            }

            //get flag indicating whether it is copy or cut
            var stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
            bool copyFlag = stream.ReadByte() == 5 ? true : false;

            //change paths into IDirectoryViewItems
            var items = new IDirectoryViewItem[paths.Length];
            for (int i = 0; i < paths.Length; i++)
                items[i] = WindowsFile.CreateFromPath(paths[i]);

            MultiFileOperation operation;
            if (copyFlag)
                operation = new CopyOperation(items, active.FullPath, new WindowsFileSystem(), active.FileSystem);
            else
                operation = new MoveOperation(items, active.FullPath, new WindowsFileSystem(), active.FileSystem);

            OperationManager.ExecuteOperation(operation);
        } 
        #endregion
    }
}
