namespace Aerarium.Application.Transactions.List;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Aerarium.Domain.Enums;
using Mediator;

public sealed record ListTransactionsQuery(
    int? Month,
    int? Year,
    TransactionType? Type,
    int Page,
    int PageSize) : IQuery<Result<PagedResult<TransactionDto>>>;

