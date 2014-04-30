
namespace nex.Commands
{
    public abstract class OperationCommand : DynamicCommand
    {
        #region Props
        public OperationsManager OperationManager { get; set; }
        #endregion
    }
}
