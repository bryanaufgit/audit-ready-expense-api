using AuditReadyExpense.Api.Contracts;
using AuditReadyExpense.Application.Contracts;
using AuditReadyExpense.Application.UseCases;
using AuditReadyExpense.Domain.Exceptions;
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
            // 400 because client didn't provide required data
            throw new ArgumentException("Missing or invalid X-Actor-UserId header (must be a GUID).");
        }

        return actorId;
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateDraft([FromBody] CreateExpenseRequest request, CancellationToken ct)
    {
        try
        {
            var actorId = GetActorUserId();

            var expense = await _service.CreateDraftAsync(
                createdByUserId: actorId,
                title: request.Title,
                amount: request.Amount,
                ct: ct);

            return CreatedAtAction(
                nameof(GetById),
                new { id = expense.Id },
                ToResponse(expense));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { code = ex.ErrorCode.ToString(), message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "BadRequest", message = ex.Message });
        }
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
        try
        {
            var actorId = GetActorUserId();

            await _service.SubmitAsync(id, actorId, ct);

            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { code = ex.ErrorCode.ToString(), message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = "BadRequest", message = ex.Message });
        }
        catch (InvalidOperationException)
        {
            // thrown when Expense not found (we will replace later with typed exception)
            return NotFound();
        }
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