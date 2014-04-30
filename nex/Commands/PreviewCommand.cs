using lib12.DependencyInjection;
using nex.Controls.Preview;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class PreviewCommand : DynamicCommand
    {
        #region CanExecute
        public override bool CanExecute(object parameter)
        {
            return MainViewModel.ActiveDirectoryView != null && MainViewModel.ActiveDirectoryView.IsOneFileSelected;
        }
        #endregion

        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            if (active.SelectedItem != null)
            {
                var pvWindow = new PreviewWindow(false);
                pvWindow.LoadFile(active.SelectedItem);
                pvWindow.ShowDialog();
            }
        } 
        #endregion
    }
}
