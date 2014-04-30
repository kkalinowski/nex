using System.Windows;
using lib12.DependencyInjection;
using nex.FileSystem.Windows;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class EncryptCommand : DynamicCommand
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
            var item = MainViewModel.ActiveDirectoryContainer.ActiveView.SelectedItem;
            if (!item.IsWindowsFile || item.IsDirectory)
            {
                MessageBox.Show("Szyfrować można tylko pliki Windows'e", "Bląd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var wfile = (WindowsFile)item;
            if ((string)parameter == "encrypt")
                wfile.Encrypt();
            else
                wfile.Decrypt();
        }
        #endregion
    }
}
