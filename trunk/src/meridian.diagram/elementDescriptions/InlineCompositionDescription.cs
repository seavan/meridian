using System.Linq;

namespace meridian.diagram
{
    public class InlineCompositionDescription : ElementDescription
    {
        public InlineCompositionDescription()
        {
            Type = ElementType.InlineComposition;
            FirstElementInterface = "None";
        }

        public string GetQualifiedName(string _name)
        {
            var strings = new string[] { FirstElementName, SecondElementName };
            strings = strings.OrderBy(s => s).ToArray();
            return string.Join("_", strings) + "_" + _name;
        }

        public string FirstElementName { get; set; }
        public ElementType FirstElementType { get; set; }
        public string SecondElementName { get; set; }
        public ElementType SecondElementType { get; set; }
        public bool Reversed { get; set; }
        public string MediatorBackend { get; set; }
        public string MediatorKeyName { get; set; }
        public string FirstElementInterface { get; set; }

        public InlineCompositionDescription Reverse()
        {
            return new InlineCompositionDescription()
            {
                Name = this.Name,
                MediatorBackend = this.MediatorBackend,
                FirstElementName = this.SecondElementName,
                SecondElementName = this.FirstElementName,
                FirstElementType = this.SecondElementType,
                SecondElementType = this.FirstElementType,
                MediatorKeyName = this.MediatorKeyName,
                FirstElementInterface = this.FirstElementInterface,
                Reversed = true
            };
        }
    }
}