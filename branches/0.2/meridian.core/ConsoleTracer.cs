using System;
using System.Collections;

namespace meridian.core
{
    public class ConsoleTracer : ITracer
    {
        public void Trace(string _message, params object[] _params)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("= " +  _message, _params);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Debug(string _message, params object[] _params)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("= " + _message, _params);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Notice(string _message, params object[] _params)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("= " + _message, _params);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Error(string _message, params object[] _params)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("= " + _message, _params);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Assert(object _value, string _message = "")
        {
            if (_value == null)
            {
                Error("Value cannot be null. {0}", _message);
            }

            //throw new NotImplementedException();
        }

        public void AssertEmpty(IList _value, string _message = "")
        {
            Assert((object)_value);
            if (_value.Count != 0)
            {
                Error("Value must be empty. {0}", _message);
            }            
        }

        public void AssertCount(IList _value, int _count, string _message = "")
        {
            Assert((object)_value);
            if (_value.Count != 0)
            {
                Error("Value must be empty. {0}", _message);
            }        
        }
    }
}