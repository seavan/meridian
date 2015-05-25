using System.Collections.Generic;
using System.Xml.Serialization;

namespace meridian.diagram
{
    public class ProtoDescription : ElementDescription
    {
        public ProtoDescription()
        {
            Fields = new List<FieldDescription>();
        }

        public bool SkipLoad { get; set; }

        public List<FieldDescription> Fields { get; set; }

        public string CustomKeyName { get; set; }

        [XmlIgnore]
        public IEnumerable<AggregationDescription> Aggregations { get; set; }

        [XmlIgnore]
        public IEnumerable<AggregationDescription> ParentAggregations { get; set; }

        [XmlIgnore]
        public IEnumerable<CompositionDescription> Compositions { get; set; }

        [XmlIgnore]
        public IEnumerable<InlineCompositionDescription> InlineCompositions { get; set; }

    }
}