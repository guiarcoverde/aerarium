namespace Aerarium.Api.Contracts;

using Aerarium.Domain.Enums;

public sealed record UpdateTransactionRequest(
    decimal Amount,
    string Description,
    DateOnly Date,
    TransactionType Type,
    TransactionCategory Category);
