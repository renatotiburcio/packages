using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator.Internal;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Internal;

/// <summary>
/// Cached-per-request-type dispatch wrappers. Instances are stateless singletons: the
/// <see cref="IServiceProvider"/> is supplied per call, so handler/behavior lifetimes are respected.
/// </summary>
internal static class Wrappers
{
    internal abstract class RequestHandlerWrapperBase
    {
        /// <summary>The response type this wrapper produces.</summary>
        public abstract Type ResponseType { get; }

        /// <summary>Handles a boxed request, returning a boxed response.</summary>
        public abstract ValueTask<object?> HandleObject(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerWrapperBase
    {
        /// <inheritdoc />
        public override Type ResponseType => typeof(TResponse);

        /// <summary>Handles a typed request.</summary>
        public abstract ValueTask<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);

        /// <inheritdoc />
        public override async ValueTask<object?> HandleObject(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            return await Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);
        }
    }

    internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>Closed 1-argument handler interface (only used for the Unit response fallback lookup).</summary>
        private static readonly Type UnitHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(typeof(TRequest));

        /// <inheritdoc />
        public override ValueTask<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();

            if (handler is null && typeof(TResponse) == typeof(Unit))
            {
                // Fallback: handlers registered under the 1-argument convenience interface
                // (it inherits IRequestHandler<TRequest, Unit>, but MS.DI does not map registrations across interfaces).
                var unitHandler = serviceProvider.GetService(UnitHandlerInterfaceType);

                if (unitHandler is not null)
                {
                    handler = (IRequestHandler<TRequest, TResponse>)(object)unitHandler;
                }
            }

            if (handler is null)
            {
                throw new HandlerNotFoundException(typeof(TRequest), typeof(TResponse));
            }

            var behaviorsEnumerable = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();
            var behaviors = behaviorsEnumerable as IPipelineBehavior<TRequest, TResponse>[] ?? behaviorsEnumerable.ToArray();
            var typedRequest = (TRequest)request;

            if (behaviors.Length == 0)
            {
                // Fast path: no behaviors → direct handler invocation with integrated exception processing.
                return ExceptionProcessor.Handle(handler, typedRequest, serviceProvider, cancellationToken);
            }

            RequestHandlerDelegate<TResponse> next = ct => ExceptionProcessor.Handle(handler, typedRequest, serviceProvider, ct);

            for (var i = behaviors.Length - 1; i >= 0; i--)
            {
                var behavior = behaviors[i];
                var inner = next;
                next = ct => behavior.Handle(typedRequest, inner, ct);
            }

            return next(cancellationToken);
        }
    }

    internal abstract class StreamRequestHandlerWrapperBase
    {
        /// <summary>The streamed item type this wrapper produces.</summary>
        public abstract Type ResponseType { get; }

        /// <summary>Handles a boxed stream request, returning a boxed stream.</summary>
        public abstract IAsyncEnumerable<object?> HandleObjectStream(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    internal abstract class StreamRequestHandlerWrapper<TResponse> : StreamRequestHandlerWrapperBase
    {
        /// <inheritdoc />
        public override Type ResponseType => typeof(TResponse);

        /// <summary>Handles a typed stream request.</summary>
        public abstract IAsyncEnumerable<TResponse> Handle(IStreamRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);

        /// <inheritdoc />
        public override async IAsyncEnumerable<object?> HandleObjectStream(object request, IServiceProvider serviceProvider,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in Handle((IStreamRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }

    internal sealed class StreamRequestHandlerWrapperImpl<TRequest, TResponse> : StreamRequestHandlerWrapper<TResponse>
        where TRequest : IStreamRequest<TResponse>
    {
        /// <inheritdoc />
        public override IAsyncEnumerable<TResponse> Handle(IStreamRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetService<IStreamRequestHandler<TRequest, TResponse>>();

            if (handler is null)
            {
                throw new HandlerNotFoundException(typeof(TRequest), typeof(TResponse));
            }

            var behaviorsEnumerable = serviceProvider.GetServices<IStreamPipelineBehavior<TRequest, TResponse>>();
            var behaviors = behaviorsEnumerable as IStreamPipelineBehavior<TRequest, TResponse>[] ?? behaviorsEnumerable.ToArray();
            var typedRequest = (TRequest)request;

            if (behaviors.Length == 0)
            {
                // Fast path: no stream behaviors registered for this request type.
                return handler.Handle(typedRequest, cancellationToken);
            }

            StreamHandlerDelegate<TResponse> next = () => handler.Handle(typedRequest, cancellationToken);

            for (var i = behaviors.Length - 1; i >= 0; i--)
            {
                var behavior = behaviors[i];
                var inner = next;
                next = () => behavior.Handle(typedRequest, inner, cancellationToken);
            }

            return next();
        }
    }

    internal abstract class NotificationHandlerWrapperBase
    {
        /// <summary>Builds the handler executors for a boxed notification and delegates publishing to <paramref name="publisher"/>.</summary>
        public abstract ValueTask Handle(object notification, IServiceProvider serviceProvider, INotificationPublisher publisher, CancellationToken cancellationToken);
    }

    internal sealed class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapperBase
        where TNotification : INotification
    {
        /// <summary>Cached static invoker: no per-publish closures.</summary>
        private static readonly NotificationHandlerCallback Callback = static (handler, notification, cancellationToken) =>
            ((INotificationHandler<TNotification>)handler).Handle((TNotification)notification, cancellationToken);

        /// <inheritdoc />
        public override ValueTask Handle(object notification, IServiceProvider serviceProvider, INotificationPublisher publisher, CancellationToken cancellationToken)
        {
            var handlersEnumerable = serviceProvider.GetServices<INotificationHandler<TNotification>>();
            var handlers = handlersEnumerable as INotificationHandler<TNotification>[] ?? handlersEnumerable.ToArray();
            var typedNotification = (TNotification)notification;

            if (handlers.Length == 0)
            {
                // Fast path: no handlers → multi-cast no-op (same semantics as MediatR).
                return default;
            }

            var executors = new NotificationHandlerExecutor[handlers.Length];
            for (var i = 0; i < handlers.Length; i++)
            {
                executors[i] = new NotificationHandlerExecutor(handlers[i], Callback);
            }

            return publisher.Publish(executors, typedNotification, cancellationToken);
        }
    }
}
