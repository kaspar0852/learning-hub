namespace Marketplace.Application.Exceptions;

public sealed class ExternalServiceAppException : AppException
{
    public ExternalServiceAppException(string message)
        : base(message)
    {
    }
}
