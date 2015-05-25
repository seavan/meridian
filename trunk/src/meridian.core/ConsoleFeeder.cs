using System;

namespace meridian.core
{
    public class ConsoleFeeder : IFeeder
    {
        public bool Eof()
        {
            return false;
        }

        public string Next()
        {
            return Console.ReadLine();
        }
    }
}