namespace XForge.Mediator;

/// <summary>
/// Publishing strategy that invokes all handlers in parallel and awaits them together
/// (<see cref="Task.WhenAll(Task[])"/>). Fast paths: no handlers → no-op; single handler → direct await;
/// handlers that complete synchronously are awaited inline with zero <see cref="Task"/> allocations
/// (only genuinely pending handlers are materialized and awaited via <see cref="Task.WhenAll(Task[])"/>).
/// This is the default publisher.
/// </summary>
public class TaskWhenAllPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public ValueTask Publish<TNotification>(IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var executors = handlerExecutors as NotificationHandlerExecutor[] ?? handlerExecutors.ToArray();

        if (executors.Length == 0)
        {
            return default;
        }

        if (executors.Length == 1)
        {
            return executors[0].HandlerCallback(executors[0].HandlerInstance, notification, cancellationToken);
        }

        // Invoke all handlers first (parallel semantics preserved); synchronously-completed handlers
        // are absorbed inline. Only pending/faulting handlers need Task materialization.
        List<Task>? pending = null;
        for (var i = 0; i < executors.Length; i++)
        {
            var valueTask = executors[i].HandlerCallback(executors[i].HandlerInstance, notification, cancellationToken);

            if (valueTask.IsCompletedSuccessfully)
            {
                continue;
            }

            (pending ??= new List<Task>(executors.Length)).Add(valueTask.AsTask());
        }

        return pending is null ? default : AwaitAll(pending);
    }

    private static async ValueTask AwaitAll(List<Task> tasks)
    {
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}

/// <summary>
/// Publishing strategy that invokes handlers sequentially, one after the other.
/// </summary>
public class ForeachAwaitPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public async ValueTask Publish<TNotification>(IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        foreach (var executor in handlerExecutors)
        {
            await executor.HandlerCallback(executor.HandlerInstance, notification, cancellationToken).ConfigureAwait(false);
        }
    }
}
