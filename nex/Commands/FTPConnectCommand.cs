using lib12.DependencyInjection;
using nex.Dialogs.AccountManagerDialog;
using nex.FileSystem.FTP;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class FTPConnectCommand : DynamicCommand
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
            dialog.IsCancelable = true;

            if (dialog.ShowModalDialog())
                MainViewModel.ActiveDirectoryContainer.ActiveView.ChangeFileSystem(new FTPFileSystem(dialog.SelectedAccount));
        }
        #endregion
    }
}
