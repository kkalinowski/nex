using System.Windows;
using lib12.DependencyInjection;
using nex.FileSystem.Windows;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ShareDirectoryCommand : OperationCommand
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
                MessageBox.Show("Zaznacz jeden folder windows'owy, który chcesz udostępnić");
                return;
            }

            var folderToShare = (WindowsFile)active.SelectedItem;
            try
            {
                ////connect to win api
                //ManagementClass win32Share = new ManagementClass("Win32_Share");
                //ManagementBaseObject inParams = win32Share.GetMethodParameters("Create");
                //ManagementBaseObject outParams;
                ////set parameters
                //inParams["Name"] = active.SelectedItem.Name;
                //inParams["Path"] = active.SelectedItem.FullName;
                //inParams["Description"] = string.Format("Folder {0} udostępniony w programie nex", active.SelectedItem.Name);
                //inParams["Type"] = 0x0;//folder sharing
                //inParams["Access"] = null;
                ////calling method
                //outParams = win32Share.InvokeMethod("Create", inParams, null);
                ////checking result
                //if ((uint)outParams["ReturnValue"] != 0)
                //    MessageBox.Show("Nie udało się udostępnić folderu");
                //else
                //    MessageBox.Show("Folder został udostępniony");
                //-------------------------------------------------------------------------------------------------------------
                //SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                //NTAccount acc = sid.Translate(typeof(NTAccount)) as NTAccount;
                ////string everyoneAccount = acc.ToString();
                //DirectorySecurity dsec= folderToShare.GetDirectoryAccessControl();
                //dsec.AddAccessRule(new FileSystemAccessRule(acc.ToString(), FileSystemRights.Read, AccessControlType.Allow));
                //folderToShare.SetDirectoryAccessControl(dsec);
                //NTAccount account = new NTAccount("Domain Users");
                //SecurityIdentifier sId = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                //DirectorySecurity dirSec = new DirectorySecurity();
                //FileSystemAccessRule accessRule =
                //    new FileSystemAccessRule(sId, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
                //dirSec.AddAccessRule(accessRule);
                //Directory.SetAccessControl(active.SelectedItem.FullName, dirSec);
                //--------------------------------------------------------------------------------------------------------------
                WindowsSharing.MethodStatus res = WindowsSharing.Create(folderToShare.FullName, folderToShare.Name, WindowsSharing.ShareType.DiskDrive, 10, "Folder shared in program nex", null);
                if (res != WindowsSharing.MethodStatus.Success)
                    MessageBox.Show("Nie udało się udostępnić folderu");
                else
                    MessageBox.Show("Folder został udostępniony");
            }
            catch
            {
                MessageBox.Show("Nie udało się udostępnić folderu");
            }
        } 
        #endregion
    }
}
