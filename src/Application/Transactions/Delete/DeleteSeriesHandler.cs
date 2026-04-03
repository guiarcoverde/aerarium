namespace Aerarium.Application.Transactions.Delete;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteSeriesHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser) : ICommandHandler<DeleteSeriesCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteSeriesCommand command,
        CancellationToken cancellationToken)
    {
        var transactions = await dbContext.Transactions
            .Where(t => t.RecurrenceGroupId == command.RecurrenceGroupId
                        && t.UserId == currentUser.UserId)
            .ToListAsync(cancellationToken);

        if (transactions.Count == 0)
            return Result<bool>.Failure("Recurring series not found.");

        dbContext.Transactions.RemoveRange(transactions);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
