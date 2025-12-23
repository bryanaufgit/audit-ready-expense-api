using AuditReadyExpense.Domain;
using AuditReadyExpense.Domain.Exceptions;

namespace AusitReadyExpense.Domain;

internal static class ExpenseStatusTransitions
{
    private static readonly Dictionary<ExpenseStatus, ExpenseStatus[]> AllowedTransitions = 
    new()
    {
        {ExpenseStatus.Draft, new[] { ExpenseStatus.Submitted } },
        {ExpenseStatus.Submitted, new[] { ExpenseStatus.Approved, ExpenseStatus.Rejected} },
        {ExpenseStatus.Approved, Array.Empty<ExpenseStatus>() },
        {ExpenseStatus.Rejected, Array.Empty<ExpenseStatus>() }
    };

    public static void EnsureTransitionAllowed(ExpenseStatus from, ExpenseStatus to)
    {
        if (!AllowedTransitions.TryGetValue(from, out var allowed) || !allowed.Contains(to))
        {
            throw new DomainException(DomainErrorCode.InvalidStateTransition, $"Transition from {from} ro {to} is not allowed.");
        }
    }
}