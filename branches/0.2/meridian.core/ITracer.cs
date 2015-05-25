using System.Collections;

namespace meridian.core
{

    public interface ITracer
    {
        
        void Trace(string _message, params object[] _params);
        void Debug(string _message, params object[] _params);
        void Notice(string _message, params object[] _params);
        void Error(string _message, params object[] _params);
        void Assert(object _value, string _message = "");
        void AssertEmpty(IList _value, string _message = "");
        void AssertCount(IList _value, int _count, string _message = "");
    }
}