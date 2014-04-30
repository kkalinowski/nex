using System.Windows;
using lib12.DependencyInjection;
using nex.Operations;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class DeleteCommand : OperationCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var items = MainViewModel.GetSelectedItems();
            if (items.Length == 0)
            {
                MessageBox.Show("Zaznacz obiekty do skasowania");
                return;
            }

            if ((items.Length == 1 &&
                 MessageBox.Show("Czy chcesz usunąć " + items[0].Name + "?",
                     "Potwierdzenie usunięcia", MessageBoxButton.YesNo,
                     MessageBoxImage.Question) == MessageBoxResult.Yes) ||
                (items.Length > 1 &&
                 MessageBox.Show("Czy chcesz usunąć " + active.SelectedItemsCount + " obiektów?",
                     "Potwierdzenie usunięcia", MessageBoxButton.YesNo,
                     MessageBoxImage.Question) == MessageBoxResult.Yes))
            {
                var operation = new DeleteOperation(active.FileSystem, items);
                OperationManager.ExecuteOperation(operation);
            }
        }
        #endregion
    }
}
