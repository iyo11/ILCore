namespace ILCore.Exceptions;

public class IlCoreException : Exception
{
    public IlCoreException() { }

    public IlCoreException(string message) : base(message)
    {
        throw new Exception(message);
    }
    
    public IlCoreException(string message, Exception innerException)
        : base(message, innerException) { }
}