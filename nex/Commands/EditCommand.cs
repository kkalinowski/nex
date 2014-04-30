using System.Diagnostics;
using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class EditCommand : DynamicCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            if (active.SelectedItem != null && !active.IsMoveUpSelected && !active.SelectedItem.IsDirectory)
            {
                var psi = new ProcessStartInfo("notepad.exe", active.SelectedItem.FullName);
                Process.Start(psi);
            }
        } 
        #endregion
    }
}
