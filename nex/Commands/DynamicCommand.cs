using System;
using System.Windows.Input;

namespace nex.Commands
{
    public abstract class DynamicCommand : ICommand
    {
        #region CanExecute
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual bool CanExecute(object parameter)
        {
            return MainViewModel.ActiveDirectoryView != null && MainViewModel.ActiveDirectoryView.IsAtLeastOneItemSelected;
        }
        #endregion

        #region Props
        public MainViewModel MainViewModel { get; set; }
        #endregion

        #region Execute
        public abstract void Execute(object parameter);
        #endregion
    }
}
