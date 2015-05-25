namespace meridian.diagram
{
    public interface ICommandOptions
    {
        
    }


    public interface ICommandExecutor
    {
        ICommandExecutor Revert();
        void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext);
        bool RequiresOperand();
        bool RequiresSecondOperand();
        ICommandOptions GetOptions();
        CommandType GetCommandType();
        ElementType GetElementType();
        ElementType GetParentElementType();
    }
}