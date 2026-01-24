using AuditReadyExpense.Domain;

namespace AuditReadyExpense.Api.Contracts;

public sealed class AuditEventResponse
{
    public ExpenseStatus FromStatus { get; init; }
    public ExpenseStatus ToStatus { get; init; }
    public Guid ActorUserId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public string? Reason { get; init; }
}