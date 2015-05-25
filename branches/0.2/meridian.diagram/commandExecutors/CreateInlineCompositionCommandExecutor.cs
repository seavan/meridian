using System;
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
    public class CreateInlineCompositionCommandExecutor : ICommandExecutor
    {
        public ICommandExecutor Revert()
        {
            throw new SystemException();
        }

        public void Execute(Command _command, IOperationContext _operationContext, IBackendOperationContext _backendOperationContext)
        {
            Tracer.I.Debug("Creating inline composition");

            var desc = _operationContext.CreateInlineComposition(_command.Element.Name, _command.SecondElement.Name, _backendOperationContext);
            if (desc == null)
            {
                Tracer.I.Error("Creating inline Composition is not successfull");
                return;
            }

            Tracer.I.Notice("Composition created successfully");
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
            return ElementType.InlineComposition;
        }

        public ElementType GetParentElementType()
        {
            return ElementType.None;
        }
    }
}