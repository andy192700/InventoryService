using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Infrastructure;

public static class HttpHandler
{
    public static async Task<IResult> HandleAsync<TInput, TResponse>(
        [AsParameters] TInput input,
        [FromServices] IServiceProvider serviceProvider,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
        where TInput : IRequest<Result<TResponse>>
    {
        var validator = serviceProvider.GetService<IValidator<TInput>>();

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

        var result = await mediator.Send(input, cancellationToken);

        if (result.IsSuccess)
        {
            return result switch
            {
                IOk => Results.Ok(result.Value),
                ICreated created => Results.Created(
                    created.Location,
                    result.Value),
                _ => Results.Problem(statusCode: 500)
            };
        }

        return result.Errors.First() switch
        {
            NotFoundError e => Results.Problem(e.Message, statusCode: 404),
            InternalServerError e => Results.Problem(e.Message, statusCode: 500),
            _ => Results.Problem(statusCode: 500)
        };
    }
}
