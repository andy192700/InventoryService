using FluentValidation.Results;

namespace InventoryService.Infrastructure.Validation;

public interface IValidationProblemWriter
{
    IResult Write(ValidationResult result);
}

public class ValidationProblemWriter : IValidationProblemWriter
{
    public IResult Write(ValidationResult result)
    {
        var errorDictionary = new Dictionary<string, string[]>();

        foreach (var error in result.Errors)
        {
            errorDictionary.Add(error.PropertyName, [error.ErrorMessage]);
        }

        return Results.ValidationProblem(errorDictionary);
    }
}
