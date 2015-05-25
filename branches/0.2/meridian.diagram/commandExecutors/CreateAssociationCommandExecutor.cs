using meridian.core;

namespace meridian.diagram
{
    /// <summary>
    /// Class creates a field according to a command parameters
    /// Example of command involving CreateField:
    /// ALTER PROTO publications CREATE FIELD isbn string BACKEND isbn
    /// This command creates a field in both operation context and backend (database) operation context
    /// If the backend is specified
    /// </summary>
    public class CreateAssociationCommandExecutor : ICommandExecutor
    {
        public ICommandExecutor Revert()
        {
            return new DropFieldCommandExecutor();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            Tracer.I.Debug("Creating aggregation");

            var desc = _operationContext.CreateAggregation(_command.Element.Name, _command.SecondElement.Name);
            if (desc == null)
            {
                Tracer.I.Error("Creating aggregation not successfull");
            }
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
            return ElementType.Aggregation;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.None;
        }
    }
}