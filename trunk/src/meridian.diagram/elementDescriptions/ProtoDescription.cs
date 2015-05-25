using System.Collections.Generic;
using System.Xml.Serialization;

namespace meridian.diagram
{
    public class InterfaceField
    {
        [XmlAttribute]
        string Type { get; set; }
        [XmlAttribute]
        string Name { get; set; }
    }
    
    public class SyntaxAbstract : Element
    {

        [XmlAttribute]
        public string Implements { get; set; }
    }

    public class Interface : SyntaxAbstract
    {
        
    }

    public class Controller : SyntaxAbstract
    {
        public static string FromProtoName(string _proto)
        {
            return "meridian_" + _proto;
        }
    }

    public class Aspect
    {
        [XmlAttribute]
        public string Field;
        [XmlAttribute]
        public string Name;
    }

    public class ProtoDescription : ElementDescription
    {
        public ProtoDescription()
        {
            Fields = new List<FieldDescription>();
            Aspects = new List<Aspect>();
        }

        public void PreserveUserFields(ProtoDescription _target)
        {
            _target.SphinxSearchable = SphinxSearchable;
            _target.CustomDataServiceCreate = CustomDataServiceCreate;
            _target.CustomDataServiceInsert = CustomDataServiceInsert;
            _target.CustomDataServiceDelete = CustomDataServiceDelete;
            _target.CustomDataServiceUpdate = CustomDataServiceUpdate;
            _target.SkipLoad = SkipLoad;
            _target.CustomKeyName = CustomKeyName;
            _target.BeautifulName = BeautifulName;
            _target.Aspects.AddRange(Aspects.ToArray());
        }

        public bool SkipLoad { get; set; }

        [XmlAttribute]
        public bool CustomDataServiceInsert { get; set; }

        [XmlAttribute]
        public bool CustomDataServiceDelete { get; set; }

        [XmlAttribute]
        public bool CustomDataServiceUpdate { get; set; }

        [XmlAttribute]
        public bool CustomDataServiceCreate { get; set; }

        [XmlAttribute]
        public bool SphinxSearchable { get; set; }


        public List<FieldDescription> Fields { get; set; }

        public List<Aspect> Aspects { get; set; }

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