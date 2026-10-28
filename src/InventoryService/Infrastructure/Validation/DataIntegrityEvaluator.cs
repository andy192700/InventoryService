using Dapper;
using FluentValidation.Results;
using System.Collections;
using System.Data;

namespace InventoryService.Infrastructure.Validation;

public interface IDataIntegrityValidator<TRequest>
{
    Task<ICollection<ValidationFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken);
}

public abstract class DataIntegrityValidator<TRequest> : IDataIntegrityValidator<TRequest>
{
    protected abstract string ErrorMessage { get; }

    protected abstract string StoredProcedureName { get; }

    protected abstract string[] ApplicableProperties { get; }

    private readonly ISQLConnectionProvider _connectionProvider;

    public DataIntegrityValidator(ISQLConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<ICollection<ValidationFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        using var conn = await _connectionProvider.GetSqlConnectionAsync("Inventory", cancellationToken);

        var result = await conn.ExecuteScalarAsync<bool>(
            StoredProcedureName,
            param: CreateParameters(request),
            commandType: CommandType.StoredProcedure);

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

    protected abstract object CreateParameters(TRequest request);
}

public interface IDataIntegrityEvaluator<TRequest>
{
    Task<ValidationResult> EvaluateAsync(TRequest request, CancellationToken cancellationToken);
}

public sealed class DataIntegrityEvaluator<TRequest> : IDataIntegrityEvaluator<TRequest>
{
    private readonly IEnumerable<DataIntegrityValidator<TRequest>> _validators;

    public DataIntegrityEvaluator(IEnumerable<DataIntegrityValidator<TRequest>> validators)
    {
        _validators = validators;
    }

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