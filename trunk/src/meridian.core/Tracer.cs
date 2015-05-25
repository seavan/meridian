using System.IO;
using System.Text;

namespace meridian.core
{
    public static class Tracer
    {
        static Tracer()
        {
            m_Tracer.AddTracer(new Log4NetTracer());
        }

        public static string Dump(object o)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(o.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, o);
            string json = Encoding.Default.GetString(ms.ToArray());
            return json;
        }

        public static void AddTracer(ITracer _tracer)
        {
            m_Tracer.AddTracer(_tracer);
        }

        public static ITracer I
        {
            get
            {
                return m_Tracer;
            }
        }

        private static AggregateTracer m_Tracer = new AggregateTracer();
    }
}