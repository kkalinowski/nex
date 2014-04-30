using System.IO;
using System.Security.Cryptography;
using lib12.DependencyInjection;
using lib12.Reflection;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ComputeCRCCommand : DynamicCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            var view = MainViewModel.ActiveDirectoryContainer.ActiveView;
            return view.FileSystem.IsWindowsFileSystem && view.IsOneFileSelected;
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var hashAlgorithm = ((HashAlgorithms)parameter).CreateType<HashAlgorithm>();
            var file = new FileStream(active.SelectedItem.FullName, FileMode.Open, FileAccess.Read);
            hashAlgorithm.ComputeHash(file);

            MessageService.ShowInfo("Obliczona suma kontrolna: " + Utility.ConvertByteArrayToHex(hashAlgorithm.Hash));
        }
        #endregion
    }
}
