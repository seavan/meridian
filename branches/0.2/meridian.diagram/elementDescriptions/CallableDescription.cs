using System.Collections.Generic;

namespace meridian.diagram
{
    public class CallableDescription : ElementDescription
    {
        public CallableDescription()
        {
            ParameterTypes = new List<ParameterDescription>();
        }

        public List<ParameterDescription> ParameterTypes { get; set; }

        public ParameterDescription ReturnType { get; set; }
    }
}