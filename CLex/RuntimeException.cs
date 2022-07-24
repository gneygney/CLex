using System.Runtime.Serialization;

namespace CLex
{
    [Serializable]
    internal class RuntimeException : Exception
    {
        private readonly string v;
        public readonly Token token;

        public RuntimeException()
        {
        }

        public RuntimeException(string? message) : base(message)
        {
        }

        public RuntimeException(Token o, string message) : base(message)
        {
            token = o;
        }

        public RuntimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}