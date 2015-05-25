namespace meridian.diagram
{
    public class DropProtoCommandExecutor : ICommandExecutor
    {

        #region ICommandExecutor Members

        public ICommandExecutor Revert()
        {
            return new CreateProtoCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            _operationContext.DropProto(_command.Element.Name);

            if (string.IsNullOrEmpty(_command.Element.Backend))
            {
                // TODO: drop fields one by one
                _backendOperationContext.DropTable(_command.Element.Name);
            }
        }

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
            throw new System.NotImplementedException();
        }

        public CommandType GetCommandType()
        {
            return CommandType.Drop;
        }

        public ElementType GetElementType()
        {
            return ElementType.Proto;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.None;
        }


        #endregion
    }
}