using lib12.DependencyInjection;
using nex.DirectoryView;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ChangeViewCommand : DynamicCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            return MainViewModel.ActiveDirectoryView.ViewType != (DirectoryViewType)parameter;
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            MainViewModel.ActiveDirectoryView.ViewType = (DirectoryViewType)parameter;
        }
        #endregion
    }
}
