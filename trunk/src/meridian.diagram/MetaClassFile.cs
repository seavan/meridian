namespace meridian.diagram
{
    public class MetaClassFile : ClassFile
    {
        public MetaClassFile(ProtoDescription _proto)
        {
            m_Proto = _proto;
            ClassName = _proto.Name + "_meta";
            //Inherits = string.Format("BaseUniversalController<{0}>", _controller.Backend);
        }

        public override void Functions(CSharpWriter _writer)
        {
            /*
                     protected override admin.db.IDataService<news> GetService()
        {
            return Meridian.Default.newsStore;
        }
             */
            _writer.WriteIndentBlank("/* metafile template */");


                foreach (var f in m_Proto.Fields)
                {
                    _writer.WriteIndentBlank("public {0} {1} {{ get; set; }}", f.FieldType.ToCSharp(), NormalizeName(f.Name));
                }
            _writer.WriteIndentBlank("/* metafile template */");
        }

        private ProtoDescription m_Proto;
    }
}