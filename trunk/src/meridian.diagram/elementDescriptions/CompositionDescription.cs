using System.Linq;


namespace meridian.diagram
{
    public class CompositionDescription : ElementDescription
    {
        public CompositionDescription()
        {
            Type = ElementType.Composition;
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

        public string MediatorBackend { get; set; }
        public string MediatorFirstKeyName { get; set; }
        public string MediatorSecondKeyName { get; set; }

        public CompositionDescription Reverse()
        {
            return new CompositionDescription()
            {
                Name = this.Name,
                MediatorBackend = this.MediatorBackend,
                MediatorFirstKeyName = this.MediatorSecondKeyName,
                MediatorSecondKeyName = this.MediatorFirstKeyName,
                FirstElementName = this.SecondElementName,
                SecondElementName = this.FirstElementName,
                FirstElementType = this.SecondElementType,
                SecondElementType = this.FirstElementType
            };
        }
    }
}