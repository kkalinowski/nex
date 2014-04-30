using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class MoveForwardCommand : StaticCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            MainViewModel.ActiveDirectoryContainer.ActiveView.MoveForward();
        } 
        #endregion
    }
}
