using System.Collections.Concurrent;

namespace XForge.Mapper;

/// <summary>
/// Static default mapper (shared process-wide), used by the parameterless fluent extensions
/// (<c>source.MapTo&lt;TDestination&gt;()</c>). Convention + auto-nested mapping works with zero configuration;
/// for custom configuration prefer DI (see XForgeMapperServiceCollectionExtensions)
/// or your own <see cref="MapperConfiguration"/>.
/// </summary>
public static class XForgeMapperDefaults
{
    private static readonly ConcurrentDictionary<string, IMapper> Mappers = new();
    private static readonly MapperConfiguration DefaultConfiguration = new(static _ => { });

    /// <summary>The default configuration (empty — convention + auto-nested mapping).</summary>
    public static MapperConfiguration Configuration => DefaultConfiguration;

    /// <summary>The default <see cref="IMapper"/> (created lazily on first use).</summary>
    public static IMapper Mapper => Mappers.GetOrAdd("default", static _ => DefaultConfiguration.CreateMapper());

    /// <summary>Gets (creating when needed) a mapper for a named custom configuration — useful for multi-tenant setups.</summary>
    public static IMapper GetOrCreateMapper(string name, Action<IMapperConfigurationExpression> configure)
    {
        return Mappers.GetOrAdd(name, _ => MapperConfiguration.Create(configure).CreateMapper());
    }
}

/// <summary>Fluent usage extensions for mapping (the "MapTo" style).</summary>
public static class MappingExtensions
{
    /// <summary>Fluent mapping with an explicit mapper: <c>pessoa.MapTo&lt;PessoaDto&gt;(mapper)</c>.</summary>
    public static TDestination MapTo<TDestination>(this object? source, IMapper mapper)
        where TDestination : notnull
    {
        return mapper.Map<TDestination>(source!);
    }

    /// <summary>Fluent mapping with the static default mapper: <c>pessoa.MapTo&lt;PessoaDto&gt;()</c>.</summary>
    public static TDestination MapTo<TDestination>(this object source)
        where TDestination : notnull
    {
        return XForgeMapperDefaults.Mapper.Map<TDestination>(source);
    }

    /// <summary>Fluent merge onto an existing instance: <c>source.MapOver(dto, mapper)</c>.</summary>
    public static TDestination MapOver<TSource, TDestination>(this TSource source, TDestination destination, IMapper mapper)
        where TDestination : notnull
    {
        mapper.Map(source, destination);
        return destination;
    }

    /// <summary>Fluent merge onto an existing instance using the static default mapper.</summary>
    public static TDestination MapOver<TSource, TDestination>(this TSource source, TDestination destination)
        where TDestination : notnull
    {
        XForgeMapperDefaults.Mapper.Map(source, destination);
        return destination;
    }

    /// <summary>Fluent list mapping with an explicit mapper (see <see cref="IMapper.MapList{TSource, TDestination}"/>).</summary>
    public static List<TDestination> MapListTo<TDestination>(this System.Collections.IEnumerable source, IMapper mapper)
        where TDestination : notnull
    {
        return mapper.MapList<TDestination>(source);
    }

    /// <summary>Fluent list mapping using the static default mapper.</summary>
    public static List<TDestination> MapListTo<TDestination>(this System.Collections.IEnumerable source)
        where TDestination : notnull
    {
        return XForgeMapperDefaults.Mapper.MapList<TDestination>(source);
    }
}
