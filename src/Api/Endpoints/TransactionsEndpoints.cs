namespace Aerarium.Api.Endpoints;

using Aerarium.Api.Contracts;
using Aerarium.Application.Transactions;
using Aerarium.Application.Transactions.Create;
using Aerarium.Application.Transactions.Delete;
using Aerarium.Application.Transactions.GetById;
using Aerarium.Application.Transactions.List;
using Aerarium.Application.Transactions.Update;
using Aerarium.Domain.Enums;
using Mediator;

public static class TransactionsEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transactions")
            .RequireAuthorization();

        group.MapPost("/", async (CreateTransactionRequest request, IMediator mediator) =>
        {
            var command = new CreateTransactionCommand(
                request.Amount,
                request.Description,
                request.Date,
                request.Type,
                request.Category);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/api/transactions/{result.Value!.Id}", result.Value)
                : Results.Problem(title: result.Error, statusCode: StatusCodes.Status400BadRequest);
        });

        group.MapGet("/", async (
            int? month,
            int? year,
            TransactionType? type,
            int page,
            int pageSize,
            IMediator mediator) =>
        {
            var query = new ListTransactionsQuery(month, year, type, page, pageSize);
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(title: result.Error, statusCode: StatusCodes.Status400BadRequest);
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTransactionQuery(id));

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(title: result.Error, statusCode: StatusCodes.Status404NotFound);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateTransactionRequest request, IMediator mediator) =>
        {
            var command = new UpdateTransactionCommand(
                id,
                request.Amount,
                request.Description,
                request.Date,
                request.Type,
                request.Category);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(title: result.Error, statusCode: StatusCodes.Status400BadRequest);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteTransactionCommand(id));

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(title: result.Error, statusCode: StatusCodes.Status404NotFound);
        });
    }
}
