using System;
using System.Windows.Input;

namespace nex.Commands
{
    public abstract class StaticCommand : ICommand
    {
        #region CanExecute
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
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
