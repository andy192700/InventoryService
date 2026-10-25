using FluentResults;

namespace InventoryService.Infrastructure;

public static class HttpResponses
{
    /// <summary>
    /// Indicates the response status code is 200.
    /// </summary>
    /// <typeparam name="TResponse">The response payload type.</typeparam>
    /// <param name="value">The response object.</param>
    /// <returns>An Instance of <see cref="IOk"/></returns>
    public static Ok<TResponse> Ok<TResponse>(TResponse value) where TResponse : class
        => new(value);

    /// <summary>
    /// Indicates the response status code is 201.
    /// </summary>
    /// <typeparam name="TResponse">The response payload type.</typeparam>
    /// <param name="value">The response object.</param>
    /// <returns>An Instance of <see cref="ICreated"/></returns>
    public static Created<TResponse> Created<TResponse>(TResponse value, string location) where TResponse : class
        => new(value, location);

    /// <summary>
    /// Indicates that the response status code is 404.
    /// </summary>
    /// <param name="message">The message detailing the particulars.</param>
    /// <returns>A <see cref="Result"/> instance.</returns>
    public static Result Notfound(string message)
        => Result.Fail(new NotFoundError(message));
}

/// <summary>
/// Basis for an "Ok" HTTP response.
/// </summary>
public interface IOk { }

/// <summary>
/// An Ok response, with payload.
/// </summary>
/// <typeparam name="TResponse">The response payload type.</typeparam>
public sealed class Ok<TResponse> : Result<TResponse>, IOk where TResponse : class
{
    public Ok(TResponse response) : base()
    {
        WithValue(response);
    }
}

/// <summary>
/// Basis for a "Created" response.
/// </summary>
public interface ICreated
{
    string Location { get; }
}

/// <summary>
/// A Created response, with payload.
/// </summary>
/// <typeparam name="TResponse">The response payload type.</typeparam>
public sealed class Created<TResponse> : Result<TResponse>, ICreated where TResponse : class
{
    private readonly string _location;

    public Created(TResponse response, string location) : base()
    {
        WithValue(response);

        _location = location;
    }

    public string Location => _location;
}

/// <summary>
/// A NotFound response, with message.
/// </summary>
/// <param name="message">A message detailing the problem particulars.</param>
public sealed class NotFoundError(string message) : Error(message) { }

/// <summary>
/// A InternalServerError response, with message.
/// </summary>
/// <param name="message">A message detailing the problem particulars.</param>
public sealed class InternalServerError(string message) : Error(message) { }
