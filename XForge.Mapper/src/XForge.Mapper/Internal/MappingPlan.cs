using System.Collections.Concurrent;

namespace XForge.Mapper.Internal;

/// <summary>Non-generic plan interface used for runtime-type dispatch and nested resolution.</summary>
internal interface IMappingPlan
{
    Type SourceType { get; }
    Type DestinationType { get; }
    bool RequiresContext { get; }
    int? MaxDepth { get; }
    int BindingCount { get; }

    object? MapObject(object source, object? destination, MappingContext? context);

    /// <summary>Creates (but does not populate) the destination instance — used for circular-reference caching.</summary>
    object CreateDestinationObject(object source, MappingContext? context);

    /// <summary>Populates an existing destination instance (no callbacks — used by nested/circular flows).</summary>
    void PopulateObject(object source, object destination, MappingContext? context);

    /// <summary>Maps onto an existing instance running Before/After callbacks (merge path).</summary>
    void MapOntoObject(object source, object destination, MappingContext? context);
}

internal interface IMemberBinding<TSource, TDestination>
{
    void Apply(TSource source, TDestination destination, MappingContext? context);
}

/// <summary>
/// One destination member binding: typed getter → transform (converter/nested plan/identity) → typed setter.
/// Used for context-dependent members (circular references, conditional ignores).
/// </summary>
internal sealed class ValueBinding<TSource, TDestination, TSourceValue, TValue> : IMemberBinding<TSource, TDestination>
{
    private readonly string _memberName;
    private readonly Func<TSource, TSourceValue> _getter;
    private readonly Action<TDestination, TValue> _setter;
    private readonly Func<TSource, TSourceValue, MappingContext?, TValue> _transform;

    public ValueBinding(string memberName, Func<TSource, TSourceValue> getter,
        Action<TDestination, TValue> setter, Func<TSource, TSourceValue, MappingContext?, TValue> transform)
    {
        _memberName = memberName;
        _getter = getter;
        _setter = setter;
        _transform = transform;
    }

    public void Apply(TSource source, TDestination destination, MappingContext? context)
    {
        try
        {
            _setter(destination, _transform(source, _getter(source), context));
        }
        catch (MappingException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new MappingException(
                $"Error mapping member '{_memberName}' ({typeof(TSource)} -> {typeof(TDestination)}): {exception.Message}",
                exception, source, _memberName);
        }
    }
}

/// <summary>Skips the inner binding when the condition returns <see langword="true"/> (IgnoreIf).</summary>
internal sealed class ConditionalBinding<TSource, TDestination> : IMemberBinding<TSource, TDestination>
{
    private readonly Func<TSource, bool> _skipWhen;
    private readonly IMemberBinding<TSource, TDestination> _inner;

    public ConditionalBinding(Func<TSource, bool> skipWhen, IMemberBinding<TSource, TDestination> inner)
    {
        _skipWhen = skipWhen;
        _inner = inner;
    }

    public void Apply(TSource source, TDestination destination, MappingContext? context)
    {
        if (!_skipWhen(source))
        {
            _inner.Apply(source, destination, context);
        }
    }
}

/// <summary>
/// Compiled mapping plan for one source→destination pair:
/// <list type="bullet">
///   <item><description><c>_fastAction</c> — ALL context-free assignments compiled into a single delegate
///   (one invocation per map, like AutoMapper/Mapster);</description></item>
///   <item><description><c>_slowBindings</c> — context-dependent members (circular refs, conditional ignores)
///   executed via interface dispatch;</description></item>
///   <item><description>Before/After callbacks.</description></item>
/// </list>
/// Cached per <see cref="MapperConfiguration"/>.
/// </summary>
internal sealed class MappingPlan<TSource, TDestination> : IMappingPlan
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, TDestination> _factory;
    private readonly Action<TSource, TDestination>? _fastAction;
    private readonly List<IMemberBinding<TSource, TDestination>> _slowBindings;
    private readonly List<Delegate> _before;
    private readonly List<Delegate> _after;
    private readonly int? _maxDepth;
    private readonly bool _requiresContext;

    public MappingPlan(Func<TSource, TDestination> factory, Action<TSource, TDestination>? fastAction,
        List<IMemberBinding<TSource, TDestination>> slowBindings, List<Delegate> before, List<Delegate> after,
        int? maxDepth, bool requiresContext)
    {
        _factory = factory;
        _fastAction = fastAction;
        _slowBindings = slowBindings;
        _before = before;
        _after = after;
        _maxDepth = maxDepth;
        _requiresContext = requiresContext;
    }

    public Type SourceType => typeof(TSource);
    public Type DestinationType => typeof(TDestination);
    public bool RequiresContext => _requiresContext;
    public int? MaxDepth => _maxDepth;
    public int BindingCount => _slowBindings.Count + (_fastAction is null ? 0 : 1);

    public TDestination Map(TSource source, TDestination? destination, MappingContext? context)
    {
        if (destination is null)
        {
            destination = _factory(source);

            // cache before populating: nested bindings may point back to this instance (circular graphs)
            context?.CacheCreated(typeof(TSource), typeof(TDestination), source, destination);
        }

        InvokeCallbacks(_before, source, destination, context);

        _fastAction?.Invoke(source, destination);

        foreach (var binding in _slowBindings)
        {
            binding.Apply(source, destination, context);
        }

        InvokeCallbacks(_after, source, destination, context);
        return destination;
    }

    public object? MapObject(object source, object? destination, MappingContext? context)
    {
        return Map((TSource)source, (TDestination?)destination, context);
    }

    public object CreateDestinationObject(object source, MappingContext? context)
    {
        return _factory((TSource)source)!;
    }

    public void PopulateObject(object source, object destination, MappingContext? context)
    {
        var typedDestination = (TDestination)destination;

        _fastAction?.Invoke((TSource)source, typedDestination);

        foreach (var binding in _slowBindings)
        {
            binding.Apply((TSource)source, typedDestination, context);
        }
    }

    public void MapOntoObject(object source, object destination, MappingContext? context)
    {
        var typedDestination = (TDestination)destination;
        InvokeCallbacks(_before, (TSource)source, typedDestination, context);

        _fastAction?.Invoke((TSource)source, typedDestination);

        foreach (var binding in _slowBindings)
        {
            binding.Apply((TSource)source, typedDestination, context);
        }

        InvokeCallbacks(_after, (TSource)source, typedDestination, context);
    }

    private void InvokeCallbacks(List<Delegate> callbacks, TSource source, TDestination destination, MappingContext? context)
    {
        foreach (var callback in callbacks)
        {
            if (callback is Action<TSource, TDestination> simple)
            {
                simple(source, destination);
            }
            else
            {
                ((Action<TSource, TDestination, IMapper>)callback)(source, destination, context!.Mapper);
            }
        }
    }
}
