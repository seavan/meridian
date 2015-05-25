using System.Collections.Generic;

namespace meridian.diagram
{
    public interface IExecutor
    {
        bool CommandExists(string _command);
        void StartBuilding(string _command);
        void FeedOptions(SortedList<string, string> _options);
        void FeedOperandType(ElementType _operandType);
        void FeedOperand(string _operand);
        void AbortBuilding();
        void CompleteCommand();
        SyntaxElement Expected();
    }
}