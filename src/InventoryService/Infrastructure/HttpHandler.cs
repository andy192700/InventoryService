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
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/>, injected by DI.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns></returns>
    public static async Task<IResult> HandleAsync<TRequest, TResponse>(
        [AsParameters] TRequest input,
        [FromServices] IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
        where TRequest : IRequest<Result<TResponse>>
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(input, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorDictionary = new Dictionary<string, string[]>();

                foreach (var error in validationResult.Errors)
                {
                    errorDictionary.Add(error.PropertyName, [error.ErrorMessage]);
                }

                return Results.ValidationProblem(errorDictionary);
            }
        }

        var evaluator = serviceProvider.GetService<IDataIntegrityEvaluator<TRequest>>();

        if (evaluator is not null)
        {
            var evaluationResult = await evaluator.EvaluateAsync(input, cancellationToken);

            if (!evaluationResult.IsValid)
            {
                var errorDictionary = new Dictionary<string, string[]>();

                foreach (var error in evaluationResult.Errors)
                {
                    errorDictionary.Add(error.PropertyName, [error.ErrorMessage]);
                }

                return Results.ValidationProblem(errorDictionary);
            }
        }

        var handler = serviceProvider.GetService<IRequestHandler<TRequest, Result<TResponse>>>();

        if (handler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        var result = await handler!.Handle(input, cancellationToken);

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
            _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
