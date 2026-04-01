namespace Aerarium.Application.Transactions.Update;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Mediator;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateTransactionHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ICategoryLocalizer categoryLocalizer) : ICommandHandler<UpdateTransactionCommand, Result<TransactionDto>>
{
    public async ValueTask<Result<TransactionDto>> Handle(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(
                t => t.Id == command.Id && t.UserId == currentUser.UserId,
                cancellationToken);

        if (transaction is null)
            return Result<TransactionDto>.Failure("Transaction not found.");

        var result = transaction.Update(
            command.Amount,
            command.Description,
            command.Date,
            command.Type,
            command.Category);

        if (result.IsFailure)
            return Result<TransactionDto>.Failure(result.Error!);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new TransactionDto(
            transaction.Id,
            transaction.Amount,
            transaction.Description,
            transaction.Date,
            transaction.Type,
            transaction.Category,
            categoryLocalizer.GetDisplayName(transaction.Category),
            transaction.CreatedAt,
            transaction.UpdatedAt);
    }
}
