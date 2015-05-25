using System.Collections.Generic;

namespace meridian.diagram
{
    public class Command
    {
        public Command()
        {
            Element = new Element(); 
            SecondElement = new Element();
            Arguments = new List<string>();
        }
        public Element Parent { get; set; }
        public Element Element { get; set; }
        public Element SecondElement { get; set; }
        public CommandType Type { get; set; }
        public List<string> Arguments { get; set; }
    }
}