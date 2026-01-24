using System.Net;
using System.Net.Http.Json;
using AuditReadyExpense.Api.Contracts;
using FluentAssertions;
using Xunit;

namespace AuditReadyExpense.Tests.Integration;

public class ExpensesIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public ExpensesIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Submit_Approve_WritesAuditAndPersistsState()
    {
        // Arrange
        var client = _factory.CreateClient();

        var creator = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var approver = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Create
        client.DefaultRequestHeaders.Remove("X-Actor-UserId");
        client.DefaultRequestHeaders.Add("X-Actor-UserId", creator.ToString());

        var createRes = await client.PostAsJsonAsync("/api/expenses", new CreateExpenseRequest
        {
            Title = "Taxi",
            Amount = 12.50m
        });

        createRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createRes.Content.ReadFromJsonAsync<ExpenseResponse>();
        created.Should().NotBeNull();
        created!.Status.ToString().Should().Be("Draft");

        // Submit
        var submitRes = await client.PostAsync($"/api/expenses/{created.Id}/submit", null);
        submitRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Approve (different actor)
        client.DefaultRequestHeaders.Remove("X-Actor-UserId");
        client.DefaultRequestHeaders.Add("X-Actor-UserId", approver.ToString());

        var approveRes = await client.PostAsync($"/api/expenses/{created.Id}/approve", null);
        approveRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify state
        var getRes = await client.GetAsync($"/api/expenses/{created.Id}");
        getRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var after = await getRes.Content.ReadFromJsonAsync<ExpenseResponse>();
        after!.Status.ToString().Should().Be("Approved");

        // Verify audit
        var auditRes = await client.GetAsync($"/api/expenses/{created.Id}/audit");
        auditRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var audit = await auditRes.Content.ReadFromJsonAsync<List<AuditEventResponse>>();
        audit.Should().NotBeNull();
        audit!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task Creator_CannotApprove_OwnExpense()
    {
        var client = _factory.CreateClient();
        var creator = Guid.Parse("11111111-1111-1111-1111-111111111111");

        client.DefaultRequestHeaders.Add("X-Actor-UserId", creator.ToString());

        var createRes = await client.PostAsJsonAsync("/api/expenses", new CreateExpenseRequest
        {
            Title = "Hotel",
            Amount = 199m
        });

        var created = await createRes.Content.ReadFromJsonAsync<ExpenseResponse>();

        await client.PostAsync($"/api/expenses/{created!.Id}/submit", null);

        var approveRes = await client.PostAsync($"/api/expenses/{created.Id}/approve", null);
        approveRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}