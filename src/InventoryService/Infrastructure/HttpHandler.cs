using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Infrastructure;

public static class HttpHandler
{
    /// <summary>
    /// Request handler delegate for handling http requests, not that a <see cref="IRequestHandler"/> of the
    /// required generic types must exist, this is picked up via assembly scan.
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

        var handler = serviceProvider.GetService<IRequestHandler<TRequest, Result<TResponse>>>();

        if (handler is null)
        {
            Results.Problem(statusCode: StatusCodes.Status404NotFound);
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
                _ => Results.Problem(statusCode: StatusCodes.Status200OK)
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
