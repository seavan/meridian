using System;

namespace meridian.core
{

    public interface ITracer
    {
        void Trace(Exception e);
        void Trace(string _message, params object[] _params);
        void Debug(string _message, params object[] _params);
        void Notice(string _message, params object[] _params);
        void Error(string _message, params object[] _params);
    }
}