namespace Aerarium.Application.Transactions.Create;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Aerarium.Domain.Entities;
using Mediator;

public sealed class CreateTransactionHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ICategoryLocalizer categoryLocalizer) : ICommandHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async ValueTask<Result<TransactionDto>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = Transaction.Create(
            currentUser.UserId,
            command.Amount,
            command.Description,
            command.Date,
            command.Type,
            command.Category);

        if (result.IsFailure)
            return Result<TransactionDto>.Failure(result.Error!);

        var transaction = result.Value!;

        dbContext.Transactions.Add(transaction);
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
