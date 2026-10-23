using XForge.Mediator;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Tests;

// ---------- Requests / commands / queries ----------

public sealed class Ping : IRequest<Pong>
{
    public string Message { get; init; } = "ping";
}

public sealed record Pong(string Message);

public sealed class PingHandler : IRequestHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken)
        => ValueTask.FromResult(new Pong(request.Message + "-pong"));
}

public sealed class Reset : ICommand
{
    public List<string>? Log { get; init; }
}

public sealed class ResetHandler : IRequestHandler<Reset>
{
    public ValueTask<Unit> Handle(Reset request, CancellationToken cancellationToken)
    {
        request.Log?.Add("reset");
        return Unit.CompletedTask;
    }
}

public sealed class GetUser : IQuery<string>
{
    public int Id { get; init; }
}

public sealed class GetUserHandler : IRequestHandler<GetUser, string>
{
    public ValueTask<string> Handle(GetUser request, CancellationToken cancellationToken)
        => ValueTask.FromResult($"user-{request.Id}");
}

// ---------- Derivado (dispatch por runtime type) ----------

public abstract class NotificationBase : IRequest<string>
{
}

public sealed class DerivedNotification : NotificationBase
{
}

public class NotificationBaseHandler : IRequestHandler<NotificationBase, string>
{
    public ValueTask<string> Handle(NotificationBase request, CancellationToken cancellationToken)
        => ValueTask.FromResult("base");
}

public sealed class DerivedNotificationHandler : IRequestHandler<DerivedNotification, string>
{
    public ValueTask<string> Handle(DerivedNotification request, CancellationToken cancellationToken)
        => ValueTask.FromResult("derived");
}

// ---------- Notificações ----------

public sealed class Pinged : INotification
{
    public List<string>? Received { get; init; }
}

public sealed class PingedHandler1 : INotificationHandler<Pinged>
{
    public ValueTask Handle(Pinged notification, CancellationToken cancellationToken)
    {
        notification.Received?.Add("h1");
        return ValueTask.CompletedTask;
    }
}

public sealed class PingedHandler2 : INotificationHandler<Pinged>
{
    public ValueTask Handle(Pinged notification, CancellationToken cancellationToken)
    {
        notification.Received?.Add("h2");
        return ValueTask.CompletedTask;
    }
}

// ---------- Behaviors ----------

public sealed class OrderTracker
{
    public List<string> Steps { get; } = new();
}

public sealed class OuterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly OrderTracker? _tracker;

    public OuterBehavior(OrderTracker? tracker = null) => _tracker = tracker;

    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _tracker?.Steps.Add("outer-pre");
        var response = await next().ConfigureAwait(false);
        _tracker?.Steps.Add("outer-post");
        return response;
    }
}

public sealed class InnerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly OrderTracker? _tracker;

    public InnerBehavior(OrderTracker? tracker = null) => _tracker = tracker;

    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _tracker?.Steps.Add("inner-pre");
        var response = await next().ConfigureAwait(false);
        _tracker?.Steps.Add("inner-post");
        return response;
    }
}

public sealed class ShortCircuitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => ValueTask.FromResult(default(TResponse)!);
}

// ---------- Processors ----------

public sealed class PingPreProcessor : IRequestPreProcessor<Ping>
{
    private readonly OrderTracker? _tracker;

    public PingPreProcessor(OrderTracker? tracker = null) => _tracker = tracker;

    public ValueTask Process(Ping request, CancellationToken cancellationToken)
    {
        _tracker?.Steps.Add("pre");
        return ValueTask.CompletedTask;
    }
}

public sealed class PingPostProcessor : IRequestPostProcessor<Ping, Pong>
{
    private readonly OrderTracker? _tracker;

    public PingPostProcessor(OrderTracker? tracker = null) => _tracker = tracker;

    public ValueTask Process(Ping request, Pong response, CancellationToken cancellationToken)
    {
        _tracker?.Steps.Add("post");
        return ValueTask.CompletedTask;
    }
}

// ---------- Exceções ----------

public sealed class BoomException : Exception
{
    public BoomException(string message) : base(message) { }
}

public sealed class Boom : IRequest<string>
{
}

public sealed class BoomHandler : IRequestHandler<Boom, string>
{
    public ValueTask<string> Handle(Boom request, CancellationToken cancellationToken)
        => throw new BoomException("boom");
}

public sealed class BoomRecoveryHandler : IRequestExceptionHandler<Boom, string, BoomException>
{
    public ValueTask Handle(Boom request, BoomException exception, IRequestExceptionHandlerState<string> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled("recovered");
        return ValueTask.CompletedTask;
    }
}

public sealed class BoomAction : IRequestExceptionAction<Boom, BoomException>
{
    public List<string>? Seen { get; init; }

    public ValueTask Process(Boom request, BoomException exception, CancellationToken cancellationToken)
    {
        Seen?.Add(exception.Message);
        return ValueTask.CompletedTask;
    }
}

public sealed class AnyExceptionRecovery : IRequestExceptionHandler<Boom, string, Exception>
{
    public ValueTask Handle(Boom request, Exception exception, IRequestExceptionHandlerState<string> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled("recovered-base");
        return ValueTask.CompletedTask;
    }
}

// ---------- Stream ----------

public sealed class Numbers : IStreamRequest<int>
{
    public int Count { get; init; } = 3;
}

public sealed class NumbersHandler : IStreamRequestHandler<Numbers, int>
{
    public async IAsyncEnumerable<int> Handle(Numbers request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < request.Count; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }
}

public sealed class StreamDoublingBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    public async IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in next().ConfigureAwait(false))
        {
            yield return item;
            yield return item;
        }
    }
}
