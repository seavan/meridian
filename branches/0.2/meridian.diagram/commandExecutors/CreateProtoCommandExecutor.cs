using meridian.core;

namespace meridian.diagram
{
    public class CreateProtoCommandExecutor : ICommandExecutor
    {
        #region ICommandExecutor Members

        public ICommandExecutor Revert()
        {
            return new DropProtoCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            if (!_backendOperationContext.HasTable(_command.Element.Name))
            {
                Tracer.I.Error("Backend has no such table {0}", _command.Element.Name);
                return;
            }

            if(_operationContext.Exists(ElementType.Proto, _command.Element.Name))
            {
                Tracer.I.Error("Proto already exists {0}", _command.Element.Name);
                return;
            }

            var proto = _operationContext.CreateProto(_command.Element.Name);

            var fields = _backendOperationContext.GetTableFields(_command.Element.Name);

            foreach (var field in fields)
            {
                _operationContext.CreateField(proto, field);
            }

            /*if (string.IsNullOrEmpty(_command.Element.Backend))
            {
                _backendOperationContext.CreateTable(_command.Element.Name);
                _backendOperationContext.AddPrimary(_command.Element.Name);
            }*/
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
            return null;
        }

        public CommandType GetCommandType()
        {
            return CommandType.Create;
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