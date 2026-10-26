using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Required configuration for hashing.
/// </summary>
public class HashOptions
{
    public required string Salt { get; set; }
}

/// <summary>
/// Validate the configured <see cref="HashOptions"/>.
/// </summary>
public sealed class HashOptionsValidator : IValidateOptions<HashOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(
        string? name,
        HashOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Salt))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(HashOptions.Salt)} must be configured.");
        }

        return ValidateOptionsResult.Success;
    }
}

/// <summary>
/// Hashes a string.
/// </summary>
public interface IComputeHash
{
    /// <summary>
    /// Hashes a supplied string.
    /// </summary>
    /// <param name="source">The unhashed string.</param>
    /// <returns>The hashed string.</returns>
    string Compute(string source);
}

/// <summary>
/// Hashes a string.
/// </summary>
public class ComputeHash : IComputeHash
{
    private readonly HashOptions _options;

    /// <inheritdoc/>
    public ComputeHash(IOptions<HashOptions> options)
    {
        _options = options.Value;
    }

    public string Compute(string source)
    {
        var value = $"{source}{_options.Salt}";

        var bytes = Encoding.UTF8.GetBytes(value);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}

