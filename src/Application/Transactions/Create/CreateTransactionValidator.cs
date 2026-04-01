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
