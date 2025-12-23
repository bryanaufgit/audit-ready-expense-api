namespace AuditReadyExpense.Domain.Exceptions;

public class DomainExceptions : Exception
{
    public DomainErrorCode ErrorCode { get; }

    public DomainExceptions(
        DomainErrorCode errorCode,
        string message
    ) : base(message)
    {
        ErrorCode = errorCode;
    }
}