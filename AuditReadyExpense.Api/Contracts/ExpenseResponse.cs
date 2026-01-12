using AuditReadyExpense.Domain;

namespace AuditReadyExpense.Api.Contracts;

public sealed class ExpenseResponse
{
    public Guid Id { get; init; }
    public Guid CreatedByUserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public ExpenseStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}