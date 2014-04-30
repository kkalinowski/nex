using lib12.Collections;
using lib12.DependencyInjection;
using nex.FileSystem;
using nex.Operations;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class NewDirectoryCommand : OperationCommand
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
            var newDirectoryName = MessageService.ShowInput("Podaj nazwę katalogu:");
            if (newDirectoryName.IsNotNullAndNotEmpty())
            {
                var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
                var operation = new NewFolderOperation(PathExt.Combine(active.FullPath, newDirectoryName, active.FileSystem.IsWindowsFileSystem), active.FileSystem);
                OperationManager.ExecuteOperation(operation);
            }
        }
        #endregion
    }
}
