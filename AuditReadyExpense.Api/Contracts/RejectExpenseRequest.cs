namespace AuditReadyExpense.Api.Contracts;

public sealed class RejectExpenseRequest
{
    public string Reason { get; set; } = string.Empty;
}