namespace Aerarium.Application.Transactions.Create;

using Aerarium.Domain.Common;
using Aerarium.Domain.Enums;
using Mediator;

public sealed record CreateTransactionCommand(
    decimal Amount,
    string Description,
    DateOnly Date,
    TransactionType Type,
    TransactionCategory Category) : ICommand<Result<TransactionDto>>;
