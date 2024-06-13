namespace NATSInternal.Services.Exceptions;

public class OperationException : Exception
{

    public OperationException(string message) : base(message) { }

    public OperationException(string propertyName, string message) : base(message)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; set; }
}
