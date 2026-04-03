namespace Aerarium.Application.Transactions.GetById;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Aerarium.Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

public sealed class GetTransactionHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ICategoryLocalizer categoryLocalizer) : IQueryHandler<GetTransactionQuery, Result<TransactionDto>>
{
    public async ValueTask<Result<TransactionDto>> Handle(
        GetTransactionQuery query,
        CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.Id == query.Id && t.UserId == currentUser.UserId,
                cancellationToken);

        if (transaction is null)
            return Result<TransactionDto>.Failure("Transaction not found.");

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
            MapSalarySchedule(transaction.SalarySchedule),
            transaction.CreatedAt,
            transaction.UpdatedAt);
    }

    private static SalaryScheduleDto? MapSalarySchedule(SalarySchedule? schedule)
    {
        if (schedule is null) return null;
        return new SalaryScheduleDto(
            schedule.Mode, schedule.BusinessDayNumber, schedule.FixedDay,
            schedule.SplitFirstAmount, schedule.SplitFirstPercentage);
    }
}
