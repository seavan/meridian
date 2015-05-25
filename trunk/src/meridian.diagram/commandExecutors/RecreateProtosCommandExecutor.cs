using meridian.core;
using System.Linq;

namespace meridian.diagram
{
    public class RecreateProtosCommandExecutor : ICommandExecutor
    {
        #region ICommandExecutor Members

        public ICommandExecutor Revert()
        {
            return new DropProtoCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            var protos = _operationContext.GetAllProtos().ToArray();

            foreach (var i in protos)
            {
                var proto = _operationContext.CreateProto(i.Name);
                var fields = _backendOperationContext.GetTableFields(i.Name);
                foreach (var field in fields)
                {
                    _operationContext.CreateField(proto, field);
                }
            }

            /*if (string.IsNullOrEmpty(_command.Element.Backend))
            {
                _backendOperationContext.CreateTable(_command.Element.Name);
                _backendOperationContext.AddPrimary(_command.Element.Name);
            }*/
        }

        public bool RequiresOperand()
        {
            return false;
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
            return CommandType.RecreateProtos;
        }

        public ElementType GetElementType()
        {
            return ElementType.None;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.None;
        }

        #endregion
    }
}