using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Internal;

/// <summary>
/// Exception processing integrated into the dispatch wrappers (innermost pipeline stage, wraps the handler
/// directly). Zero-cost when no exception occurs; on failures it runs registered
/// <c>IRequestExceptionAction</c>s (side effects) and <c>IRequestExceptionHandler</c>s (recovery) for the
/// exception type hierarchy, most derived first.
/// </summary>
internal static class ExceptionProcessor
{
    /// <summary>Invokes the handler with exception processing. Fast path: no exception → returns as-is.</summary>
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler,
        TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        ValueTask<TResponse> result;

        try
        {
            result = handler.Handle(request, cancellationToken);
        }
        catch (Exception exception)
        {
            // Synchronous failure: recover or rethrow (async path only on failures).
            return ProcessAsync<TRequest, TResponse>(exception, request, serviceProvider, cancellationToken);
        }

        if (result.IsCompletedSuccessfully)
        {
            // Zero-allocation fast path: the pipeline completed synchronously without exceptions.
            return result;
        }

        return Awaited(result, request, serviceProvider, cancellationToken);
    }

    private static async ValueTask<TResponse> Awaited<TRequest, TResponse>(ValueTask<TResponse> result, TRequest request,
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        try
        {
            return await result.ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return await ProcessAsync<TRequest, TResponse>(exception, request, serviceProvider, cancellationToken).ConfigureAwait(false);
        }
    }

    internal static async ValueTask<TResponse> ProcessAsync<TRequest, TResponse>(Exception exception, TRequest request,
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // 1) Exception actions: side effects only; cannot recover. Most derived exception type first.
        //    Note: typeof(Exception).BaseType is object, so the walk must stop before object
        //    (it would violate the TException : Exception constraint).
        for (var exceptionType = exception.GetType(); exceptionType is not null && exceptionType != typeof(object); exceptionType = exceptionType.BaseType)
        {
            var info = ExceptionInfoCache<TRequest, TResponse>.For(exceptionType);
            var actions = serviceProvider.GetService(info.ActionsEnumerableType) as IEnumerable;

            if (actions is null)
            {
                continue;
            }

            foreach (var action in actions)
            {
                await ((ValueTask)info.ProcessMethod.Invoke(action, new[] { (object?)request, (object?)exception, (object?)cancellationToken })!).ConfigureAwait(false);
            }
        }

        // 2) Exception handlers: may recover by setting the state. Most derived exception type first.
        var state = new RequestExceptionHandlerState<TResponse>();
        for (var exceptionType = exception.GetType(); exceptionType is not null && exceptionType != typeof(object); exceptionType = exceptionType.BaseType)
        {
            var info = ExceptionInfoCache<TRequest, TResponse>.For(exceptionType);
            var handlers = serviceProvider.GetService(info.HandlersEnumerableType) as IEnumerable;

            if (handlers is null)
            {
                continue;
            }

            foreach (var handler in handlers)
            {
                await ((ValueTask)info.HandleMethod.Invoke(handler, new[] { (object?)request, (object?)exception, (object?)state, (object?)cancellationToken })!).ConfigureAwait(false);

                if (state.IsHandled)
                {
                    return state.Response;
                }
            }
        }

        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        return default!; // inalcançável: EDI.Throw sempre lança
    }

    /// <summary>Reflection cache: closed exception interfaces + methods per exception type, per (request, response).</summary>
    private static class ExceptionInfoCache<TRequest, TResponse>
    {
        private static readonly ConcurrentDictionary<Type, ExceptionInfo> Cache = new();

        public static ExceptionInfo For(Type exceptionType)
        {
            return Cache.GetOrAdd(exceptionType, static exceptionType2 =>
            {
                var actionInterface = typeof(IRequestExceptionAction<,>).MakeGenericType(typeof(TRequest), exceptionType2);
                var processMethod = actionInterface.GetMethod("Process")!;

                var handlerInterface = typeof(IRequestExceptionHandler<,,>).MakeGenericType(typeof(TRequest), typeof(TResponse), exceptionType2);
                var handleMethod = handlerInterface.GetMethod("Handle")!;

                return new ExceptionInfo(
                    typeof(IEnumerable<>).MakeGenericType(actionInterface),
                    processMethod,
                    typeof(IEnumerable<>).MakeGenericType(handlerInterface),
                    handleMethod);
            });
        }
    }

    private sealed class ExceptionInfo
    {
        public ExceptionInfo(Type actionsEnumerableType, MethodInfo processMethod, Type handlersEnumerableType, MethodInfo handleMethod)
        {
            ActionsEnumerableType = actionsEnumerableType;
            ProcessMethod = processMethod;
            HandlersEnumerableType = handlersEnumerableType;
            HandleMethod = handleMethod;
        }

        public Type ActionsEnumerableType { get; }
        public MethodInfo ProcessMethod { get; }
        public Type HandlersEnumerableType { get; }
        public MethodInfo HandleMethod { get; }
    }
}
