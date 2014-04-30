using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class UnselectAllCommand : StaticCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            active.UnselectAll();
            active.SetFocusOnContent();
        }
        #endregion
    }
}
