using AuditReadyExpense.Domain.Expenses;

namespace AuditReadyExpense.Application.Contracts;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Expense expense, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}