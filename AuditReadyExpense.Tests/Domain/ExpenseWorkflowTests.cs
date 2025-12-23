using AuditReadyExpense.Domain;
using AuditReadyExpense.Domain.Exceptions;
using AuditReadyExpense.Domain.Expenses;

namespace AuditReadyExpense.Tests.Domain;

public class ExpenseWorkflowTests
{
    private static Expense CreateDraft(
        Guid? createdBy = null,
        decimal amount = 10m,
        string title = "Taxi"
    )
    {
        return new Expense(
            id: Guid.NewGuid(),
            createdByUserId: createdBy ?? Guid.NewGuid(),
            title: title,
            amount: amount
        );
    }

    [Fact]
    public void Submit_from_Draft_sets_status_to_Submitted_and_creates_audit_event()
    {
        var creator = Guid.NewGuid();
        var actor = creator;
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(actor);

        Assert.Equal(ExpenseStatus.Submitted, expense.Status);
        var ev = Assert.Single(expense.AuditEvents);
        Assert.Equal(ExpenseStatus.Draft, ev.FromStatus);
        Assert.Equal(ExpenseStatus.Submitted, ev.ToStatus);
        Assert.Equal(actor, ev.ActorUserId);
        Assert.Equal(expense.Id, ev.ExpenseId);
    }

    [Fact]
    public void Approve_from_Submitted_sets_status_to_Approved_and_creates_audit_event()
    {
        var creator = Guid.NewGuid();
        var approver = Guid.NewGuid();
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(creator);
        expense.Approve(approver);

        Assert.Equal(ExpenseStatus.Approved, expense.Status);

        Assert.Equal(2, expense.AuditEvents.Count);
        var last = expense.AuditEvents.Last();

        Assert.Equal(ExpenseStatus.Submitted, last.FromStatus);
        Assert.Equal(ExpenseStatus.Approved, last.ToStatus);
        Assert.Equal(approver, last.ActorUserId);
    }

    [Fact]
    public void Reject_from_Submitted_sets_status_to_Rejected_and_creates_audit_event_with_reason()
    {
        var creator = Guid.NewGuid();
        var approver = Guid.NewGuid();
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(creator);
        expense.Reject(approver, "Receipt missing");

        Assert.Equal(ExpenseStatus.Rejected, expense.Status);

        var last = expense.AuditEvents.Last();
        Assert.Equal(ExpenseStatus.Submitted, last.FromStatus);
        Assert.Equal(ExpenseStatus.Rejected, last.ToStatus);
        Assert.Equal("Receipt missing", last.Reason);
    }

    [Fact]
    public void Approve_in_Draft_throws_InvalidStateTransition()
    {
        var expense = CreateDraft();

        var ex = Assert.Throws<DomainException>(() => expense.Approve(Guid.NewGuid()));

        Assert.Equal(DomainErrorCode.InvalidStateTransition, ex.ErrorCode);
    }

    [Fact]
    public void Creator_cannot_approve_own_expense()
    {
        var creator = Guid.NewGuid();
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(creator);

        var ex = Assert.Throws<DomainException>(() => expense.Approve(creator));
        Assert.Equal(DomainErrorCode.UnauthorizedAction, ex.ErrorCode);
    }

    [Fact]
    public void Reject_requires_reason()
    {
        var creator = Guid.NewGuid();
        var approver = Guid.NewGuid();
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(creator);

        var ex = Assert.Throws<DomainException>(() => expense.Reject(approver, "   "));
        Assert.Equal(DomainErrorCode.MissingRequiredField, ex.ErrorCode);
    }

    [Fact]
    public void Audit_event_from_status_is_previous_status_not_new_status()
    {
        var creator = Guid.NewGuid();
        var expense = CreateDraft(createdBy: creator);

        expense.Submit(creator);

        var ev = expense.AuditEvents.Single();
        Assert.Equal(ExpenseStatus.Draft, ev.FromStatus);
        Assert.Equal(ExpenseStatus.Submitted, ev.ToStatus);
    }
}