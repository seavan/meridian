using System.Collections.Generic;

namespace meridian.diagram
{
    public interface IOperationContext
    {
        void Load(string _fileName);
        void Save(string _fileName);
        ProtoDescription CreateProto(string _name);
        AggregationDescription CreateAggregation(string _complexName, string _complexForeignName);
        CompositionDescription CreateComposition(string _complexName, string _complexForeignNameDouble, IBackendOperationContext _backend);
        /// <summary>
        /// Create inline composition (inline composition stores values in varchar comma-separated)
        /// There can be several compositions by the same name
        /// </summary>
        /// <param name="_complexName">{proto1}.{proto2}.{composition_name}
        /// proto1 - proto type, which contains the field with the comma-separated values
        /// proto2 - related proto
        /// </param>
        /// <param name="_complexForeignName">{mediator}.{field_name}</param>
        /// <param name="_backend"></param>
        /// <returns></returns>
        InlineCompositionDescription CreateInlineComposition(string _complexName, string _complexForeignName, IBackendOperationContext _backend);
        AssociationDescription CreateAssociation(string _complexName, string _complexForeignNameDouble);
        ViewDescription CreateView(string _name, string _protoName);
        FieldDescription CreateField(Element _parent, FieldDescription _element);
        bool Exists(ElementType _type, string _name);
        

        void DropProto(string _name);
        void DropField(string _parentName, string _name);

        void Generate(IGenerator _generator);

        AggregationDescription[] GetAggregationsFor(Element _element);
        AggregationDescription[] GetParentAggregationsFor(Element _element);

        CompositionDescription[] GetCompositionsFor(Element _element);
        InlineCompositionDescription[] GetInlineCompositionsFor(Element _element);

        IEnumerable<ProtoDescription> GetAllProtos();
        IEnumerable<CompositionDescription> GetAllCompositions();
    }
}