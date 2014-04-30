using System.Linq;
using lib12.DependencyInjection;
using nex.Dialogs.FileCompareDialog;
using nex.DirectoryView;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class CompareFilesCommand : OperationCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            var activeView = MainViewModel.ActiveDirectoryView;
            var inactiveView = MainViewModel.InActiveDirectoryContainer.ActiveView;
            return (activeView.SelectedItemsCount == 2 && activeView.SelectedItems.Cast<IDirectoryViewItem>().All(x => x.IsWindowsFile))
                || (activeView.IsOneFileSelected && inactiveView.IsOneFileSelected);
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var activeView = MainViewModel.ActiveDirectoryView;
            var inactiveView = MainViewModel.InActiveDirectoryContainer.ActiveView;
            string firstFile = null, secondFile = null;

            firstFile = ((IDirectoryViewItem)activeView.SelectedItems.First()).FullName;
            if (activeView.SelectedItemsCount == 2)
                secondFile = ((IDirectoryViewItem)activeView.SelectedItems[1]).FullName;
            else
                secondFile = inactiveView.SelectedItem.FullName;

            var dialog = new FileCompareDialog(firstFile, secondFile);
            dialog.Show();
        }
        #endregion
    }
}
