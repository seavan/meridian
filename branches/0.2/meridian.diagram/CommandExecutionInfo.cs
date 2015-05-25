using System.Collections.Generic;

namespace meridian.diagram
{
    public class CommandExecutionInfo
    {
        public Command ActualCommand { get; set; }
        public IList<ICommandExecutor> CommandExecutor { get; set; }
        public bool SkipOptions { get; set; }
        public bool SkipFirstOperandType { get; set; }
        public bool SkipFirstOperand { get; set; }
        public bool SkipSecondOperand { get; set; }
        public bool SkipSecondOperandType { get; set; }

    }
}