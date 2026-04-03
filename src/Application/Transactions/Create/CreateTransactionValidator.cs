namespace Aerarium.Application.Transactions.Create;

using Aerarium.Domain.Enums;
using FluentValidation;

public sealed class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.Category)
            .Must((cmd, category) => IsValidCategoryForType(cmd.Type, category))
            .WithMessage("Category is not valid for the given transaction type.");

        RuleFor(x => x.Recurrence)
            .IsInEnum();

        When(x => x.Recurrence != Recurrence.None, () =>
        {
            RuleFor(x => x)
                .Must(x => x.RecurrenceEndDate is not null || x.RecurrenceCount is not null)
                .WithMessage("Recurring transactions must have either an end date or an occurrence count.");
        });

        When(x => x.Recurrence == Recurrence.None, () =>
        {
            RuleFor(x => x.RecurrenceEndDate).Null()
                .WithMessage("Non-recurring transactions cannot have an end date.");
            RuleFor(x => x.RecurrenceCount).Null()
                .WithMessage("Non-recurring transactions cannot have an occurrence count.");
        });

        RuleFor(x => x.RecurrenceCount)
            .GreaterThan(0)
            .When(x => x.RecurrenceCount is not null);
    }

    private static bool IsValidCategoryForType(TransactionType type, TransactionCategory category)
    {
        return type switch
        {
            TransactionType.Income => (int)category is >= 100 and < 200,
            TransactionType.Expense => (int)category is >= 200 and < 300,
            _ => false
        };
    }
}
