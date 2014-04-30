using lib12.DependencyInjection;
using nex.Dialogs.AccountManagerDialog;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ManageAccountsCommand : DynamicCommand
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
            var dialog = Instances.Get<AccountManagerDialog>();
            dialog.ShowModalDialog();
        }
        #endregion
    }
}
