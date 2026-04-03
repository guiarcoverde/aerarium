namespace Aerarium.UnitTests.Domain.Entities;

using Aerarium.Domain.Entities;
using Aerarium.Domain.Enums;
using FluentAssertions;

public sealed class TransactionTests
{
    private const string ValidUserId = "user-123";
    private const decimal ValidAmount = 150.50m;
    private const string ValidDescription = "Test transaction";
    private static readonly DateOnly ValidDate = new(2026, 4, 1);

    [Fact]
    public void Create_ValidInputs_ReturnsSuccess()
    {
        var result = Transaction.Create(
            ValidUserId, ValidAmount, ValidDescription, ValidDate,
            TransactionType.Income, TransactionCategory.Salary);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(ValidAmount);
        result.Value.Description.Should().Be(ValidDescription);
        result.Value.Date.Should().Be(ValidDate);
        result.Value.Type.Should().Be(TransactionType.Income);
        result.Value.Category.Should().Be(TransactionCategory.Salary);
        result.Value.UserId.Should().Be(ValidUserId);
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Value.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_ZeroAmount_ReturnsFailure()
    {
        var result = Transaction.Create(
            ValidUserId, 0, ValidDescription, ValidDate,
            TransactionType.Income, TransactionCategory.Salary);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Amount");
    }

    [Fact]
    public void Create_NegativeAmount_ReturnsFailure()
    {
        var result = Transaction.Create(
            ValidUserId, -10m, ValidDescription, ValidDate,
            TransactionType.Income, TransactionCategory.Salary);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Amount");
    }

    [Fact]
    public void Create_EmptyDescription_ReturnsFailure()
    {
        var result = Transaction.Create(
            ValidUserId, ValidAmount, "", ValidDate,
            TransactionType.Income, TransactionCategory.Salary);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Description");
    }

    [Fact]
    public void Create_IncomeCategoryWithExpenseType_ReturnsFailure()
    {
        var result = Transaction.Create(
            ValidUserId, ValidAmount, ValidDescription, ValidDate,
            TransactionType.Expense, TransactionCategory.Salary);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category");
    }

    [Fact]
    public void Create_ExpenseCategoryWithIncomeType_ReturnsFailure()
    {
        var result = Transaction.Create(
            ValidUserId, ValidAmount, ValidDescription, ValidDate,
            TransactionType.Income, TransactionCategory.Housing);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category");
    }

    [Fact]
    public void Update_ValidInputs_UpdatesPropertiesAndSetsUpdatedAt()
    {
        var transaction = Transaction.Create(
            ValidUserId, ValidAmount, ValidDescription, ValidDate,
            TransactionType.Income, TransactionCategory.Salary).Value!;

        var newDate = new DateOnly(2026, 5, 1);
        var result = transaction.Update(
            200m, "Updated description", newDate,
            TransactionType.Expense, TransactionCategory.Housing);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(200m);
        result.Value.Description.Should().Be("Updated description");
        result.Value.Date.Should().Be(newDate);
        result.Value.Type.Should().Be(TransactionType.Expense);
        result.Value.Category.Should().Be(TransactionCategory.Housing);
        result.Value.UpdatedAt.Should().NotBeNull();
        result.Value.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
