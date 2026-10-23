namespace XForge.Mapper.Internal;

/// <summary>
/// Per-top-level-map state: instance cache (circular references), depth tracking and mapper access.
/// Created only when the mapping plan requires it (zero allocation otherwise).
/// </summary>
internal sealed class MappingContext
{
    private Dictionary<(Type Source, Type Destination), Dictionary<object, object?>>? _instanceCache;

    public MappingContext(IMapper mapper)
    {
        Mapper = mapper;
    }

    /// <summary>The mapper that started the mapping (available to Before/After callbacks).</summary>
    public IMapper Mapper { get; }

    /// <summary>Current nested-object depth.</summary>
    public int Depth { get; internal set; }

    /// <summary>Maximum allowed depth (from <c>MaxDepth</c> on the active map).</summary>
    public int MaxDepth { get; internal set; } = int.MaxValue;

    public bool TryGetCreated(Type sourceType, Type destinationType, object source, out object? created)
    {
        created = null;

        if (_instanceCache is null)
        {
            return false;
        }

        if (_instanceCache.TryGetValue((sourceType, destinationType), out var cache) &&
            cache.TryGetValue(source, out var cached))
        {
            created = cached;
            return true;
        }

        return false;
    }

    public void CacheCreated(Type sourceType, Type destinationType, object source, object? created)
    {
        (_instanceCache ??= new Dictionary<(Type, Type), Dictionary<object, object?>>())
            .TryGetValue((sourceType, destinationType), out var cache);

        if (cache is null)
        {
            cache = new Dictionary<object, object?>(ReferenceEqualityComparer.Instance);
            _instanceCache[(sourceType, destinationType)] = cache;
        }

        cache[source] = created;
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        internal static readonly ReferenceEqualityComparer Instance = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}
