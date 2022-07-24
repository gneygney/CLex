using System.Runtime.Serialization;

namespace CLex;

internal class ParserException : Exception
{
    public readonly Token token;

    public ParserException()
    {
    }

    public ParserException(Token o, string message) : base(message)
    {
        token = o;
    }


    public ParserException(string? message) : base(message)
    {
    }

    public ParserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}