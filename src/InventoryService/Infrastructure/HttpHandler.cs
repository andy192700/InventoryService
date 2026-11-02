using FluentResults;
using FluentValidation;
using InventoryService.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Infrastructure;

public static class HttpHandler
{
    /// <summary>
    /// Request handler delegate for handling http requests.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="input">An instance of the request.</param>
    /// <param name="mediator">A <see cref="IMediator"/>, injected by DI.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/>, injected by DI.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The handler's result, encapsualted by a task.</returns>
    public static async Task<IResult> HandleAsync<TRequest, TResponse>(
        [AsParameters] TRequest input,
        [FromServices] IMediator mediator,
        [FromServices] IServiceProvider serviceProvider,
        [FromServices] IValidationProblemWriter validationProblemWriter,
        CancellationToken cancellationToken)
        where TRequest : IRequest<Result<TResponse>>
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(input, cancellationToken);

            if (!validationResult.IsValid)
            {
                return validationProblemWriter.Write(validationResult);
            }
        }

        var evaluator = serviceProvider.GetService<IDataIntegrityEvaluator<TRequest>>();

        if (evaluator is not null)
        {
            var evaluationResult = await evaluator.EvaluateAsync(input, cancellationToken);

            if (!evaluationResult.IsValid)
            {
                return validationProblemWriter.Write(evaluationResult);
            }
        }

        var result = await mediator.Send(input, cancellationToken);

        if (result.IsSuccess)
        {
            if (result.Value is null)
            {
                return Results.Problem(statusCode: StatusCodes.Status404NotFound);
            }

            return result switch
            {
                IOk => Results.Ok(result.Value),
                ICreated created => Results.Created(
                    created.Location,
                    result.Value),
                _ => Results.Ok()
            };
        }

        return result.Errors.First() switch
        {
            NotFoundError e => Results.Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
            InternalServerError e => Results.Problem(e.Message, statusCode: StatusCodes.Status500InternalServerError),
            UnauthorizedError e => Results.Problem(statusCode: StatusCodes.Status401Unauthorized),
            _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
