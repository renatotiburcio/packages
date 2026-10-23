namespace XForge.Mediator;

/// <summary>
/// Thrown when a request or stream request has no registered handler.
/// Notifications without handlers are no-ops by design (multi-cast semantics) and do not throw.
/// </summary>
public class HandlerNotFoundException : InvalidOperationException
{
    /// <summary>The request type that had no handler.</summary>
    public Type RequestType { get; }

    /// <summary>The expected response type, when known.</summary>
    public Type? ResponseType { get; }

    /// <summary>Creates the exception.</summary>
    /// <param name="requestType">The request type.</param>
    /// <param name="responseType">The expected response type, when known.</param>
    public HandlerNotFoundException(Type requestType, Type? responseType = null)
        : base(BuildMessage(requestType, responseType))
    {
        RequestType = requestType;
        ResponseType = responseType;
    }

    private static string BuildMessage(Type requestType, Type? responseType)
    {
        var responseSuffix = responseType is null ? string.Empty : $" with response '{responseType}'";
        return
            $"No handler registered for request of type '{requestType}'{responseSuffix}. " +
            $"Register a handler implementing IRequestHandler<{requestType.Name}, {responseType?.Name ?? "TResponse"}> " +
            "(or IRequestHandler<TRequest> for Unit responses) and ensure the handler assembly is registered, " +
            "e.g. services.AddXForgeMediator(cfg => cfg.RegisterServicesFromAssemblyContaining<...>());";
    }
}
