using FluentResults;
using FluentValidation;
using InventoryService.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Infrastructure;

public static class RequestHandler
{
    /// <summary>
    /// Request handler delegate for handling http requests.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="input">An instance of the request.</param>
    /// <param name="IRequestHandler">A <see cref="IRequestHandler"/>, injected by DI.</param>
    /// <param name="requestValidator">An optional <see cref="IValidator{TRequest}"/>, injected by DI.</param>
    /// <param name="dataIntegrityEvaluator">An optional <see cref="IDataIntegrityEvaluator{TRequest}"/>, injected by DI.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The handler's result, encapsualted by a task.</returns>
    public static async Task<IResult> HandleAsync<TRequest, TResponse>(
        [AsParameters] TRequest input,
        [FromServices] IRequestHandler<TRequest, Result<TResponse>> hanlder,
        [FromServices] IValidator<TRequest>? requestValidator,
        [FromServices] IDataIntegrityEvaluator<TRequest>? dataIntegrityEvaluator,
        [FromServices] IValidationProblemWriter validationProblemWriter,
        CancellationToken cancellationToken)
        where TRequest : IRequest<Result<TResponse>>
    {
        if (requestValidator is not null)
        {
            var validationResult = await requestValidator.ValidateAsync(input, cancellationToken);

            if (!validationResult.IsValid)
            {
                return validationProblemWriter.Write(validationResult);
            }
        }

        if (dataIntegrityEvaluator is not null)
        {
            var evaluationResult = await dataIntegrityEvaluator.EvaluateAsync(input, cancellationToken);

            if (!evaluationResult.IsValid)
            {
                return validationProblemWriter.Write(evaluationResult);
            }
        }

        var result = await hanlder.Handle(input, cancellationToken);

        if (result.IsSuccess)
        {
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
