namespace AuditReadyExpense.Domain.Exceptions;

public enum DomainErrorCode
{
    InvalidStateTransition,
    MissingRequiredField,
    InvalidAmount,
    ReceiptRequired,
    UnauthorizedAction
}