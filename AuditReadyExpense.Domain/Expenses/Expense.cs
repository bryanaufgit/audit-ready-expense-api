using AuditReadyExpense.Domain.Exceptions;
using AusitReadyExpense.Domain;

namespace AuditReadyExpense.Domain.Expenses;

public class Expense
{
    private readonly List<AuditEvent> _auditEvents = new();

    public Guid Id { get; }
    public Guid CreatedByUserId { get; }
    public string Title { get; private set; }
    public decimal Amount { get; private set; }
    public ExpenseStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public IReadOnlyCollection<AuditEvent> AuditEvents => _auditEvents.AsReadOnly();

    private Expense()
    {
        Title = string.Empty;
        CreatedAtUtc = DateTime.UtcNow;
    } // Für EF Core später

    public Expense(
        Guid id,
        Guid createdByUserId,
        string title,
        decimal amount
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException(
                DomainErrorCode.MissingRequiredField,
                "Title is required."
            );

        if (amount <= 0)
            throw new DomainException(
                DomainErrorCode.InvalidAmount,
                "Amount must be greater than zero."
            );

        Id = id;
        CreatedByUserId = createdByUserId;
        Title = title;
        Amount = amount;
        Status = ExpenseStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Submit(Guid actorUserId)
    {
        var from = Status;
        var to = ExpenseStatus.Submitted;

        ExpenseStatusTransitions.EnsureTransitionAllowed(from, to);

        Status = to;
        AddAuditEvent(actorUserId, from, to);
    }

    public void Approve(Guid actorUserId)
    {
        if (actorUserId == CreatedByUserId)
            throw new DomainException(
                DomainErrorCode.UnauthorizedAction,
                "Creator cannot approve their own expense."
            );

        var from = Status;
        var to = ExpenseStatus.Approved;

        ExpenseStatusTransitions.EnsureTransitionAllowed(from, to);

        Status = to;
        AddAuditEvent(actorUserId, from, to);
    }

    public void Reject(Guid actorUserId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                DomainErrorCode.MissingRequiredField,
                "Rejection reason is required."
            );

        var from = Status;
        var to = ExpenseStatus.Rejected;

        ExpenseStatusTransitions.EnsureTransitionAllowed(from, to);

        Status = to;
        AddAuditEvent(actorUserId, from, to, reason);
    }

    private void AddAuditEvent(
        Guid actorUserId,
        ExpenseStatus fromStatus,
        ExpenseStatus toStatus,
        string? reason = null
    )
    {
        _auditEvents.Add(new AuditEvent(
            Id,
            fromStatus,
            toStatus,
            actorUserId,
            DateTime.UtcNow,
            reason
        ));
    }
}