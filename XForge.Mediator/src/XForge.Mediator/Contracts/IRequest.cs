namespace XForge.Mediator;

/// <summary>
/// Marker for messages dispatched by <c>Send</c> that expect a response.
/// </summary>
/// <remarks>
/// All dispatch contracts derive from this marker, so the engine can resolve handlers and
/// validate response types without knowing the concrete message role (request, command or query).
/// </remarks>
public interface IBaseRequest
{
}

/// <summary>Marker for messages dispatched by <c>CreateStream</c> that stream responses.</summary>
public interface IBaseStreamRequest
{
}

/// <summary>
/// A request/response message. Implement to send a request that returns <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequest<TResponse> : IBaseRequest
{
}

/// <summary>A request that returns <see cref="Unit"/> (no meaningful response).</summary>
public interface IRequest : IRequest<Unit>
{
}

/// <summary>A streaming request that returns <see cref="IAsyncEnumerable{T}"/> of <typeparamref name="TResponse"/>.</summary>
public interface IStreamRequest<TResponse> : IBaseStreamRequest
{
}

/// <summary>
/// Convenience marker for a command that mutates state and expects <typeparamref name="TResponse"/>.
/// Purely semantic: dispatch only depends on <see cref="IRequest{TResponse}"/>.
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}

/// <summary>A command that mutates state and returns <see cref="Unit"/>.</summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Convenience marker for a query that reads state and returns <typeparamref name="TResponse"/>.
/// Purely semantic: dispatch only depends on <see cref="IRequest{TResponse}"/>.
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}
