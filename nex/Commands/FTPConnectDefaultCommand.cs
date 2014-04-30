using System.Windows;
using lib12.DependencyInjection;
using nex.Accounts;
using nex.FileSystem.FTP;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class FTPConnectDefaultCommand : DynamicCommand
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
            var account = AccountManager.FindCurrentDefault();
            if (account == null)
                MessageBox.Show("Nie ma domyślnego konta dla FTP");
            else
                MainViewModel.ActiveDirectoryContainer.ActiveView.ChangeFileSystem(new FTPFileSystem(account));
        }
        #endregion
    }
}
