namespace meridian.diagram
{
    public class AssociationDescription : ElementDescription
    {
        public string FirstElementName { get; set; }
        public ElementType FirstElementType { get; set; }
        public string SecondElementName { get; set; }
        public ElementType SecondElementType { get; set; }

        public string FirstElementKeyName { get; set; }
        public string SecondElementKeyName { get; set; }
    }
}