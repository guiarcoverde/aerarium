namespace Aerarium.Application.Transactions.Update;

using Aerarium.Domain.Common;
using Aerarium.Domain.Enums;
using Mediator;

public sealed record UpdateTransactionCommand(
    Guid Id,
    decimal Amount,
    string Description,
    DateOnly Date,
    TransactionType Type,
    TransactionCategory Category) : ICommand<Result<TransactionDto>>;
