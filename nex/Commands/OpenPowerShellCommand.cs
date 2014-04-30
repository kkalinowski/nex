using System.Diagnostics;
using System.Windows;
using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class OpenPowerShellCommand : DynamicCommand
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
            if (!active.FileSystem.IsWindowsFileSystem)
            {
                MessageBox.Show("To polecenie działa tylko w windows'owym systemie plików");
                return;
            }

            //powershell -NoExit -Command "& {cd D:\temp1}"
            var process = new Process();
            var startInfo = new ProcessStartInfo("powershell");
            startInfo.Arguments = string.Format("-NoExit -Command \"& {{cd {0}}}\"", active.FullPath);
            process.StartInfo = startInfo;
            process.Start();
        } 
        #endregion
    }
}
