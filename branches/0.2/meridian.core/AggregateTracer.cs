using System.Collections;
using System.Collections.Generic;

namespace meridian.core
{
    public class AggregateTracer : ITracer
    {
        public AggregateTracer()
        {
        }

        public void AddTracer(ITracer _tracer)
        {
            m_Tracers.Add(_tracer);
        }

        private List<ITracer> m_Tracers = new List<ITracer>();


        public void Trace(string _message, params object[] _params)
        {
            m_Tracers.ForEach(s => s.Trace(_message, _params));
        }

        public void Debug(string _message, params object[] _params)
        {
            m_Tracers.ForEach(s => s.Debug(_message, _params));
        }

        public void Notice(string _message, params object[] _params)
        {
            m_Tracers.ForEach(s => s.Notice(_message, _params));
        }

        public void Error(string _message, params object[] _params)
        {
            m_Tracers.ForEach(s => s.Error(_message, _params));
        }

        public void Assert(object _value, string _message = "")
        {
            m_Tracers.ForEach(s => s.Assert(_value, _message));
        }

        public void AssertEmpty(IList _value, string _message = "")
        {
            m_Tracers.ForEach(s => s.AssertEmpty(_value, _message));
        }

        public void AssertCount(IList _value, int _count, string _message = "")
        {
            m_Tracers.ForEach(s => s.AssertCount(_value, _count, _message));
        }
    }
}