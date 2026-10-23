using System.Runtime.CompilerServices;
using Ar = Arbiter.Mediation;
using Md = Mediator;
using Mr = MediatR;
using Xf = XForge.Mediator;

namespace XForge.Mediator.Benchmarks;

// =====================================================================
// XForge.Mediator
// =====================================================================

public sealed record XfPing(string Message) : Xf.IRequest<XfPong>;
public sealed record XfPong(string Message);

public sealed class XfPingHandler : Xf.IRequestHandler<XfPing, XfPong>
{
    public ValueTask<XfPong> Handle(XfPing request, CancellationToken cancellationToken)
        => ValueTask.FromResult(new XfPong(request.Message));
}

public sealed record XfReset : Xf.IRequest;

public sealed class XfResetHandler : Xf.IRequestHandler<XfReset>
{
    public ValueTask<Xf.Unit> Handle(XfReset request, CancellationToken cancellationToken)
        => Xf.Unit.CompletedTask;
}

public sealed record XfPinged : Xf.INotification;

public sealed class XfPingedHandler : Xf.INotificationHandler<XfPinged>
{
    public ValueTask Handle(XfPinged notification, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;
}

public sealed class XfBehavior<TRequest, TResponse> : Xf.IPipelineBehavior<TRequest, TResponse>
    where TRequest : Xf.IRequest<TResponse>
{
    public ValueTask<TResponse> Handle(TRequest request, Xf.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}

public sealed record XfNumbers(int Count) : Xf.IStreamRequest<int>;

public sealed class XfNumbersHandler : Xf.IStreamRequestHandler<XfNumbers, int>
{
    public async IAsyncEnumerable<int> Handle(XfNumbers request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < request.Count; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }
}

// =====================================================================
// MediatR 12.5
// =====================================================================

public sealed record MrPing(string Message) : Mr.IRequest<MrPong>;
public sealed record MrPong(string Message);

public sealed class MrPingHandler : Mr.IRequestHandler<MrPing, MrPong>
{
    public Task<MrPong> Handle(MrPing request, CancellationToken cancellationToken)
        => Task.FromResult(new MrPong(request.Message));
}

public sealed record MrReset : Mr.IRequest;

public sealed class MrResetHandler : Mr.IRequestHandler<MrReset>
{
    public Task Handle(MrReset request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

public sealed record MrPinged : Mr.INotification;

public sealed class MrPingedHandler : Mr.INotificationHandler<MrPinged>
{
    public Task Handle(MrPinged notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

public sealed class MrBehavior<TRequest, TResponse> : Mr.IPipelineBehavior<TRequest, TResponse>
    where TRequest : Mr.IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, Mr.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}

public sealed record MrNumbers(int Count) : Mr.IStreamRequest<int>;

public sealed class MrNumbersHandler : Mr.IStreamRequestHandler<MrNumbers, int>
{
    public async IAsyncEnumerable<int> Handle(MrNumbers request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < request.Count; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }
}

// =====================================================================
// Mediator 3.0.2 (martinothamar, source generator)
// =====================================================================

public sealed record MdPing(string Message) : Md.IRequest<MdPong>;
public sealed record MdPong(string Message);

public sealed class MdPingHandler : Md.IRequestHandler<MdPing, MdPong>
{
    public ValueTask<MdPong> Handle(MdPing request, CancellationToken cancellationToken)
        => ValueTask.FromResult(new MdPong(request.Message));
}

public sealed record MdReset : Md.ICommand;

public sealed class MdResetHandler : Md.ICommandHandler<MdReset>
{
    public ValueTask<Md.Unit> Handle(MdReset command, CancellationToken cancellationToken)
        => Md.Unit.ValueTask;
}

public sealed record MdPinged : Md.INotification;

public sealed class MdPingedHandler : Md.INotificationHandler<MdPinged>
{
    public ValueTask Handle(MdPinged notification, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;
}

public sealed class MdBehavior<TMessage, TResponse> : Md.IPipelineBehavior<TMessage, TResponse>
    where TMessage : Md.IMessage
{
    public ValueTask<TResponse> Handle(TMessage message, Md.MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
        => next(message, cancellationToken);
}

public sealed record MdNumbers(int Count) : Md.IStreamRequest<int>;

public sealed class MdNumbersHandler : Md.IStreamRequestHandler<MdNumbers, int>
{
    public async IAsyncEnumerable<int> Handle(MdNumbers request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < request.Count; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }
}

#nullable disable
// =====================================================================
// Arbiter.Mediation 6.13.1 (interfaces sem anotações de nulidade)
// =====================================================================

public sealed record ArPing(string Message) : Ar.IRequest<ArPong>;
public sealed record ArPong(string Message);

public sealed class ArPingHandler : Ar.IRequestHandler<ArPing, ArPong>
{
    public ValueTask<ArPong> Handle(ArPing request, CancellationToken cancellationToken)
        => ValueTask.FromResult(new ArPong(request.Message));
}

public sealed record ArReset : Ar.IRequest<Ar.Unit>;

public sealed class ArResetHandler : Ar.IRequestHandler<ArReset, Ar.Unit>
{
    public ValueTask<Ar.Unit> Handle(ArReset request, CancellationToken cancellationToken)
        => ValueTask.FromResult(default(Ar.Unit));
}

public sealed record ArPinged : Ar.INotification;

public sealed class ArPingedHandler : Ar.INotificationHandler<ArPinged>
{
    public ValueTask Handle(ArPinged notification, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;
}

public sealed class ArBehavior<TRequest, TResponse> : Ar.IPipelineBehavior<TRequest, TResponse>
    where TRequest : Ar.IRequest<TResponse>
{
    public ValueTask<TResponse> Handle(TRequest request, Ar.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}
#nullable restore