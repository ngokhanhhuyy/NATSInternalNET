namespace NATSInternal.Services.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException() {}
    
    public ConcurrencyException(string message) : base(message) {}
}