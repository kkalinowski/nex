using lib12.DependencyInjection;
using nex.Dialogs.SearchDialog;
using nex.Utilities;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class SearchCommand : StaticCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var dialog = new SearchDialog();
            if (dialog.ShowModalDialog())
                MainViewModel.ActiveDirectoryContainer.ActiveView.LoadDirectory(dialog.FoundObject);
        } 
        #endregion
    }
}
