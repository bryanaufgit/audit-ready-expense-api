using AuditReadyExpense.Application.Contracts;
using AuditReadyExpense.Domain.Expenses;

namespace AuditReadyExpense.Application.UseCases;

public class ExpenseWorkflowService
{
    private readonly IExpenseRepository _repo;

    public ExpenseWorkflowService(IExpenseRepository repo)
    {
        _repo = repo;
    }

    public async Task<Expense> CreateDraftAsync(
        Guid createdByUserId,
        string title,
        decimal amount,
        CancellationToken ct = default)
    {
        var expense = new Expense(
            id: Guid.NewGuid(),
            createdByUserId: createdByUserId,
            title: title,
            amount: amount);

        await _repo.AddAsync(expense, ct);
        await _repo.SaveChangesAsync(ct);

        return expense;
    }

    public async Task SubmitAsync(Guid expenseId, Guid actorUserId, CancellationToken ct = default)
    {
        var expense = await _repo.GetByIdAsync(expenseId, ct)
            ?? throw new InvalidOperationException($"Expense {expenseId} not found.");

        expense.Submit(actorUserId);

        await _repo.SaveChangesAsync(ct);
    }

    public async Task ApproveAsync(Guid expenseId, Guid actorUserId, CancellationToken ct = default)
    {
        var expense = await _repo.GetByIdAsync(expenseId, ct)
            ?? throw new InvalidOperationException($"Expense {expenseId} not found.");

        expense.Approve(actorUserId);

        await _repo.SaveChangesAsync(ct);
    }

    public async Task RejectAsync(Guid expenseId, Guid actorUserId, string reason, CancellationToken ct = default)
    {
        var expense = await _repo.GetByIdAsync(expenseId, ct)
            ?? throw new InvalidOperationException($"Expense {expenseId} not found.");

        expense.Reject(actorUserId, reason);

        await _repo.SaveChangesAsync(ct);
    }
}