using System.Linq;

namespace meridian.diagram
{
    public class ClassFile : CodeFile
    {
        public override void WriteInners(CSharpWriter _writer)
        {
            base.WriteInners(_writer);

            _writer.WriteClass(ClassName);
            _writer.OpenBlock();

            // constructor
            _writer.WriteFunction("public", null, ClassName);
            _writer.OpenBlock();
            Constructor(_writer);
            _writer.CloseBlock();

            // fields
            Fields(_writer);

            // functions
            Functions(_writer);

            // functions
            Properties(_writer);


            _writer.CloseBlock();
        }

        public virtual void Constructor(CSharpWriter _writer)
        {
        }

        public virtual void Functions(CSharpWriter _writer)
        {
        }


        public virtual void Properties(CSharpWriter _writer)
        {
        }

        public virtual void Fields(CSharpWriter _writer)
        {
        }

        public string NormalizeName(string _name)
        {
            var reserved = new string[] { "event", "delegate", "class" }.ToList();

            if (reserved.Contains(_name))
            {
                return "@" + _name;
            }

            return _name;
        }

        public string ClassName { get; set; }

        public string AdoNetPrefix { get; set; }
        public string FieldSeparatorL { get; set; }
        public string FieldSeparatorR { get; set; }
        public string AdoNetNamespace { get; set; }

    }
}