using System;
using System.Windows.Input;
using lib12.DependencyInjection;
using nex.Operations;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class CancelOperationCommand : ICommand
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
        public OperationsManager OperationsManager { get; set; }
        #endregion

        #region Execute
        public void Execute(object parameter)
        {
            var operation = (OperationBase)parameter;
            OperationsManager.CancelOperation(operation);
        }
        #endregion
    }
}
