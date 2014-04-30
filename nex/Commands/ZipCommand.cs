using System.IO;
using System.Linq;
using Ionic.Zip;
using lib12.Collections;
using lib12.DependencyInjection;
using nex.Dialogs.CompressDialog;
using nex.DirectoryView;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ZipCommand : OperationCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            var view = MainViewModel.ActiveDirectoryView;
            return view.FileSystem.IsWindowsFileSystem
                && view.SelectedItems.IsNotNullAndNotEmpty()
                && view.SelectedItems.Cast<IDirectoryViewItem>().Any(x => !x.IsMoveUp);
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            var dialog = new CompressDialog();
            if (dialog.ShowModalDialog())
            {
                using (var zipFile = new ZipFile())
                {
                    foreach (var item in MainViewModel.GetSelectedItems())
                    {
                        if (item.IsMoveUp)
                            continue;
                        else if (item.IsDirectory)
                            zipFile.AddDirectory(item.FullName);
                        else
                            zipFile.AddFile(item.FullName);
                    }

                    if (!string.IsNullOrEmpty(dialog.Password))
                    {
                        zipFile.Password = dialog.Password;
                        zipFile.Encryption = EncryptionAlgorithm.WinZipAes256;
                    }

                    zipFile.Save(Path.Combine(active.FullPath, dialog.FileName));
                }
            }
        }
        #endregion
    }
}
