using lib12.DependencyInjection;
using lib12.Extensions;
using nex.Accounts;
using nex.Dialogs.AccountDialogLogic;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class NewAccountCommand : DynamicCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        #endregion

        #region Props
        public AccountManager AccountManager { get; set; }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var dialog = new AccountDialog();
            if (dialog.ShowDialog().IsTrue())
                AccountManager.AddNewAccount(dialog.Account);
        } 
        #endregion
    }
}
