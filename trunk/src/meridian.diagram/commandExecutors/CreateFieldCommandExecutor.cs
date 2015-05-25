using System;

namespace meridian.diagram
{
    /// <summary>
    /// Class creates a field according to a command parameters
    /// Example of command involving CreateField:
    /// ALTER PROTO publications CREATE FIELD isbn string BACKEND isbn
    /// This command creates a field in both operation context and backend (database) operation context
    /// If the backend is specified
    /// </summary>
    public class CreateFieldCommandExecutor : ICommandExecutor
    {
        public ICommandExecutor Revert()
        {
            return new DropFieldCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            if ((_command.Arguments == null) || (_command.Arguments.Count != 1))
            {
                throw new InvalidOperationException();
            }

            FieldType dbType = Naming.CastFieldType(_command.Arguments[0]);

            //_operationContext.CreateField(_command.Parent, _command.Element );

            if (string.IsNullOrEmpty(_command.Element.Backend))
            {
                _backendOperationContext.AddColumn(_command.Parent.Name, _command.Element.Name, dbType);
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
            return null;
        }

        public CommandType GetCommandType()
        {
            return CommandType.Create; 
        }

        public ElementType GetElementType()
        {
            return ElementType.Field;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.Proto;
        }
    }
}