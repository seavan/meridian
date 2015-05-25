namespace meridian.diagram
{
    public class Element
    {
        public string Name { get; set; }
        public string BeautifulName { get; set; }
        public string Modifier { get; set; }
        public string GetBeautifulName()
        {
            return string.IsNullOrEmpty(BeautifulName) ? Name : BeautifulName;
        }

        public string GetModifier()
        {
            return string.IsNullOrEmpty(Modifier) ? "public" : Modifier;
        }

        public string Backend { get; set; }
        public ElementType Type { get; set; }
    }
}