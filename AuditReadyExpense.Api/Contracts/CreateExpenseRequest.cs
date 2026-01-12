namespace AuditReadyExpense.Api.Contracts;

public sealed class CreateExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}