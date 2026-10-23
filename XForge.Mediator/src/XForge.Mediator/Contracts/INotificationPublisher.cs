namespace XForge.Mediator;

/// <summary>
/// Invokes a notification handler: <c>(handlerInstance, notification, cancellationToken) → ValueTask</c>.
/// Static, cacheable per notification type — no per-publish closures.
/// </summary>
public delegate ValueTask NotificationHandlerCallback(object handlerInstance, object notification, CancellationToken cancellationToken);

/// <summary>
/// One registered notification handler plus an invoker for its <c>Handle</c> method.
/// Passed to <see cref="INotificationPublisher.Publish{TNotification}"/> so custom publishing strategies
/// can choose how (and when) to invoke handlers.
/// </summary>
public readonly struct NotificationHandlerExecutor
{
    /// <summary>Creates an executor.</summary>
    /// <param name="handlerInstance">The resolved handler instance (usually from DI).</param>
    /// <param name="handlerCallback">Invokes the handler: <c>(handler, notification, cancellationToken) → ValueTask</c>.</param>
    public NotificationHandlerExecutor(object handlerInstance, NotificationHandlerCallback handlerCallback)
    {
        HandlerInstance = handlerInstance;
        HandlerCallback = handlerCallback;
    }

    /// <summary>The resolved handler instance.</summary>
    public object HandlerInstance { get; }

    /// <summary>Invokes the handler: <c>(handler, notification, cancellationToken) → ValueTask</c>.</summary>
    public NotificationHandlerCallback HandlerCallback { get; }
}

/// <summary>
/// Defines how the registered handlers of a notification are invoked when it is published.
/// </summary>
/// <remarks>
/// Built-in strategies: <see cref="ForeachAwaitPublisher"/> (sequential) and
/// <see cref="TaskWhenAllPublisher"/> (parallel, the default).
/// </remarks>
public interface INotificationPublisher
{
    /// <summary>Publishes a notification using the supplied handler executors.</summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="handlerExecutors">The registered handlers and their invokers.</param>
    /// <param name="notification">The notification being published.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes according to the publishing strategy.</returns>
    ValueTask Publish<TNotification>(IEnumerable<NotificationHandlerExecutor> handlerExecutors, TNotification notification,
        CancellationToken cancellationToken = default) where TNotification : INotification;
}
