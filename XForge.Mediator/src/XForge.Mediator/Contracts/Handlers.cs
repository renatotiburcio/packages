namespace XForge.Mediator;

/// <summary>
/// Handles a request of type <typeparamref name="TRequest"/> and returns <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>Handles the request.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response.</returns>
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Convenience marker over <see cref="IRequestHandler{TRequest, TResponse}"/> for commands that return
/// <see cref="Unit"/>. Implement <c>Handle</c> returning <see cref="Unit.CompletedTask"/> when there is no result.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit> where TRequest : IRequest<Unit>
{
}

/// <summary>
/// Handles a streaming request of type <typeparamref name="TRequest"/>, yielding responses of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The stream request type.</typeparam>
/// <typeparam name="TResponse">The streamed item type.</typeparam>
public interface IStreamRequestHandler<in TRequest, TResponse> where TRequest : IStreamRequest<TResponse>
{
    /// <summary>Handles the request, streaming responses.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An async stream of responses.</returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handles a notification of type <typeparamref name="TNotification"/>.
/// All registered handlers receive the notification when it is published.
/// </summary>
/// <typeparam name="TNotification">The notification type.</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    /// <summary>Handles the notification.</summary>
    /// <param name="notification">The notification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when the notification is handled.</returns>
    ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
}
