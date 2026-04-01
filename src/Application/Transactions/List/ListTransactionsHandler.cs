namespace Aerarium.Application.Transactions.List;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Aerarium.Domain.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

public sealed class ListTransactionsHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ICategoryLocalizer categoryLocalizer) : IQueryHandler<ListTransactionsQuery, Result<PagedResult<TransactionDto>>>
{
    public async ValueTask<Result<PagedResult<TransactionDto>>> Handle(
        ListTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        var dbQuery = dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == currentUser.UserId);

        if (query.Month.HasValue)
            dbQuery = dbQuery.Where(t => t.Date.Month == query.Month.Value);

        if (query.Year.HasValue)
            dbQuery = dbQuery.Where(t => t.Date.Year == query.Year.Value);

        if (query.Type.HasValue)
            dbQuery = dbQuery.Where(t => t.Type == query.Type.Value);

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        var transactions = await dbQuery
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = transactions
            .Select(t => new TransactionDto(
                t.Id,
                t.Amount,
                t.Description,
                t.Date,
                t.Type,
                t.Category,
                categoryLocalizer.GetDisplayName(t.Category),
                t.CreatedAt,
                t.UpdatedAt))
            .ToList();

        return new PagedResult<TransactionDto>(items, totalCount, query.Page, query.PageSize);
    }
}

