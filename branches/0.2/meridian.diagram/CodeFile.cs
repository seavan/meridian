using System.Collections.Generic;
using System.IO;

namespace meridian.diagram
{
    public class CodeFile
    {
        public CodeFile()
        {
            Usings = new List<string>();
            Usings.Add("System");
            Usings.Add("System.Linq");
            Usings.Add("System.Text");
            Usings.Add("System.Data.Linq");
         //   Usings.Add("System.Data.SqlClient");
            Usings.Add("System.Collections.Generic");
        }

        public void WriteFile()
        {
            using (var file = File.Create(FilePath))
            {
                var writer = new CSharpWriter(file);

                foreach (var u in Usings)
                {
                    writer.WriteUsing(u);
                }

                writer.WriteBlank();

                writer.WriteNamespace(Namespace);

                writer.OpenBlock();

                WriteInners(writer);

                writer.CloseBlock();

                writer.Flush();
                file.Close();
            }


        }

        public virtual void WriteInners(CSharpWriter _writer)
        {

        }

        public string FilePath { get; set; }
        public List<string> Usings { get; set; }
        public string Namespace { get; set; }


    }
}