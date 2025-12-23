namespace AuditReadyExpense.Domain.Expenses;

public sealed record AuditEvent(
    Guid ExpenseId,
    ExpenseStatus FromStatus,
    ExpenseStatus ToStatus,
    Guid ActorUserId,
    DateTime OccurredAtUtc,
    string? Reason = null
);