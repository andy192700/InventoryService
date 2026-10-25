using FluentResults;

namespace InventoryService.Infrastructure;

public static class HttpResponses
{
    public static Ok<TResponse> Ok<TResponse>(TResponse value) where TResponse : class
        => new(value);

    public static Created<TResponse> Created<TResponse>(TResponse value, string location) where TResponse : class
        => new(value, location);

    public static Result Notfound(string message)
        => Result.Fail(new NotFoundError(message));
}

public interface IOk { }

public class Ok<TResponse> : Result<TResponse>, IOk where TResponse : class
{
    public Ok(TResponse response) : base()
    {
        WithValue(response);
    }
}

public interface ICreated
{
    string Location { get; }
}

public class Created<TResponse> : Result<TResponse>, ICreated where TResponse : class
{
    private readonly string _location;

    public Created(TResponse response, string location) : base()
    {
        WithValue(response);

        _location = location;
    }

    public string Location => _location;
}

public class NotFoundError(string message) : Error(message) { }

public class InternalServerError(string message) : Error(message) { }
