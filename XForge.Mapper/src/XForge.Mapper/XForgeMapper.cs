using XForge.Mapper.Internal;

namespace XForge.Mapper;

/// <summary>
/// The default <see cref="IMapper"/> implementation. Mapping plans are built once per type pair
/// (reflection + compiled typed delegates at build time only) and cached in the configuration,
/// so the steady-state hot path is a dictionary lookup plus typed delegate invocations.
/// </summary>
public sealed class XForgeMapper : IMapper
{
    private readonly MapperConfiguration _configuration;

    /// <summary>Creates the mapper bound to a configuration.</summary>
    public XForgeMapper(MapperConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public MapperConfiguration Configuration => _configuration;

    /// <inheritdoc />
    public TDestination Map<TDestination>(object? source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var plan = _configuration.ResolvePlan(source.GetType(), typeof(TDestination));
        return (TDestination)plan.MapObject(source, null, CreateContext(plan))!;
    }

    /// <inheritdoc />
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var plan = _configuration.GetOrSynthesizePlan(typeof(TSource), typeof(TDestination));
        return (TDestination)plan.MapObject(source, null, CreateContext(plan))!;
    }

    /// <inheritdoc />
    public void Map<TSource, TDestination>(TSource source, TDestination existing)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (existing is null)
        {
            throw new ArgumentNullException(nameof(existing));
        }

        var plan = _configuration.GetOrSynthesizePlan(typeof(TSource), typeof(TDestination));
        plan.MapOntoObject(source, existing, CreateContext(plan));
    }

    /// <inheritdoc />
    public List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var plan = _configuration.GetOrSynthesizePlan(typeof(TSource), typeof(TDestination));

        // fast path: planos puros materializam a lista inteira em UMA chamada (sem dispatch por elemento)
        if (plan is MappingPlan<TSource, TDestination> typedPlan)
        {
            var listMapper = typedPlan.GetListMapper();

            if (listMapper is not null)
            {
                return listMapper(source);
            }
        }

        // caminho por elemento: planos com callbacks/bindings lentos/contexto (semântica preservada)
        var context = CreateContext(plan);
        var result = new List<TDestination>();

        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            result.Add((TDestination)plan.MapObject(item, null, context)!);
        }

        return result;
    }

    /// <inheritdoc />
    public List<TDestination> MapList<TDestination>(System.Collections.IEnumerable source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var result = new List<TDestination>();

        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            result.Add(Map<TDestination>(item));
        }

        return result;
    }

    private MappingContext? CreateContext(IMappingPlan plan)
    {
        if (!plan.RequiresContext)
        {
            return null;
        }

        return new MappingContext(this) { MaxDepth = plan.MaxDepth ?? int.MaxValue };
    }
}
