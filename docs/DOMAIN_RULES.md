# Domain Rules (source of truth)

## Core concept
The system manages Expenses in a controlled workflow:
Draft → Submitted → Approved/Rejected

## Roles
- Employee: creates and edits Draft expenses; can submit.
- Approver: can approve/reject Submitted expenses.
- Admin: optional (later), e.g. reporting or management endpoints.

## Entities (high level)
- Expense (aggregate root)
- AuditEvent (immutable record of important changes)

## Expense status
- Draft
- Submitted
- Approved
- Rejected

## Allowed transitions
- Draft -> Submitted (by Employee)
- Submitted -> Approved (by Approver)
- Submitted -> Rejected (by Approver)

Forbidden examples:
- Draft -> Approved
- Approved -> Submitted
- Rejected -> Approved
- Any transition by a user without the required role

## Invariants (must always be true)
- Amount must be > 0
- Currency must be set (later, optional)
- CreatedAt is set once and never changes
- Status changes are only done through domain methods (no direct property set)
- Every status change creates an AuditEvent

## Validation rules (initial)
- Title/Description is required (min length can be defined later)
- Receipt is required if Amount >= 100.00 (placeholder threshold, adjustable)
- Reject requires a non-empty reason

## Audit events (minimum fields)
- ExpenseId
- EventType (Submitted/Approved/Rejected/Updated?)
- FromStatus, ToStatus (if status change)
- ActorUserId
- OccurredAt (UTC)
- Metadata (optional, e.g. reason)

## Error strategy (domain)
- Domain methods should throw a domain-specific exception (e.g. DomainException)
- API layer maps domain errors to HTTP 400/409 and permission errors to 403
