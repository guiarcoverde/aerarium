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
            command.Category,
            command.Recurrence,
            command.RecurrenceEndDate,
            command.RecurrenceCount,
            command.SalarySchedule);

        if (result.IsFailure)
            return Result<TransactionDto>.Failure(result.Error!);

        await dbContext.SaveChangesAsync(cancellationToken);

        var salaryDto = transaction.SalarySchedule is not null
            ? new SalaryScheduleDto(
                transaction.SalarySchedule.Mode, transaction.SalarySchedule.BusinessDayNumber,
                transaction.SalarySchedule.FixedDay, transaction.SalarySchedule.SplitFirstAmount,
                transaction.SalarySchedule.SplitFirstPercentage)
            : null;

        return new TransactionDto(
            transaction.Id,
            transaction.Amount,
            transaction.Description,
            transaction.Date,
            transaction.Type,
            transaction.Category,
            categoryLocalizer.GetDisplayName(transaction.Category),
            transaction.Recurrence,
            transaction.RecurrenceGroupId,
            transaction.RecurrenceEndDate,
            transaction.RecurrenceCount,
            salaryDto,
            transaction.CreatedAt,
            transaction.UpdatedAt);
    }
}
