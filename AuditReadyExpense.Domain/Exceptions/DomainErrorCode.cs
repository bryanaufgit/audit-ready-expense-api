namespace AuditReadyExpense.Domain.Exceptions;

public enum DomainErrorCode
{
    InvalidStateTransition,
    MissingRequieredField,
    InvalidAmount,
    ReceiptReuquired,
    UnauthorizedAction
}