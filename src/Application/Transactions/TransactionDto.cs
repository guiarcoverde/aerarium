namespace Aerarium.Application.Transactions;

using Aerarium.Domain.Enums;

public sealed record TransactionDto(
    Guid Id,
    decimal Amount,
    string Description,
    DateOnly Date,
    TransactionType Type,
    TransactionCategory Category,
    string CategoryDisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
