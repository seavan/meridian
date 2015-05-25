namespace meridian.diagram
{
    public class DropFieldCommandExecutor : ICommandExecutor
    {
        #region ICommandExecutor Members

        public ICommandExecutor Revert()
        {
            return new CreateFieldCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            _operationContext.DropField(_command.Parent.Name, _command.Element.Name);
            
            if (string.IsNullOrEmpty(_command.Element.Backend))
            {
                _backendOperationContext.DropColumn(_command.Parent.Name, _command.Element.Name);
            }
        }

        public CommandType GetCommandType()
        {
            return CommandType.Drop;
        }

        public ElementType GetElementType()
        {
            return ElementType.Field;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.Proto;
        }

        #endregion

        #region ICommandExecutor Members

        public bool RequiresOperand()
        {
            return true;
        }

        public bool RequiresSecondOperand()
        {
            return false;
        }

        public ICommandOptions GetOptions()
        {
            return null;
        }

        #endregion
    }
}