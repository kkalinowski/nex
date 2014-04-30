using lib12.DependencyInjection;
using nex.Operations;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class CopyCommand : OperationCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var source = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var dest = MainViewModel.InActiveDirectoryContainer.ActiveView;

            var operation = new CopyOperation(MainViewModel.GetSelectedItems(), dest.FullPath, source.FileSystem, dest.FileSystem);
            OperationManager.ExecuteOperation(operation);
        }
        #endregion
    }
}
