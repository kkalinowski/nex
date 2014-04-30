using System.Linq;
using lib12.DependencyInjection;
using nex.Dialogs.MultiRenameDialog;
using nex.Dialogs.RenameDialog;
using nex.DirectoryView;
using nex.Operations;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class RenameCommand : OperationCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;

            if (active.SelectedItemsCount == 1)
                RenameSingleItem(active);
            else
                RenameMultiItems(active);
        }
        #endregion

        #region Logic
        private void RenameSingleItem(DirectoryViewModel active)
        {
            var renameDialog = Instances.Get<RenameDialog>();
            renameDialog.ToRename = active.SelectedItem.FullName;
            if (renameDialog.ShowModalDialog())
            {
                //active.UnselectAll();//need because rename operation works strange when selecting new item after finishing operation
                var renameOp = new RenameOperation(active.SelectedItem, renameDialog.NewName, active.FileSystem);
                OperationManager.ExecuteOperation(renameOp);
            }
        }

        private void RenameMultiItems(DirectoryViewModel active)
        {
            bool dirChange = ((IDirectoryViewItem)active.SelectedItems[0]).IsDirectory;
            if (!active.SelectedItems.Cast<IDirectoryViewItem>().All(x => x.IsDirectory == dirChange))
                MessageService.ShowError("Naraz można zmienić nazwy tylko jednego typu obiektów - plików lub katalogów");

            var dialog = new MultiRenameDialog(active.SelectedItems);
            if (dialog.ShowModalDialog())
            {
                var operation = new MultiRenameOperation(dialog.Items, MainViewModel.GetSelectedItems(), active.FileSystem);
                OperationManager.ExecuteOperation(operation);
            }
        }
        #endregion
    }
}
