namespace Aerarium.Application.Transactions.Create;

using Aerarium.Application.Common;
using Aerarium.Domain.Common;
using Aerarium.Domain.Entities;
using Aerarium.Domain.Enums;
using Aerarium.Domain.ValueObjects;
using Mediator;

public sealed class CreateTransactionHandler(
    IAppDbContext dbContext,
    ICurrentUserService currentUser,
    ICategoryLocalizer categoryLocalizer,
    IBusinessDayCalendar businessDayCalendar) : ICommandHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async ValueTask<Result<TransactionDto>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Recurrence != Recurrence.None)
            return await HandleRecurring(command, cancellationToken);

        var result = Transaction.Create(
            currentUser.UserId,
            command.Amount,
            command.Description,
            command.Date,
            command.Type,
            command.Category,
            salarySchedule: command.SalarySchedule);

        if (result.IsFailure)
            return Result<TransactionDto>.Failure(result.Error!);

        var transaction = result.Value!;

        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(transaction);
    }

    private async ValueTask<Result<TransactionDto>> HandleRecurring(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = Transaction.CreateSeries(
            currentUser.UserId,
            command.Amount,
            command.Description,
            command.Date,
            command.Type,
            command.Category,
            command.Recurrence,
            command.RecurrenceEndDate,
            command.RecurrenceCount,
            command.SalarySchedule,
            businessDayCalendar);

        if (result.IsFailure)
            return Result<TransactionDto>.Failure(result.Error!);

        var transactions = result.Value!;

        dbContext.Transactions.AddRange(transactions);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(transactions[0]);
    }

    private TransactionDto ToDto(Transaction transaction)
    {
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
