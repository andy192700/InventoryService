using FluentValidation.Results;

namespace InventoryService.Infrastructure.Validation;

/// <summary>
/// Writes a <see cref="ValidationResult"/> to a <see cref="IResult"/> object.
/// </summary>
public interface IValidationProblemWriter
{
    /// <summary>
    /// Write the result.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <returns>The result to be articulated in a response.</returns>
    IResult Write(ValidationResult result);
}

/// <summary>
/// Writes a <see cref="ValidationResult"/> to a <see cref="IResult"/> object.
/// </summary>
public class ValidationProblemWriter : IValidationProblemWriter
{
    /// <inheritdoc/>
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
