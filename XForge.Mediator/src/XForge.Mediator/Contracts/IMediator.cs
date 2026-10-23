namespace XForge.Mediator;

/// <summary>Sends requests and creates response streams.</summary>
public interface ISender
{
    /// <summary>Sends a request expecting <typeparamref name="TResponse"/>.</summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response.</returns>
    ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request whose response type is discovered from its runtime type
    /// (the closed <see cref="IRequest{TResponse}"/> it implements).
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response, boxed (for <see cref="Unit"/> responses, the boxed <see cref="Unit"/> value).</returns>
    ValueTask<object?> Send(object request, CancellationToken cancellationToken = default);

    /// <summary>Sends a runtime-typed request while declaring the response type explicitly.</summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response.</returns>
    ValueTask<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default);

    /// <summary>Creates a response stream for a streaming request.</summary>
    /// <typeparam name="TResponse">The streamed item type.</typeparam>
    /// <param name="request">The stream request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An async stream of responses.</returns>
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a response stream whose item type is discovered from the request runtime type
    /// (the closed <see cref="IStreamRequest{TResponse}"/> it implements).
    /// </summary>
    /// <param name="request">The stream request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An async stream of responses, boxed.</returns>
    IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default);
}

/// <summary>Publishes notifications to all registered handlers.</summary>
public interface IPublisher
{
    /// <summary>Publishes a notification to all registered handlers of its runtime type.</summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="notification">The notification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when all handlers have been invoked (according to the configured publisher).</returns>
    ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    /// <summary>Publishes a runtime-typed notification to all handlers of its runtime type.</summary>
    /// <param name="notification">The notification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when all handlers have been invoked (according to the configured publisher).</returns>
    ValueTask Publish(object notification, CancellationToken cancellationToken = default);
}

/// <summary>The mediator facade: <see cref="ISender"/> + <see cref="IPublisher"/>.</summary>
public interface IMediator : ISender, IPublisher
{
}
