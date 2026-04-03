namespace Aerarium.UnitTests.Application.Transactions;

using Aerarium.Application.Transactions.Create;
using Aerarium.Domain.Enums;
using FluentAssertions;
using FluentValidation.TestHelper;

public sealed class CreateTransactionValidatorTests
{
    private readonly CreateTransactionValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateTransactionCommand(
            100m, "Salary payment", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ZeroAmount_HasError()
    {
        var command = new CreateTransactionCommand(
            0, "Test", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var command = new CreateTransactionCommand(
            100m, "", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_InvalidCategoryForType_HasError()
    {
        var command = new CreateTransactionCommand(
            100m, "Test", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Housing);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Validate_RecurringWithoutEndRule_HasError()
    {
        var command = new CreateTransactionCommand(
            100m, "Test", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary,
            Recurrence.Monthly);

        var result = _validator.TestValidate(command);

        result.Errors.Should().Contain(e =>
            e.ErrorMessage == "Recurring transactions must have either an end date or an occurrence count.");
    }

    [Fact]
    public void Validate_NonRecurringWithEndDate_HasError()
    {
        var command = new CreateTransactionCommand(
            100m, "Test", new DateOnly(2026, 4, 1),
            TransactionType.Income, TransactionCategory.Salary,
            Recurrence.None, RecurrenceEndDate: new DateOnly(2026, 12, 31));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RecurrenceEndDate);
    }
}
