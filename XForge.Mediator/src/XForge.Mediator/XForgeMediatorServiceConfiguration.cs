using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace XForge.Mediator;

/// <summary>
/// Registration options for <c>AddXForgeMediator</c>.
/// </summary>
public sealed class XForgeMediatorServiceConfiguration
{
    /// <summary>
    /// Service lifetime applied to handlers, behaviors, processors <b>and the mediator itself</b>
    /// (<see cref="IMediator"/>, <see cref="ISender"/>, <see cref="IPublisher"/>).
    /// Default: <see cref="ServiceLifetime.Transient"/> (safe for scoped handlers — the mediator resolves
    /// handlers from the provider/scope it was resolved from, same as MediatR's default).
    /// Set to <see cref="ServiceLifetime.Singleton"/> for maximum performance when handlers are singleton-safe.
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    /// Optional predicate to include/exclude types during assembly scanning. Default: include all candidate types.
    /// </summary>
    public Func<Type, bool>? TypeEvaluator { get; set; }

    /// <summary>Assemblies scanned for handlers, processors and exception actions/handlers.</summary>
    public List<Assembly> AssembliesToRegister { get; } = new();

    /// <summary>Behaviors (closed or open generic) registered around request pipelines.</summary>
    public List<Type> BehaviorsToRegister { get; } = new();

    /// <summary>Behaviors (closed or open generic) registered around streaming pipelines.</summary>
    public List<Type> StreamBehaviorsToRegister { get; } = new();

    /// <summary>Pre-processors (closed or open generic) registered for the request pipeline.</summary>
    public List<Type> RequestPreProcessorsToRegister { get; } = new();

    /// <summary>Post-processors (closed or open generic) registered for the request pipeline.</summary>
    public List<Type> RequestPostProcessorsToRegister { get; } = new();

    /// <summary>
    /// When <see langword="true"/> (default), the built-in pre/post/exception behaviors are registered automatically
    /// as soon as any processor or exception action/handler is found (scanned or added via options).
    /// </summary>
    public bool AutoRegisterRequestProcessors { get; set; } = true;

    /// <summary>
    /// When <see langword="true"/>, open generic handlers (e.g. <c>AuditHandler&lt;,&gt;</c>) found during scanning are
    /// registered as open generics. Default: <see langword="false"/> (same default as MediatR, keeps resolution predictable).
    /// </summary>
    public bool RegisterGenericHandlers { get; set; }

    /// <summary>
    /// Enables the built-in ActivitySource/Meter diagnostics. Default: <see langword="false"/> (zero overhead —
    /// dispatch skips instrumentation entirely). Set to <see langword="true"/> to emit
    /// <c>xforge.mediator.send/publish.*</c> metrics and <c>mediator.send/publish</c> activities.
    /// </summary>
    public bool EnableDiagnostics { get; set; }

    /// <summary>
    /// A specific publisher instance to use (wins over <see cref="NotificationPublisherType"/>).
    /// </summary>
    public INotificationPublisher? NotificationPublisher { get; set; }

    /// <summary>
    /// The publisher type to resolve/create when no <see cref="NotificationPublisher"/> instance is supplied.
    /// Default: <see cref="TaskWhenAllPublisher"/> (parallel).
    /// </summary>
    public Type NotificationPublisherType { get; set; } = typeof(TaskWhenAllPublisher);

    /// <summary>Registers all candidate types from <paramref name="assembly"/>.</summary>
    public XForgeMediatorServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);
        return this;
    }

    /// <summary>Registers all candidate types from the assembly containing <typeparamref name="T"/>.</summary>
    public XForgeMediatorServiceConfiguration RegisterServicesFromAssemblyContaining<T>()
    {
        return RegisterServicesFromAssembly(typeof(T).Assembly);
    }

    /// <summary>Registers all candidate types from the assembly containing <paramref name="type"/>.</summary>
    public XForgeMediatorServiceConfiguration RegisterServicesFromAssemblyContaining(Type type)
    {
        return RegisterServicesFromAssembly(type.Assembly);
    }

    /// <summary>Registers all candidate types from the supplied assemblies.</summary>
    public XForgeMediatorServiceConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AssembliesToRegister.Add(assembly);
        }

        return this;
    }

    /// <summary>Registers a behavior (closed or open generic) around request pipelines.</summary>
    public XForgeMediatorServiceConfiguration AddBehavior(Type behaviorType)
    {
        BehaviorsToRegister.Add(behaviorType);
        return this;
    }

    /// <summary>Registers a behavior (closed or open generic) around request pipelines.</summary>
    public XForgeMediatorServiceConfiguration AddBehavior<TBehavior>()
    {
        return AddBehavior(typeof(TBehavior));
    }

    /// <summary>Alias of <see cref="AddBehavior(Type)"/> for open generic behaviors.</summary>
    public XForgeMediatorServiceConfiguration AddOpenBehavior(Type behaviorType)
    {
        return AddBehavior(behaviorType);
    }

    /// <summary>Alias of <see cref="AddBehavior{TBehavior}"/> for open generic behaviors.</summary>
    public XForgeMediatorServiceConfiguration AddOpenBehavior<TBehavior>()
    {
        return AddBehavior(typeof(TBehavior));
    }

    /// <summary>Registers a behavior (closed or open generic) around streaming pipelines.</summary>
    public XForgeMediatorServiceConfiguration AddStreamBehavior(Type behaviorType)
    {
        StreamBehaviorsToRegister.Add(behaviorType);
        return this;
    }

    /// <summary>Registers a behavior (closed or open generic) around streaming pipelines.</summary>
    public XForgeMediatorServiceConfiguration AddStreamBehavior<TBehavior>()
    {
        return AddStreamBehavior(typeof(TBehavior));
    }

    /// <summary>Alias of <see cref="AddStreamBehavior(Type)"/> for open generic behaviors.</summary>
    public XForgeMediatorServiceConfiguration AddOpenStreamBehavior(Type behaviorType)
    {
        return AddStreamBehavior(behaviorType);
    }

    /// <summary>Registers a pre-processor (closed or open generic).</summary>
    public XForgeMediatorServiceConfiguration AddRequestPreProcessor(Type processorType)
    {
        RequestPreProcessorsToRegister.Add(processorType);
        return this;
    }

    /// <summary>Registers a pre-processor (closed or open generic).</summary>
    public XForgeMediatorServiceConfiguration AddOpenRequestPreProcessor(Type processorType)
    {
        return AddRequestPreProcessor(processorType);
    }

    /// <summary>Registers a post-processor (closed or open generic).</summary>
    public XForgeMediatorServiceConfiguration AddRequestPostProcessor(Type processorType)
    {
        RequestPostProcessorsToRegister.Add(processorType);
        return this;
    }

    /// <summary>Registers a post-processor (closed or open generic).</summary>
    public XForgeMediatorServiceConfiguration AddOpenRequestPostProcessor(Type processorType)
    {
        return AddRequestPostProcessor(processorType);
    }
}
