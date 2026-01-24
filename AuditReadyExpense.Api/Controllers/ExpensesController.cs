using AuditReadyExpense.Api.Contracts;
using AuditReadyExpense.Application.Contracts;
using AuditReadyExpense.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AuditReadyExpense.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseWorkflowService _service;
    private readonly IExpenseRepository _repo;

    public ExpensesController(ExpenseWorkflowService service, IExpenseRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    // TEMP until Auth: Actor is passed via header
    private Guid GetActorUserId()
    {
        if (!Request.Headers.TryGetValue("X-Actor-UserId", out var raw) ||
            !Guid.TryParse(raw.ToString(), out var actorId))
        {
            throw new ArgumentException("Missing or invalid X-Actor-UserId header (must be a GUID).");
        }

        return actorId;
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateDraft([FromBody] CreateExpenseRequest request, CancellationToken ct)
    {
        var actorId = GetActorUserId();

        var expense = await _service.CreateDraftAsync(
            createdByUserId: actorId,
            title: request.Title,
            amount: request.Amount,
            ct: ct);

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, ToResponse(expense));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var expense = await _repo.GetByIdAsync(id, ct);
        if (expense is null) return NotFound();

        return Ok(ToResponse(expense));
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct)
    {
        var actorId = GetActorUserId();

        await _service.SubmitAsync(id, actorId, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve([FromRoute] Guid id, CancellationToken ct)
    {
        var actorId = GetActorUserId();

        await _service.ApproveAsync(id, actorId, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectExpenseRequest request, CancellationToken ct)
    {
        var actorId = GetActorUserId();

        await _service.RejectAsync(id, actorId, request.Reason, ct);

        return NoContent();
    }
    [HttpGet("{id:guid}/audit")]
    public async Task<ActionResult<IReadOnlyList<AuditEventResponse>>> GetAuditTrail([FromRoute] Guid id, CancellationToken ct)
    {
        var expense = await _repo.GetByIdAsync(id, ct);
        if (expense is null) return NotFound();

        var events = expense.AuditEvents
            .OrderBy(e => e.OccurredAtUtc)
            .Select(e => new AuditEventResponse
            {
                FromStatus = e.FromStatus,
                ToStatus = e.ToStatus,
                ActorUserId = e.ActorUserId,
                OccurredAtUtc = e.OccurredAtUtc,
                Reason = e.Reason
            })
            .ToList();

        return Ok(events);
    }

    private static ExpenseResponse ToResponse(AuditReadyExpense.Domain.Expenses.Expense expense)
        => new()
        {
            Id = expense.Id,
            CreatedByUserId = expense.CreatedByUserId,
            Title = expense.Title,
            Amount = expense.Amount,
            Status = expense.Status,
            CreatedAtUtc = expense.CreatedAtUtc
        };
}