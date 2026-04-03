namespace Aerarium.IntegrationTests.Endpoints;

using System.Net;
using System.Net.Http.Json;
using Aerarium.Application.Common;
using Aerarium.Application.Transactions;
using Aerarium.Domain.Enums;
using Aerarium.IntegrationTests.Infrastructure;
using FluentAssertions;

public sealed class TransactionEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateTransaction_ValidRequest_Returns201()
    {
        var client = await factory.CreateAuthenticatedClientAsync();
        var request = new
        {
            Amount = 100.50m,
            Description = "Test income",
            Date = "2026-04-01",
            Type = TransactionType.Income,
            Category = TransactionCategory.Salary
        };

        var response = await client.PostAsJsonAsync("/api/transactions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<TransactionDto>();
        dto.Should().NotBeNull();
        dto!.Amount.Should().Be(100.50m);
        dto.Description.Should().Be("Test income");
    }

    [Fact]
    public async Task CreateTransaction_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();
        var request = new
        {
            Amount = 100m,
            Description = "Test",
            Date = "2026-04-01",
            Type = TransactionType.Income,
            Category = TransactionCategory.Salary
        };

        var response = await client.PostAsJsonAsync("/api/transactions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTransaction_OtherUsersTransaction_Returns404()
    {
        var client1 = await factory.CreateAuthenticatedClientAsync(userId: "user-1", email: "user1@test.com");
        var createRequest = new
        {
            Amount = 50m,
            Description = "User1 transaction",
            Date = "2026-04-01",
            Type = TransactionType.Expense,
            Category = TransactionCategory.Grocery
        };

        var createResponse = await client1.PostAsJsonAsync("/api/transactions", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TransactionDto>();

        var client2 = await factory.CreateAuthenticatedClientAsync(userId: "user-2", email: "user2@test.com");
        var response = await client2.GetAsync($"/api/transactions/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListTransactions_FilterByMonth_ReturnsFiltered()
    {
        var client = await factory.CreateAuthenticatedClientAsync(userId: "list-user", email: "list@test.com");

        await client.PostAsJsonAsync("/api/transactions", new
        {
            Amount = 100m, Description = "April", Date = "2026-04-15",
            Type = TransactionType.Income, Category = TransactionCategory.Salary
        });

        await client.PostAsJsonAsync("/api/transactions", new
        {
            Amount = 200m, Description = "March", Date = "2026-03-15",
            Type = TransactionType.Income, Category = TransactionCategory.Bonus
        });

        var response = await client.GetAsync("/api/transactions?month=4&year=2026&page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TransactionDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().AllSatisfy(t => t.Date.Month.Should().Be(4));
    }

    [Fact]
    public async Task DeleteTransaction_Existing_Returns204()
    {
        var client = await factory.CreateAuthenticatedClientAsync(userId: "delete-user", email: "delete@test.com");
        var createResponse = await client.PostAsJsonAsync("/api/transactions", new
        {
            Amount = 50m, Description = "To delete", Date = "2026-04-01",
            Type = TransactionType.Expense, Category = TransactionCategory.Housing
        });

        var created = await createResponse.Content.ReadFromJsonAsync<TransactionDto>();

        var response = await client.DeleteAsync($"/api/transactions/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
