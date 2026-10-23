using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator.Internal;

namespace XForge.Mediator;

/// <summary>
/// The default <see cref="IMediator"/> implementation.
/// Dispatch wrappers are built once per request type (reflection is only used on first dispatch and for
/// runtime-typed overloads) and cached in static dictionaries, so the steady-state hot path is
/// dictionary lookup + DI resolution + delegate invocation.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _publisher;

    // Static caches: wrappers are stateless (they receive the service provider per call),
    // so they can be shared across mediator instances/process lifetime.
    private static readonly ConcurrentDictionary<Type, Wrappers.RequestHandlerWrapperBase> RequestHandlers = new();
    private static readonly ConcurrentDictionary<Type, Wrappers.StreamRequestHandlerWrapperBase> StreamRequestHandlers = new();
    private static readonly ConcurrentDictionary<Type, Wrappers.NotificationHandlerWrapperBase> NotificationHandlers = new();

    /// <summary>Creates the mediator. The provider must be able to resolve <see cref="INotificationPublisher"/>.</summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers, behaviors and publishers.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _publisher = serviceProvider.GetRequiredService<INotificationPublisher>();
    }

    /// <summary>Creates the mediator with an explicit notification publisher (bypasses DI resolution).</summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers and behaviors.</param>
    /// <param name="publisher">The publishing strategy for notifications.</param>
    public Mediator(IServiceProvider serviceProvider, INotificationPublisher publisher)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    /// <inheritdoc />
    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var baseWrapper = RequestHandlers.GetOrAdd(
            requestType,
            static t => CreateRequestWrapper(t, typeof(TResponse)));

        if (baseWrapper.ResponseType != typeof(TResponse))
        {
            throw new InvalidOperationException(
                $"Request of type '{requestType}' was sent expecting response '{typeof(TResponse)}', but it implements IRequest<{baseWrapper.ResponseType}>.");
        }

        var wrapper = (Wrappers.RequestHandlerWrapper<TResponse>)baseWrapper;

        if (!XForgeMediatorDiagnostics.Enabled)
        {
            return wrapper.Handle(request, _serviceProvider, cancellationToken);
        }

        return SendInstrumented(wrapper, request, cancellationToken);
    }

    /// <inheritdoc />
    #if NET8_0_OR_GREATER
    [RequiresDynamicCode(DynamicDispatchRequiresDynamicCode)]
    [RequiresUnreferencedCode(DynamicDispatchRequiresUnreferencedCode)]
    #endif
    public ValueTask<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var wrapper = RequestHandlers.GetOrAdd(
            requestType,
            static t => CreateRequestWrapper(t, RequestTypeHelper.GetResponseType(t)));

        if (!XForgeMediatorDiagnostics.Enabled)
        {
            return wrapper.HandleObject(request, _serviceProvider, cancellationToken);
        }

        return SendObjectInstrumented(wrapper, request, cancellationToken);
    }

    /// <inheritdoc />
    #if NET8_0_OR_GREATER
    [RequiresDynamicCode(DynamicDispatchRequiresDynamicCode)]
    [RequiresUnreferencedCode(DynamicDispatchRequiresUnreferencedCode)]
    #endif
    public ValueTask<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var baseWrapper = RequestHandlers.GetOrAdd(
            requestType,
            static t => CreateRequestWrapper(t, typeof(TResponse)));

        if (baseWrapper.ResponseType != typeof(TResponse))
        {
            throw new InvalidOperationException(
                $"Request of type '{requestType}' was sent expecting response '{typeof(TResponse)}', but it implements IRequest<{baseWrapper.ResponseType}>.");
        }

        var wrapper = (Wrappers.RequestHandlerWrapper<TResponse>)baseWrapper;

        if (!XForgeMediatorDiagnostics.Enabled)
        {
            return wrapper.Handle((IRequest<TResponse>)request, _serviceProvider, cancellationToken);
        }

        return SendInstrumented(wrapper, (IRequest<TResponse>)request, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var baseWrapper = StreamRequestHandlers.GetOrAdd(
            requestType,
            static t => CreateStreamRequestWrapper(t, typeof(TResponse)));

        if (baseWrapper.ResponseType != typeof(TResponse))
        {
            throw new InvalidOperationException(
                $"Stream request of type '{requestType}' was sent expecting item type '{typeof(TResponse)}', but it implements IStreamRequest<{baseWrapper.ResponseType}>.");
        }

        var wrapper = (Wrappers.StreamRequestHandlerWrapper<TResponse>)baseWrapper;

        return wrapper.Handle(request, _serviceProvider, cancellationToken);
    }

    /// <inheritdoc />
    #if NET8_0_OR_GREATER
    [RequiresDynamicCode(DynamicDispatchRequiresDynamicCode)]
    [RequiresUnreferencedCode(DynamicDispatchRequiresUnreferencedCode)]
    #endif
    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var wrapper = StreamRequestHandlers.GetOrAdd(
            requestType,
            static t => CreateStreamRequestWrapper(t, RequestTypeHelper.GetStreamResponseType(t)));

        return wrapper.HandleObjectStream(request, _serviceProvider, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = typeof(TNotification);
        var wrapper = NotificationHandlers.GetOrAdd(
            notificationType,
            static t => (Wrappers.NotificationHandlerWrapperBase)Activator.CreateInstance(
                typeof(Wrappers.NotificationHandlerWrapperImpl<>).MakeGenericType(t))!);

        if (!XForgeMediatorDiagnostics.Enabled)
        {
            return wrapper.Handle(notification, _serviceProvider, _publisher, cancellationToken);
        }

        return PublishInstrumented(wrapper, notification, cancellationToken);
    }

    /// <inheritdoc />
    #if NET8_0_OR_GREATER
    [RequiresDynamicCode(DynamicDispatchRequiresDynamicCode)]
    [RequiresUnreferencedCode(DynamicDispatchRequiresUnreferencedCode)]
    #endif
    public ValueTask Publish(object notification, CancellationToken cancellationToken = default)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = notification.GetType();
        var wrapper = NotificationHandlers.GetOrAdd(
            notificationType,
            static t => (Wrappers.NotificationHandlerWrapperBase)Activator.CreateInstance(
                typeof(Wrappers.NotificationHandlerWrapperImpl<>).MakeGenericType(t))!);

        if (!XForgeMediatorDiagnostics.Enabled)
        {
            return wrapper.Handle(notification, _serviceProvider, _publisher, cancellationToken);
        }

        return PublishObjectInstrumented(wrapper, notification, cancellationToken);
    }

    private static Wrappers.RequestHandlerWrapperBase CreateRequestWrapper(Type requestType, Type responseType)
    {
        return (Wrappers.RequestHandlerWrapperBase)Activator.CreateInstance(
            typeof(Wrappers.RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType))!;
    }

    private static Wrappers.StreamRequestHandlerWrapperBase CreateStreamRequestWrapper(Type requestType, Type responseType)
    {
        return (Wrappers.StreamRequestHandlerWrapperBase)Activator.CreateInstance(
            typeof(Wrappers.StreamRequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType))!;
    }

    private const string DynamicDispatchRequiresDynamicCode =
        "Runtime-typed dispatch may require dynamic code generation when building the dispatch wrapper for new message types. " +
        "Use the generic overloads (Send<TResponse>(IRequest<TResponse>), CreateStream<TResponse>(IStreamRequest<TResponse>), Publish<TNotification>) for Native AOT.";

    private const string DynamicDispatchRequiresUnreferencedCode =
        "Runtime-typed dispatch relies on metadata of the message's runtime type and may be removed by trimming. " +
        "Use the generic overloads (Send<TResponse>(IRequest<TResponse>), CreateStream<TResponse>(IStreamRequest<TResponse>), Publish<TNotification>) for trimmed deployments.";

    #region Instrumented paths (only used when XForgeMediatorDiagnostics.Enabled)

    private async ValueTask<TResponse> SendInstrumented<TResponse>(Wrappers.RequestHandlerWrapper<TResponse> wrapper,
        IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        Activity? activity = null;
        if (XForgeMediatorDiagnostics.ActivitySource.HasListeners())
        {
            activity = XForgeMediatorDiagnostics.ActivitySource.StartActivity(XForgeMediatorDiagnostics.SendActivityName);
            activity?.SetTag(XForgeMediatorDiagnostics.RequestTypeTag, request.GetType().FullName);
            activity?.SetTag(XForgeMediatorDiagnostics.ResponseTypeTag, typeof(TResponse).FullName);
        }

        var start = Stopwatch.GetTimestamp();

        try
        {
            return await wrapper.Handle(request, _serviceProvider, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (activity is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
        finally
        {
            var elapsed = XForgeMediatorDiagnostics.ElapsedMilliseconds(start);
            XForgeMediatorDiagnostics.SendCounter.Add(1);
            XForgeMediatorDiagnostics.SendDuration.Record(elapsed);

            activity?.Dispose();
        }
    }

    private async ValueTask<object?> SendObjectInstrumented(Wrappers.RequestHandlerWrapperBase wrapper,
        object request, CancellationToken cancellationToken)
    {
        Activity? activity = null;
        if (XForgeMediatorDiagnostics.ActivitySource.HasListeners())
        {
            activity = XForgeMediatorDiagnostics.ActivitySource.StartActivity(XForgeMediatorDiagnostics.SendActivityName);
            activity?.SetTag(XForgeMediatorDiagnostics.RequestTypeTag, request.GetType().FullName);
        }

        var start = Stopwatch.GetTimestamp();

        try
        {
            return await wrapper.HandleObject(request, _serviceProvider, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (activity is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
        finally
        {
            var elapsed = XForgeMediatorDiagnostics.ElapsedMilliseconds(start);
            XForgeMediatorDiagnostics.SendCounter.Add(1);
            XForgeMediatorDiagnostics.SendDuration.Record(elapsed);

            activity?.Dispose();
        }
    }

    private async ValueTask PublishInstrumented<TNotification>(Wrappers.NotificationHandlerWrapperBase wrapper,
        TNotification notification, CancellationToken cancellationToken) where TNotification : INotification
    {
        Activity? activity = null;
        if (XForgeMediatorDiagnostics.ActivitySource.HasListeners())
        {
            activity = XForgeMediatorDiagnostics.ActivitySource.StartActivity(XForgeMediatorDiagnostics.PublishActivityName);
            activity?.SetTag(XForgeMediatorDiagnostics.NotificationTypeTag, notification.GetType().FullName);
        }

        var start = Stopwatch.GetTimestamp();

        try
        {
            await wrapper.Handle(notification, _serviceProvider, _publisher, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (activity is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
        finally
        {
            var elapsed = XForgeMediatorDiagnostics.ElapsedMilliseconds(start);
            XForgeMediatorDiagnostics.PublishCounter.Add(1);
            XForgeMediatorDiagnostics.PublishDuration.Record(elapsed);

            activity?.Dispose();
        }
    }

    private async ValueTask PublishObjectInstrumented(Wrappers.NotificationHandlerWrapperBase wrapper,
        object notification, CancellationToken cancellationToken)
    {
        Activity? activity = null;
        if (XForgeMediatorDiagnostics.ActivitySource.HasListeners())
        {
            activity = XForgeMediatorDiagnostics.ActivitySource.StartActivity(XForgeMediatorDiagnostics.PublishActivityName);
            activity?.SetTag(XForgeMediatorDiagnostics.NotificationTypeTag, notification.GetType().FullName);
        }

        var start = Stopwatch.GetTimestamp();

        try
        {
            await wrapper.Handle(notification, _serviceProvider, _publisher, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (activity is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
        finally
        {
            var elapsed = XForgeMediatorDiagnostics.ElapsedMilliseconds(start);
            XForgeMediatorDiagnostics.PublishCounter.Add(1);
            XForgeMediatorDiagnostics.PublishDuration.Record(elapsed);

            activity?.Dispose();
        }
    }

    #endregion
}
