using System;

namespace meridian.diagram
{
    [Flags]
    public enum SyntaxElement
    {
        seNone = 0x0,
        seEol = 0x1,
        seCommand = 0x2,
        seOperandType = 0x4,
        seIdentifier = 0x8,
        seComplexIdentifier = 0x10,
        seOptions = 0x20,

    }
}