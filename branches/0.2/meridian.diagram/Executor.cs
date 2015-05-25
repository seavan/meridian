using System.Collections.Generic;
using meridian.core;
using System.Linq;

namespace meridian.diagram
{
    public class Executor : IExecutor
    {
        public Executor(IDiagramContext _diagramContext)
        {
            m_DiagramContext = _diagramContext;
            m_CommandTypes["CREATE"] = CommandType.Create;
            m_CommandTypes["ALTER"] = CommandType.Alter;
            m_CommandTypes["DROP"] = CommandType.Drop;
            m_CommandTypes["RECREATE_PROTOS"] = CommandType.RecreateProtos; 

        }

        public void Register(ICommandExecutor _executor)
        {
            m_Executors.Add(_executor.GetCommandType(), _executor);
        }

/*        public void Execute(CommandType _commandType, Element _parent, Element _operand)
        {
            var command = new Command()
                {
                    Element = _operand,
                    Parent = _parent,
                    Type = _commandType
                };

            m_Executors[_commandType].Execute(
                command, GetDiagramContext().GetOperationContext(), GetDiagramContext().GetBackendOperationContext()
                );

            GetDiagramContext().IncreaseVersion();

            m_Commands.Add(command);
        } */

        public void RevertAll()
        {

        }

        public void RevertLast()
        {
        }



        private IDiagramContext GetDiagramContext()
        {
            return m_DiagramContext;
        }

        public HashSet<ICommandExecutor> GetCommandExecutor(string _command)
        {
            return m_Executors[m_CommandTypes[_command.ToUpper()]];
        }

        #region IExecutor Members

        public bool CommandExists(string _command)
        {
            _command = _command.ToUpper();
            return (m_CommandTypes.IndexOfKey(_command) != -1)
                && m_Executors.ContainsKey(m_CommandTypes[_command]);
        }

        public void StartBuilding(string _command)
        {
            AbortBuilding();
            Tracer.I.Debug("Start building command {0}", _command);
            m_CurrentCommandExecution = new CommandExecutionInfo();
            m_CurrentCommandExecution.ActualCommand = new Command();
            m_CurrentCommandExecution.CommandExecutor = GetCommandExecutor(_command).ToList();
        }

        public void FeedOptions(SortedList<string, string> _options)
        {
            m_CurrentCommandExecution.SkipOptions = true;
            Tracer.I.Debug("Got {0} parsed options", _options.Keys.Count);
        }

        public void FeedOperandType(ElementType _operandType)
        {
            m_CurrentCommandExecution.SkipOptions = true;
            if (m_CurrentCommandExecution.SkipFirstOperandType)
            {
                m_CurrentCommandExecution.SkipSecondOperandType = true;
                Tracer.I.Debug("Got second operand type {0}", _operandType);
                m_CurrentCommandExecution.ActualCommand.SecondElement.Type = _operandType;
            }
            else
            {
                m_CurrentCommandExecution.SkipFirstOperandType = true;
                Tracer.I.Debug("Got first operand type {0}", _operandType);
                m_CurrentCommandExecution.ActualCommand.Element.Type = _operandType;
            }
            
        }

        public void FeedOperand(string _operand)
        {
            m_CurrentCommandExecution.SkipOptions = true;

            if (m_CurrentCommandExecution.SkipFirstOperand)
            {
                m_CurrentCommandExecution.SkipSecondOperandType = true;
                m_CurrentCommandExecution.SkipSecondOperand = true;
                Tracer.I.Debug("Got second operand {0}", _operand);
                m_CurrentCommandExecution.ActualCommand.SecondElement.Name = _operand;
            }
            else
            {
                m_CurrentCommandExecution.SkipFirstOperandType = true;
                m_CurrentCommandExecution.SkipFirstOperand = true;
                Tracer.I.Debug("Got first operand {0}", _operand);
                
                //Tracer.I.AssertEmpty(m_CurrentCommandExecution.ActualCommand.Arguments);
                //m_CurrentCommandExecution.ActualCommand.Arguments.Add(_operand);
                m_CurrentCommandExecution.ActualCommand.Element.Name = _operand;
            }
        }

        public void AbortBuilding()
        {
            Tracer.I.Debug("Abort building command");
            m_CurrentCommandExecution = null;
        }

        public void CompleteCommand()
        {
            var cmdList = m_CurrentCommandExecution.CommandExecutor.Where(
                s => s.GetElementType().Equals(m_CurrentCommandExecution.ActualCommand.Element.Type)
                ).ToArray();

            if (cmdList.Length == 0)
            {
                Tracer.I.Error("No suitable command found for this operand type and command");
                return;
            }

            if (cmdList.Length > 1)
            {
                Tracer.I.Error("Ambiguous commands found for this operand type and command");
                return;
            }

            var executor = cmdList.First();
            executor.Execute(m_CurrentCommandExecution.ActualCommand, m_DiagramContext.GetOperationContext(), m_DiagramContext.GetBackendOperationContext());
        }

        public SyntaxElement Expected()
        {
            SyntaxElement expected = SyntaxElement.seNone;

            if (!m_CurrentCommandExecution.SkipOptions) expected |=  SyntaxElement.seOptions;

            bool wantFirstOperand = m_CurrentCommandExecution.CommandExecutor.Count(s => s.RequiresOperand()) > 0;
            
            if (wantFirstOperand && !m_CurrentCommandExecution.SkipFirstOperandType) expected |= SyntaxElement.seOperandType;

            if (wantFirstOperand && m_CurrentCommandExecution.SkipFirstOperandType && !m_CurrentCommandExecution.SkipFirstOperand) expected |= SyntaxElement.seComplexIdentifier;

            if (!wantFirstOperand || (m_CurrentCommandExecution.SkipFirstOperandType && m_CurrentCommandExecution.SkipFirstOperand)) expected |= SyntaxElement.seEol; 

            if(m_CurrentCommandExecution.CommandExecutor.Count( s => !s.RequiresOperand()) > 0) expected |= SyntaxElement.seEol;

            if (m_CurrentCommandExecution.SkipFirstOperand && m_CurrentCommandExecution.SkipFirstOperandType)
            {
                bool wantSecondOperand = m_CurrentCommandExecution.CommandExecutor.Count(s => s.RequiresSecondOperand()) > 0;
                if (wantSecondOperand && !m_CurrentCommandExecution.SkipSecondOperandType) expected |= SyntaxElement.seOperandType;
                if (wantSecondOperand && m_CurrentCommandExecution.SkipSecondOperandType && !m_CurrentCommandExecution.SkipSecondOperand) expected |= SyntaxElement.seComplexIdentifier;
            }

            return expected;
        }


        #endregion

        MultiValueDictionary<CommandType, ICommandExecutor> m_Executors = new MultiValueDictionary<CommandType, ICommandExecutor>();
        List<Command> m_Commands = new List<Command>();
        SortedList<string, CommandType> m_CommandTypes = new SortedList<string, CommandType>();
        IDiagramContext m_DiagramContext;
        CommandExecutionInfo m_CurrentCommandExecution = null;

    }
}