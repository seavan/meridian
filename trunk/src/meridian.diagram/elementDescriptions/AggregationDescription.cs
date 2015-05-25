namespace meridian.diagram
{
    public class AggregationDescription : ElementDescription
    {
        public AggregationDescription()
        {
            Type = ElementType.Aggregation; ;
        }
        public ElementType ThisElementType { get; set; }
        public string ThisElementName { get; set; }
        public ElementType ForeignElementType { get; set; }
        
        public string ForeignElementName { get; set; }
        public string ForeignKey { get; set; }
    }
}