namespace XForge.Mapper;

/// <summary>
/// The mapper facade. Obtain via <see cref="MapperConfiguration.CreateMapper"/>,
/// via <c>services.AddXForgeMapper(...)</c> or the static default
/// (see <see cref="MappingExtensions"/> parameterless overloads).
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps <paramref name="source"/> to <typeparamref name="TDestination"/> using the runtime source type
    /// (polymorphic: walks the source hierarchy to find the closest registered map).
    /// </summary>
    TDestination Map<TDestination>(object? source);

    /// <summary>Maps a strongly-typed source to a new <typeparamref name="TDestination"/>.</summary>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps <paramref name="source"/> onto an <paramref name="existing"/> instance (merge/patch:
    /// only members with bindings are overwritten; Before/After callbacks run).
    /// </summary>
    void Map<TSource, TDestination>(TSource source, TDestination existing);

    /// <summary>Maps a strongly-typed source collection to a new <see cref="List{TDestination}"/>.</summary>
    List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source);

    /// <summary>
    /// Maps a non-generic source collection to a new <see cref="List{TDestination}"/> using the runtime
    /// item type (polymorphic dispatch per item).
    /// </summary>
    List<TDestination> MapList<TDestination>(System.Collections.IEnumerable source);

    /// <summary>The configuration backing this mapper.</summary>
    MapperConfiguration Configuration { get; }
}
