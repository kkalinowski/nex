using System.Windows;
using lib12.DependencyInjection;
using nex.FileSystem.Windows;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class FinishSharingDirectoryCommand : OperationCommand
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
            if (active.SelectedItemsCount != 1 || !active.SelectedItem.IsDirectory || !active.FileSystem.IsWindowsFileSystem)
            {
                MessageBox.Show("Zaznacz jeden folder windows'owy, dla którego chcesz zakończyć udostępnianie");
                return;
            }

            try
            {
                ////connect to win api
                //ManagementClass win32ShareDelete = new ManagementClass("root\\cimv2", string.Format("Win32_Share.Name='{0}'", active.SelectedItem.Name), null);
                //ManagementBaseObject outParams;
                ////calling method
                //outParams = win32ShareDelete.InvokeMethod("Delete", null, null);
                ////checking result
                //if ((uint)outParams["ReturnValue"] != 0)
                //    MessageBox.Show("Nie udało się zakończyć udostępniania");
                //else
                //    MessageBox.Show("Udostępnianie zostało zakończone");
                WindowsSharing wShare = WindowsSharing.GetNamedShare(active.SelectedItem.Name);
                WindowsSharing.MethodStatus res = wShare.Delete();
                if (res != WindowsSharing.MethodStatus.Success)
                    MessageBox.Show("Nie udało się zakończyć udostępniania");
                else
                    MessageBox.Show("Udostępnianie zostało zakończone");
            }
            catch
            {
                MessageBox.Show("Nie udało się zakończyć udostępniania");
            }
        } 
        #endregion
    }
}
