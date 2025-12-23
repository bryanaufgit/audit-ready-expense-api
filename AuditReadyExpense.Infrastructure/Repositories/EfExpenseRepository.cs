using AuditReadyExpense.Application.Contracts;
using AuditReadyExpense.Domain.Expenses;
using AuditReadyExpense.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditReadyExpense.Infrastructure.Repositories;

public class EfExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _db;

    public EfExpenseRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Expenses.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AddAsync(Expense expense, CancellationToken ct = default)
        => _db.Expenses.AddAsync(expense, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}