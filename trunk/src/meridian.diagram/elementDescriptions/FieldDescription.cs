using System.Collections.Generic;

namespace meridian.diagram
{
    public class FieldDescription : ElementDescription
    {
        public FieldType FieldType { get; set; }
        public int MaxLength { get; set; }

        public string GuiType { get; set; }
        public List<string> GuiValidators { get; set; }
    }
}