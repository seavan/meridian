using meridian.core;

namespace meridian.diagram
{
    public class CreateViewCommandExecutor : ICommandExecutor
    {
        public ICommandExecutor Revert()
        {
            // todo
            return new DropProtoCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            if (_operationContext.Exists(ElementType.View, _command.Element.Name))
            {
                Tracer.I.Error("View already exists {0}", _command.Element.Name);
                return;
            }

            if (!_operationContext.Exists(ElementType.Proto, _command.SecondElement.Name))
            {
                Tracer.I.Error("Proto {0} does not exist", _command.SecondElement.Name);
                return;

            }

            var view = _operationContext.CreateView(_command.Element.Name, _command.SecondElement.Name);
        }

        public bool RequiresOperand()
        {
            return true;
        }

        public bool RequiresSecondOperand()
        {
            return true;
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
            return ElementType.View;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.None;
        }
    }
}