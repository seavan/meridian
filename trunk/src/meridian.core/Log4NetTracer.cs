using System;
using log4net;
using log4net.Config;

namespace meridian.core
{
    public class Log4NetTracer : ITracer
    {
        public Log4NetTracer()
        {
            XmlConfigurator.Configure();
        }

        private static readonly ILog log = LogManager.GetLogger("Meridian");

        public void Trace(string _message, params object[] _params)
        {
            log.InfoFormat(_message, _params);
        }

        public void Debug(string _message, params object[] _params)
        {
            log.DebugFormat(_message, _params);
        }

        public void Notice(string _message, params object[] _params)
        {
            log.DebugFormat(_message, _params);
        }

        public void Error(string _message, params object[] _params)
        {
            log.ErrorFormat(_message, _params);
        }

        public void Trace(Exception e)
        {
            log.Error("Unexpected failure", e);
        }
    }
}