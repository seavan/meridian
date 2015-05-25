using System.IO;

namespace meridian.diagram
{
    public class BasicFile
    {
        public virtual void WriteCode(CSharpWriter _writer)
        {
            
        }

        public void WriteFile()
        {
            using (var file = File.Create(FilePath))
            {
                var writer = new CSharpWriter(file);

                WriteCode(writer);

                writer.Flush();
                file.Close();
            }


        }

        public string FilePath { get; set; }
    }
}