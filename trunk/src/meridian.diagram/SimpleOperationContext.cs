using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using meridian.core;

namespace meridian.diagram
{
    public class SimpleOperationContext : IOperationContext
    {
        public SimpleOperationContext()
        {
            Bind();
        }

        private void Bind()
        {
            m_Lists.Clear();
            m_Lists[ElementType.Proto] = m_Protos;
            m_Lists[ElementType.View] = m_Views;
            m_Lists[ElementType.Aggregation] = m_Aggregations;
            m_Lists[ElementType.Composition] = m_Compositions;
            m_Lists[ElementType.Association] = m_Associations;
            m_Lists[ElementType.InlineComposition] = m_InlineCompositions;
            m_Lists[ElementType.Controller] = m_Controllers;
            m_Lists[ElementType.Interface] = m_Interfaces;
        }

        public void Generate(IGenerator _generator)
        {
            // protos
            foreach (var proto in m_Protos)
            {
                var pc = _generator.CreateProtoClass(proto);
                pc.WriteFile();
                var psc = _generator.CreateprotoStoreClass(proto);
                psc.WriteFile();
                var aspxGrid = _generator.CreateGrid(proto);
                aspxGrid.WriteFile();
                var aspxIndex = _generator.CreateIndex(proto);
                aspxIndex.WriteFile();
                var aspxSingle = _generator.CreateSingle(proto);
                aspxSingle.WriteFile();
                var metaFile = _generator.CreateMetaFile(proto);
                metaFile.WriteFile();

            }

            foreach (var controller in m_Controllers)
            {
                var pcontroller = _generator.CreateControllerClass(controller);
                pcontroller.WriteFile();
                var ucontroller = _generator.CreateControllerImplClass(controller);
                ucontroller.WriteFile();
            }

            var loader = _generator.CreateLoaderClass(this);
            loader.WriteFile();
        }

        public AggregationDescription[] GetAggregationsFor(Element _element)
        {
            return m_Aggregations.Where(s => s.ThisElementName.Equals(_element.Name)).ToArray();
        }

        public AggregationDescription[] GetParentAggregationsFor(Element _element)
        {
            return m_Aggregations.Where(s => s.ForeignElementName.Equals(_element.Name)).ToArray();
        }

        public CompositionDescription[] GetCompositionsFor(Element _element)
        {
            return m_Compositions.Where(s => s.FirstElementName.Equals(_element.Name)).Union(
                m_Compositions.Where(s => s.SecondElementName.Equals(_element.Name)).Select(c => c.Reverse())
                ).ToArray();
        }


        /// <summary>
        /// automatically merges compositions
        /// </summary>
        /// <param name="_element"></param>
        /// <returns></returns>
        public InlineCompositionDescription[] GetInlineCompositionsFor(Element _element)
        {
            return m_InlineCompositions.Where(s => s.FirstElementName.Equals(_element.Name)).Union(
                m_InlineCompositions.Where(s => s.SecondElementName.Equals(_element.Name)).Select(c => c.Reverse())
                ).ToArray();
        }

        public IEnumerable<StoredProcedureDescription> GetAllStoredProcedures()
        {
            return m_StoredProcedures;
        }

        public IEnumerable<ProtoDescription> GetAllProtos()
        {
            return m_Protos;
        }

        public IEnumerable<CompositionDescription> GetAllCompositions()
        {
            return m_Compositions;
        }

        public void Load(string _fileName)
        {
            try
            {
                var serializer = new XmlSerializer(GetType());
                using (var fileStream = new FileStream(_fileName, FileMode.Open))
                {
                    var tw = new StreamReader(fileStream);
                    var obj =
                    serializer.Deserialize(tw) as SimpleOperationContext;
                    this.m_Protos = obj.m_Protos;
                    this.m_Aggregations = obj.m_Aggregations;
                    this.m_Views = obj.m_Views;
                    this.m_Compositions = obj.m_Compositions;
                    this.m_Associations = obj.m_Associations;
                    this.m_InlineCompositions = obj.m_InlineCompositions;
                    this.m_StoredProcedures = obj.m_StoredProcedures;
                    Bind();
                    fileStream.Close();
                }

            }
            catch (Exception _e)
            {
                Tracer.I.Debug(_e.Message);
                Tracer.I.Debug(_e.StackTrace);
            }
        }

        public void Save(string _fileName)
        {
            var serializer = new XmlSerializer(GetType());
            using (var fileStream = new FileStream(_fileName, FileMode.Create))
            {
                var tw = new StreamWriter(fileStream);
                serializer.Serialize(tw, this);
                fileStream.Close();
            }
        }

        public ProtoDescription CreateProto(string _name)
        {
            var existing = m_Protos.SingleOrDefault(s => s.Name.Equals(_name));
            if (existing != null)
            {
                m_Protos.Remove(existing);
            }

            var r = new ProtoDescription();
            r.Type = ElementType.Proto;
            r.Name = _name;
            r.Backend = _name;
            m_Protos.Add(r);
            Tracer.I.Notice("Added proto {0}", _name);
            return r;
            //throw new System.NotImplementedException();
        }

        public AggregationDescription CreateAggregation(string _complexName, string _foreignComplexName)
        {
            var aggDesc = new AggregationDescription();
            var subName = "";
            var firstItem = ResolveComplexParent(_complexName, out subName);
            if (firstItem == null)
            {
                Tracer.I.Error("No this item found (sub {0}, all {1})", subName, _complexName);
                return null;
            }

            var subName2 = "";

            var secondParent = ResolveComplexParent(_foreignComplexName, out subName2);
            var secondItem = ResolveComplex(_foreignComplexName);

            if (secondItem == null)
            {
                Tracer.I.Error("No foreign item found for {0}", _foreignComplexName);
                return null;
            }

            aggDesc.Name = subName;
            aggDesc.ThisElementName = firstItem.Name;
            aggDesc.ThisElementType = firstItem.Type;
            aggDesc.ForeignElementName = secondParent.Name;
            aggDesc.ForeignElementType = secondParent.Type;
            aggDesc.ForeignKey = secondItem.Name;


            m_Aggregations.Add(aggDesc);
            return aggDesc;
        }

        /// <summary>
        /// Create a composition for two proto
        /// generated members should be
        /// 
        /// </summary>
        /// <param name="_complexNameDouble">{proto1}.{proto2}.{composition_name}</param>
        /// <param name="_complexForeignNameDouble">{mediator}.{proto1fieldname}.{proto2fieldname}</param>
        /// <returns></returns>
        public CompositionDescription CreateComposition(string _complexNameDouble, string _complexForeignNameDouble, IBackendOperationContext _backend)
        {
            var compositionDescription = new CompositionDescription();

            var proto1name = "";
            var proto2name = "";
            var compositionName = "";

            var splitterproto = SplitComplexDouble(_complexNameDouble, out proto1name, out proto2name, out compositionName);

            if (splitterproto == null)
            {
                Tracer.I.Error("Invalid operand {0} for aggregation", _complexNameDouble);
                return null;
            }

            if(m_Protos.Count( s => s.Name.Equals(proto1name)) == 0)
            {
                Tracer.I.Error("Proto {0} not found", proto1name);
                return null;
            }

            if (m_Protos.Count(s => s.Name.Equals(proto2name)) == 0)
            {
                Tracer.I.Error("Proto {0} not found", proto2name);
                return null;
            }

            var mediatorbackend = "";
            var mediatorfirstkey = "";
            var mediatorsecondkey = "";


            var splitter = SplitComplexDouble(_complexForeignNameDouble, out mediatorbackend, out  mediatorfirstkey, out mediatorsecondkey);

            if (splitter == null)
            {
                Tracer.I.Error("Invalid operand {0} for aggregation", _complexForeignNameDouble);
                return null;
            }

            
            if (!_backend.HasTable(mediatorbackend))
            {
                Tracer.I.Error("Mediator table {0} does not exist", mediatorbackend);
                return null;
            }

            var mediator = _backend.GetTableFields(mediatorbackend);

            if (mediator.Count(s => s.Name.Equals(mediatorfirstkey)) == 0)
            {
                Tracer.I.Error("Mediator first key {0} does not exist", mediatorfirstkey);
                return null;
            }

            if (mediator.Count(s => s.Name.Equals(mediatorsecondkey)) == 0)
            {
                Tracer.I.Error("Mediator second key {0} does not exist", mediatorsecondkey);
                return null;
            }

            compositionDescription.FirstElementName = proto1name;
            compositionDescription.SecondElementName = proto2name;
            compositionDescription.MediatorBackend = mediatorbackend;
            compositionDescription.MediatorFirstKeyName = mediatorfirstkey;
            compositionDescription.MediatorSecondKeyName = mediatorsecondkey;
            compositionDescription.FirstElementType = Resolve(proto1name).Type;
            compositionDescription.SecondElementType = Resolve(proto2name).Type;

            compositionDescription.Name = compositionDescription.GetQualifiedName(compositionName);

            if (Exists(compositionDescription.Name))
            {
                Tracer.I.Error("Element with name {0} already exists", compositionDescription.Name);
                return null;
            }

            m_Compositions.Add(compositionDescription);
            return compositionDescription;
        }

        public InlineCompositionDescription CreateInlineComposition(string _complexNameDouble, string _complexForeignName, IBackendOperationContext _backend)
        {
            var inlineCompositionDescription = new InlineCompositionDescription();

            var proto1name = "";
            var proto2name = "";
            var compositionName = "";

            var splitterproto = SplitComplexDouble(_complexNameDouble, out proto1name, out proto2name, out compositionName);

            if (splitterproto == null)
            {
                Tracer.I.Error("Invalid operand {0} for aggregation", _complexNameDouble);
                return null;
            }

            if (m_Protos.Count(s => s.Name.Equals(proto1name)) == 0)
            {
                Tracer.I.Error("Proto {0} not found", proto1name);
                return null;
            }

            if (m_Protos.Count(s => s.Name.Equals(proto2name)) == 0)
            {
                Tracer.I.Error("Proto {0} not found", proto2name);
                return null;
            }

            var inlineFieldName = "";
            var inlineCompositionTable = ResolveComplexParent(_complexForeignName, out inlineFieldName);

            if (inlineCompositionTable == null)
            {
                Tracer.I.Error("Table {0} not found", _complexForeignName);
                return null;
            }

            if (String.IsNullOrEmpty(inlineFieldName))
            {
                Tracer.I.Error("Field {0} not found", _complexForeignName);
                return null;
            }
            
            inlineCompositionDescription.FirstElementName = proto1name;
            inlineCompositionDescription.SecondElementName = proto2name;
            inlineCompositionDescription.MediatorBackend = proto1name;
            inlineCompositionDescription.MediatorKeyName = inlineFieldName;
            inlineCompositionDescription.FirstElementType = Resolve(proto1name).Type;
            inlineCompositionDescription.SecondElementType = Resolve(proto2name).Type;

            inlineCompositionDescription.Name = inlineCompositionDescription.GetQualifiedName(compositionName);

            if (Exists(inlineCompositionDescription.Name))
            {
                Tracer.I.Error("Element with name {0} already exists", inlineCompositionDescription.Name);
                return null;
            }

            m_InlineCompositions.Add(inlineCompositionDescription);
            return inlineCompositionDescription;
        }


        public AssociationDescription CreateAssociation(string _complexName, string _complexForeignNameDouble)
        {
            throw new NotImplementedException();
        }

        public ViewDescription CreateView(string _name, string _protoName)
        {
            var view = new ViewDescription();
            view.Name = _name;
            view.m_ProtoBase.Add(_protoName);
            return view;
        }

        public FieldDescription CreateField(Element _parent, FieldDescription _element)
        {
            var field = new FieldDescription();
            field.Backend = _element.Backend;
            field.FieldType = _element.FieldType;
            field.Type = ElementType.Field;
            field.Name = _element.Name;
            switch (_parent.Type)
            {
                case ElementType.Proto:
                    var proto = GetProto(_parent.Name);
                    proto.Fields.Add(field);
                    break;
            }

            Tracer.I.Notice("Added field {0}", _element.Name);
            return field;
        }

        public bool Exists(ElementType _type, string _name)
        {
            return m_Lists[_type].Count(s => s.Name.Equals(_name)) > 0;
        }

        public void DropProto(string _name)
        {
            var proto = GetProto(_name);
            m_Protos.Remove(proto);
            
            //throw new System.NotImplementedException();
        }

        public void DropField(string _parentName, string _name)
        {
            //throw new System.NotImplementedException();
        }

        public ProtoDescription GetProto(string _name)
        {
            return m_Protos.Single(s => s.Name.Equals(_name));
        }

        public Element Resolve(string _name)
        {
            return GetAllElements().Single(s => s.Name.Equals(_name));
        }

        // todo
        public string SplitComplexDouble(string _complexNameDouble, out string _parentName, out string _name1, out string _name2)
        {
            var items = _complexNameDouble.Split('.');
            if (items.Length < 3)
            {
                _parentName = null;
                _name1 = null;
                _name2 = null;
                return null;
            }

            _parentName = items[0];
            _name1 = items[1];
            _name2 = items[2];
            return _parentName;
        }

        public Element ResolveComplexParent(string _complexName, out string _subName)
        {
            var cmpn = _complexName.Split('.');
            if (cmpn.Length < 2)
            {
                _subName = null;
                return null;
            }
            var result = new Element();
            _subName = cmpn[1];
            return Resolve(cmpn[0]);
        }

        public Element ResolveComplex(string _complexName)
        {
            var subName = "";
            var element = ResolveComplexParent(_complexName, out subName);
            if (element == null)
            {
                return null;
            }

            switch (element.Type)
            {
                case ElementType.Proto:
                    var proto = element as ProtoDescription;
                    return proto.Fields.SingleOrDefault(s => s.Name.Equals(subName));
                default: break;
            }

            return null;
        }


        public bool Exists(string _name)
        {
            return GetAllElements().Count(s => s.Name.Equals(_name)) > 0;
        }

        public List<Element> GetAllElements()
        {
            var list = new List<Element>();

            foreach (var item in m_Lists.Values)
            {
                list.AddRange(item);
            }

            return list;
        }

        private SortedList<ElementType, IEnumerable<Element>> m_Lists = new SortedList<ElementType, IEnumerable<Element>>();
        public List<ProtoDescription> m_Protos = new List<ProtoDescription>();
        public List<ViewDescription> m_Views = new List<ViewDescription>();
        public List<AggregationDescription> m_Aggregations = new List<AggregationDescription>();
        public List<CompositionDescription> m_Compositions = new List<CompositionDescription>();
        public List<InlineCompositionDescription> m_InlineCompositions = new List<InlineCompositionDescription>();
        public List<AssociationDescription> m_Associations = new List<AssociationDescription>();
        public List<StoredProcedureDescription> m_StoredProcedures = new List<StoredProcedureDescription>();
        public List<Controller> m_Controllers = new List<Controller>();
        public List<Interface> m_Interfaces = new List<Interface>();


        public IEnumerable<Controller> GetAllControllers()
        {
            return m_Controllers;
        }

        public IEnumerable<Interface> GetAllInterfaces()
        {
            return m_Interfaces;
        }


        public Controller CreateController(string _name, string _proto)
        {
            var existing = m_Controllers.SingleOrDefault(s => s.Name.Equals(_name));
            if (existing != null)
            {
                m_Controllers.Remove(existing);
            }
            var r = new Controller();
            r.Type = ElementType.Controller;
            r.Name = _name;
            r.Backend = _proto;
            m_Controllers.Add(r);
            Tracer.I.Notice("Added controller {0}", _name);
            return r;
            //throw new System.NotImplementedException();
        }

        public Interface CreateInterface(string _name)
        {
            var existing = m_Interfaces.SingleOrDefault(s => s.Name.Equals(_name));
            if (existing != null)
            {
                m_Interfaces.Remove(existing);
            }
            var r = new Interface();
            r.Type = ElementType.Interface;
            r.Name = _name;
            m_Interfaces.Add(r);
            Tracer.I.Notice("Added controller {0}", _name);
            return r;
        }
    }
}