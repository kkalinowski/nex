using System.Diagnostics;
using System.Windows;
using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class OpenConsoleCommand : DynamicCommand
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

            //cmd /K "cd /d C:\Windows\"
            var process = new Process();
            var startInfo = new ProcessStartInfo("cmd");
            startInfo.Arguments = string.Format("/K \"cd /d {0}\"", active.FullPath);
            process.StartInfo = startInfo;
            process.Start();
        } 
        #endregion
    }
}
