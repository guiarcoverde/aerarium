namespace Aerarium.Domain.Entities;

using Aerarium.Domain.Common;
using Aerarium.Domain.Enums;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public DateOnly Date { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Transaction() { }

    public static Result<Transaction> Create(
        string userId,
        decimal amount,
        string description,
        DateOnly date,
        TransactionType type,
        TransactionCategory category)
    {
        if (amount <= 0)
            return Result<Transaction>.Failure("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Transaction>.Failure("Description is required.");

        if (!IsValidCategoryForType(type, category))
            return Result<Transaction>.Failure("Category is not valid for the given transaction type.");

        return new Transaction
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Amount = amount,
            Description = description.Trim(),
            Date = date,
            Type = type,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result<Transaction> Update(
        decimal amount,
        string description,
        DateOnly date,
        TransactionType type,
        TransactionCategory category)
    {
        if (amount <= 0)
            return Result<Transaction>.Failure("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Transaction>.Failure("Description is required.");

        if (!IsValidCategoryForType(type, category))
            return Result<Transaction>.Failure("Category is not valid for the given transaction type.");

        Amount = amount;
        Description = description.Trim();
        Date = date;
        Type = type;
        Category = category;
        UpdatedAt = DateTime.UtcNow;

        return this;
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
