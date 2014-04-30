using System.Linq;
using lib12.DependencyInjection;
using nex.Dialogs.DirectorySynchronizeDialog;
using nex.Operations;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class SyncDirectoriesCommand : OperationCommand
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
            var source = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var dest = MainViewModel.InActiveDirectoryContainer.ActiveView;
            var dialog = new DirectorySynchronizeDialog(source.FullPath, dest.FullPath);
            if (dialog.ShowModalDialog())
            {
                var syncRes = dialog.SyncResult;
                var items = syncRes.Comparison.Select(c => c.Result == DirectoryComparisonResult.LeftNewer ? c.Left : c.Right).ToArray();
                var operation = new DirectorySynchronizeOperation(syncRes, items, source.FileSystem, dest.FileSystem);
                OperationManager.ExecuteOperation(operation);
            }
        }
        #endregion
    }
}
