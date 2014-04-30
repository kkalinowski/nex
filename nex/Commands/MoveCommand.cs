using System.Windows;
using lib12.DependencyInjection;
using nex.Operations;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class MoveCommand : OperationCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var source = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var dest = MainViewModel.InActiveDirectoryContainer.ActiveView;
            var items = MainViewModel.GetSelectedItems();
            if (items.Length == 0)
            {
                MessageBox.Show("Zaznacz obiekty do przeniesienia");
                return;
            }

            var operation = new MoveOperation(items, dest.FullPath, source.FileSystem, dest.FileSystem);
            OperationManager.ExecuteOperation(operation);
        }
        #endregion
    }
}
