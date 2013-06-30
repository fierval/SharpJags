using System;

namespace SharpJags
{
    public class JagsException : Exception
    {
        public JagsException(String message) : base(message) { }
    }
}
