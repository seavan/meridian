namespace meridian.diagram
{
    public class AggregationClassFile : ClassFile
    {
        public AggregationClassFile(AggregationDescription _descr)
        {
            ClassName = _descr.ThisElementName;
            ThisClassName = _descr.ThisElementName;
            ForeignClassName = _descr.ForeignElementName;
            m_Description = _descr;
        }

        public string ThisClassName { get; set; }
        public string ForeignClassName { get; set; }

        private AggregationDescription m_Description;

        public override void Constructor(CSharpWriter _writer)
        {
            base.Constructor(_writer);

        }

        public override void Fields(CSharpWriter _writer)
        {
            base.Fields(_writer);
            // todo 
            _writer.WriteIndentEol("private List<{0}> m_{1}List = new List<{0}>()", ForeignClassName, m_Description.Name);
        }

        public override void Properties(CSharpWriter _writer)
        {
            base.Properties(_writer);

        }

        public override void Functions(CSharpWriter _writer)
        {
            base.Functions(_writer);
            _writer.WriteFunction("public", "void", "LoadFromReader", new string[] { "SqlDataReader" }, new string[] { "_reader" });
            _writer.OpenBlock();
            
            _writer.CloseBlock();

        }

    }
}