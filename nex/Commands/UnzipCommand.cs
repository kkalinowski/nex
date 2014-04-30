using System.IO;
using System.Windows;
using Ionic.Zip;
using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class UnzipCommand : OperationCommand
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
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            if (!active.FileSystem.IsWindowsFileSystem || !ZipFile.IsZipFile(active.SelectedItem.FullName))
            {
                MessageBox.Show("Można rozpakować tylko windows'owe", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (ZipFile zip = ZipFile.Read(active.SelectedItem.FullName))
            {
                var pathToExtracted = Path.Combine(active.FullPath, Path.GetFileNameWithoutExtension(active.SelectedItem.FullName));
                Directory.CreateDirectory(pathToExtracted);
                zip.ExtractAll(pathToExtracted);
            }
        }
        #endregion
    }
}
