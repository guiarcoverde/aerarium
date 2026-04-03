namespace Aerarium.UnitTests.Application.Transactions;

using Aerarium.Application.Common;
using Aerarium.Application.Transactions.Create;
using Aerarium.Domain.Entities;
using Aerarium.Domain.Enums;
using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;

public sealed class CreateTransactionHandlerTests
{
    private readonly IAppDbContext _dbContext = Substitute.For<IAppDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ICategoryLocalizer _categoryLocalizer = Substitute.For<ICategoryLocalizer>();
    private readonly CreateTransactionHandler _handler;

    public CreateTransactionHandlerTests()
    {
        _currentUser.UserId.Returns("user-123");
        _categoryLocalizer.GetDisplayName(Arg.Any<TransactionCategory>()).Returns(c => c.Arg<TransactionCategory>().ToString());

        var transactions = new List<Transaction>().BuildMockDbSet();
        _dbContext.Transactions.Returns(transactions);

        _handler = new CreateTransactionHandler(_dbContext, _currentUser, _categoryLocalizer);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsDto()
    {
        var command = new CreateTransactionCommand(
            150.50m, "Salary", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(150.50m);
        result.Value.Description.Should().Be("Salary");
        result.Value.Type.Should().Be(TransactionType.Income);
        result.Value.Category.Should().Be(TransactionCategory.Salary);

        _dbContext.Transactions.Received(1).Add(Arg.Any<Transaction>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidCategory_ReturnsFailure()
    {
        var command = new CreateTransactionCommand(
            100m, "Test", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Housing);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category");
    }
}
