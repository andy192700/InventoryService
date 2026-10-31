using Dapper;
using FluentValidation.Results;
using InventoryService.Extensions;

namespace InventoryService.Infrastructure.Validation;

/// <summary>
/// Validates specific data requirements for a given request by querying the database.
/// </summary>
/// <typeparam name="TRequest">The Incoming request object.</typeparam>
public abstract class DataIntegrityValidator<TRequest>
{
    /// <summary>
    /// The reason why the request is invalid, returned in the response.
    /// </summary>
    protected abstract string ErrorMessage { get; }

    /// <summary>
    /// The stored procedure to be executed to determine whether the incoming data is valid.
    /// </summary>
    protected abstract string StoredProcedureName { get; }

    /// <summary>
    /// An array of affected properties, detailed in the response should the request be invalid.
    /// </summary>
    protected abstract string[] ApplicableProperties { get; }

    private readonly ISQLActivity _activity;

    public DataIntegrityValidator(ISQLActivity activity)
    {
        _activity = activity;
    }

    /// <summary>
    /// Validate the request using the database call.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The collection of validation failures.</returns>
    public async Task<ICollection<ValidationFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        await _activity.ConnectAndEstablishAsync(
            StoredProcedureName,
            cancellationToken,
            CreateParameters(request));

        var result = await _activity.CommenceAsync(async (cmd, connection) =>
        {
            return await connection.ExecuteScalarAsync<bool>(cmd);
        });

        if (result)
        {
            return [];
        }

        var failures = new List<ValidationFailure>();

        foreach (var property in ApplicableProperties)
        {
            failures.Add(new ValidationFailure(property, ErrorMessage));
        }

        return failures;
    }

    /// <summary>
    /// Create the anonymous parameters object used to invoke the database stored procedure.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <returns>The parameters object.</returns>
    protected abstract object CreateParameters(TRequest request);
}

/// <summary>
/// Evaluates whether a request is valid or not.
/// </summary>
/// <typeparam name="TRequest">The incoming request object.</typeparam>
public interface IDataIntegrityEvaluator<TRequest>
{
    Task<ValidationResult> EvaluateAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Evaluates whether a request is valid or not.
/// </summary>
/// <typeparam name="TRequest">The incoming request object.</typeparam>
public sealed class DataIntegrityEvaluator<TRequest> : IDataIntegrityEvaluator<TRequest>
{
    private readonly IEnumerable<DataIntegrityValidator<TRequest>> _validators;

    public DataIntegrityEvaluator(IEnumerable<DataIntegrityValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> EvaluateAsync(TRequest request, CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            failures.AddRange(validationResult);
        }

        return new ValidationResult { Errors = failures };
    }

}