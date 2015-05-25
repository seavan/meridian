using System.Collections.Generic;

namespace meridian.diagram
{
    public class ViewDescription : ElementDescription
    {
        public ViewDescription()
        {
            m_ProtoBase = new List<string>();
        }

        public List<string> m_ProtoBase;
    }
}