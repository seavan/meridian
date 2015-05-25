using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace meridian.core
{
    public class FileTracer : ITracer
    {
        public FileTracer(string _path, string _debugFile = null, string _noticeFile = null, string _errorFile = null)
        {
            var ts = DateTime.Now.ToShortDateString();
            m_DebugFile = CheckInit(Path.Combine(_path, String.Format(_debugFile, ts)));
            m_NoticeFile = CheckInit(Path.Combine(_path, String.Format(_noticeFile, ts)));
            m_ErrorFile = CheckInit(Path.Combine(_path, String.Format(_errorFile, ts)));
        }

        private string CheckInit(string _path)
        {
            try
            {
                File.AppendAllText(_path, DateTime.Now.ToString() + ": trace initialized");
            }
            catch(Exception _e)
            {
                return null;
            }

            return _path;
        }

        private void WriteMessagOut(string _message, params string[] _files)
        {
            var files =
                _files.Where(s => !String.IsNullOrEmpty(s));
            foreach(var f in files)
            {
                try
                {
                    File.AppendAllText(f, String.Format("{0}: {1}\r\n", DateTime.Now.ToString(), _message));
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private string m_DebugFile;
        private string m_NoticeFile;
        private string m_ErrorFile;

        public void Trace(string _message, params object[] _params)
        {
            WriteMessagOut(string.Format(_message, _params), m_NoticeFile, m_DebugFile);
        }

        public void Debug(string _message, params object[] _params)
        {
            //WriteMessagOut(string.Format(_message, _params), m_DebugFile);
        }

        public void Notice(string _message, params object[] _params)
        {
            //WriteMessagOut(string.Format(_message, _params), m_NoticeFile, m_DebugFile);
        }

        public void Error(string _message, params object[] _params)
        {
            WriteMessagOut(string.Format(_message, _params), m_DebugFile, m_ErrorFile);
        }

        public void Assert(object _value, string _message = "")
        {
            
        }

        public void AssertEmpty(IList _value, string _message = "")
        {
            
        }

        public void AssertCount(IList _value, int _count, string _message = "")
        {
            
        }
    }
}