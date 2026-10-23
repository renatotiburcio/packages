using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XForge.Mediator.Internal;
using XForge.Mediator.Pipeline;

namespace XForge.Mediator.Internal;

/// <summary>Scans assemblies and registers mediator services into the container.</summary>
internal static class ServiceRegistrar
{
    internal static void RegisterMediatorServices(IServiceCollection services, XForgeMediatorServiceConfiguration configuration)
    {
        // 1) Mediator facade + notification publisher.
        if (configuration.NotificationPublisher is not null)
        {
            services.TryAddSingleton(configuration.NotificationPublisher);
        }
        else
        {
            services.TryAddSingleton<INotificationPublisher>(serviceProvider =>
                (INotificationPublisher)ActivatorUtilities.CreateInstance(serviceProvider, configuration.NotificationPublisherType));
        }

        services.TryAdd(new ServiceDescriptor(typeof(Mediator), typeof(Mediator), configuration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IMediator), static serviceProvider => serviceProvider.GetRequiredService<Mediator>(), configuration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(ISender), static serviceProvider => serviceProvider.GetRequiredService<Mediator>(), configuration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IPublisher), static serviceProvider => serviceProvider.GetRequiredService<Mediator>(), configuration.Lifetime));

        // 2) Scan assemblies for handlers and processors.
        var foundPreProcessors = configuration.RequestPreProcessorsToRegister.Count > 0;
        var foundPostProcessors = configuration.RequestPostProcessorsToRegister.Count > 0;
        foreach (var assembly in configuration.AssembliesToRegister.Distinct())
        {
            foreach (var type in GetCandidateTypes(assembly, configuration.TypeEvaluator))
            {
                if (type.IsGenericTypeDefinition)
                {
                    if (configuration.RegisterGenericHandlers)
                    {
                        RegisterOpenGenericHandler(services, type, configuration.Lifetime);
                    }

                    continue;
                }

                foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
                {
                    if (!implementedInterface.IsGenericType)
                    {
                        continue;
                    }

                    var definition = implementedInterface.GetGenericTypeDefinition();

                    if (definition == typeof(IRequestHandler<,>) ||
                        definition == typeof(IRequestHandler<>) ||
                        definition == typeof(IStreamRequestHandler<,>) ||
                        definition == typeof(INotificationHandler<>))
                    {
                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, configuration.Lifetime));
                    }
                    else if (definition == typeof(IRequestPreProcessor<>))
                    {
                        foundPreProcessors = true;
                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, ServiceLifetime.Singleton));
                    }
                    else if (definition == typeof(IRequestPostProcessor<,>))
                    {
                        foundPostProcessors = true;
                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, ServiceLifetime.Singleton));
                    }
                    else if (definition == typeof(IRequestExceptionHandler<,,>))
                    {
                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, ServiceLifetime.Singleton));
                    }
                    else if (definition == typeof(IRequestExceptionAction<,>))
                    {
                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, ServiceLifetime.Singleton));
                    }
                }
            }
        }

        // 3) Behaviors and processors supplied via options — registered FIRST so they wrap the built-ins
        //    (DI resolution order = outermost-to-innermost pipeline order).
        foreach (var behaviorType in configuration.BehaviorsToRegister)
        {
            RegisterBehavior(services, behaviorType, typeof(IPipelineBehavior<,>), configuration.Lifetime);
        }

        foreach (var behaviorType in configuration.StreamBehaviorsToRegister)
        {
            RegisterBehavior(services, behaviorType, typeof(IStreamPipelineBehavior<,>), configuration.Lifetime);
        }

        foreach (var processorType in configuration.RequestPreProcessorsToRegister)
        {
            RegisterProcessor(services, processorType, typeof(IRequestPreProcessor<>));
        }

        foreach (var processorType in configuration.RequestPostProcessorsToRegister)
        {
            RegisterProcessor(services, processorType, typeof(IRequestPostProcessor<,>));
        }

        // 4) Built-in pipeline behaviors — post/pre are registered only when there is something to do
        //    (keeping the no-processor fast path free of pipeline stages). Exception processing is
        //    integrated directly into the dispatch wrappers (zero-cost when no exception occurs), so
        //    exception actions/handlers work even when registered manually. Order (outermost → innermost):
        //    post-processors → pre-processors → [exception processing wraps the handler].
        if (configuration.AutoRegisterRequestProcessors)
        {
            if (foundPostProcessors)
            {
                services.TryAddEnumerable(new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>), ServiceLifetime.Singleton));
            }

            if (foundPreProcessors)
            {
                services.TryAddEnumerable(new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>), ServiceLifetime.Singleton));
            }
        }
    }

    private static IEnumerable<Type> GetCandidateTypes(Assembly assembly, Func<Type, bool>? typeEvaluator)
    {
        foreach (var type in RequestTypeHelper.GetLoadableTypes(assembly))
        {
            if (type.IsDefined(typeof(XForgeMediatorIgnoreAttribute), inherit: false))
            {
                continue;
            }

            if (type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
            {
                continue;
            }

            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            if (type.Name.StartsWith("<", StringComparison.Ordinal))
            {
                continue;
            }

            if (typeEvaluator is not null && !typeEvaluator(type))
            {
                continue;
            }

            yield return type;
        }
    }

    private static void RegisterOpenGenericHandler(IServiceCollection services, Type openGenericType, ServiceLifetime lifetime)
    {
        foreach (var implementedInterface in openGenericType.GetInterfaces())
        {
            if (!implementedInterface.IsGenericType)
            {
                continue;
            }

            var definition = implementedInterface.GetGenericTypeDefinition();

            if (definition == typeof(IRequestHandler<,>) ||
                definition == typeof(IRequestHandler<>) ||
                definition == typeof(IStreamRequestHandler<,>) ||
                definition == typeof(INotificationHandler<>))
            {
                services.TryAddEnumerable(new ServiceDescriptor(definition, openGenericType, lifetime));
            }
        }
    }

    private static void RegisterBehavior(IServiceCollection services, Type behaviorType, Type behaviorInterfaceDefinition, ServiceLifetime lifetime)
    {
        if (behaviorType.IsGenericTypeDefinition)
        {
            // Open generic behavior: confirm it implements the expected open interface, then register open.
            var matches = false;
            foreach (var implementedInterface in behaviorType.GetInterfaces())
            {
                if (implementedInterface.IsGenericType &&
                    implementedInterface.GetGenericTypeDefinition() == behaviorInterfaceDefinition)
                {
                    matches = true;
                    break;
                }
            }

            if (!matches)
            {
                throw new ArgumentException(
                    $"Type '{behaviorType}' does not implement {behaviorInterfaceDefinition}.", nameof(behaviorType));
            }

            services.TryAddEnumerable(new ServiceDescriptor(behaviorInterfaceDefinition, behaviorType, lifetime));
            return;
        }

        var registered = false;
        foreach (var implementedInterface in behaviorType.GetInterfaces())
        {
            if (implementedInterface.IsGenericType &&
                implementedInterface.GetGenericTypeDefinition() == behaviorInterfaceDefinition)
            {
                services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, behaviorType, lifetime));
                registered = true;
            }
        }

        if (!registered)
        {
            throw new ArgumentException(
                $"Type '{behaviorType}' does not implement {behaviorInterfaceDefinition}.", nameof(behaviorType));
        }
    }

    private static void RegisterProcessor(IServiceCollection services, Type processorType, Type processorInterfaceDefinition)
    {
        RegisterBehavior(services, processorType, processorInterfaceDefinition, ServiceLifetime.Singleton);
    }
}
