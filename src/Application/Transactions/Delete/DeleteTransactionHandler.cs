namespace Aerarium.Application.Transactions.Delete;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteTransactionHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser) : ICommandHandler<DeleteTransactionCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(
                t => t.Id == command.Id && t.UserId == currentUser.UserId,
                cancellationToken);

        if (transaction is null)
            return Result<bool>.Failure("Transaction not found.");

        dbContext.Transactions.Remove(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
