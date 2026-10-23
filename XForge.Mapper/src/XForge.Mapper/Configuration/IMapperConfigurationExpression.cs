namespace XForge.Mapper;

/// <summary>
/// Fluent configuration DSL: obtained in <c>new MapperConfiguration(cfg => { ... })</c>.
/// </summary>
public interface IMapperConfigurationExpression
{
    /// <summary>
    /// Creates a map between two types: <c>cfg.CreateMap&lt;Pessoa, PessoaDto&gt;()...</c>.
    /// Members with matching names (case-insensitive) map automatically; use the returned
    /// <see cref="MappingExpression{TSource, TDestination}"/> for overrides.
    /// </summary>
    MappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        where TSource : notnull
        where TDestination : notnull;

    /// <summary>Non-generic map creation (runtime types / open scenarios).</summary>
    void CreateMap(Type sourceType, Type destinationType);

    /// <summary>
    /// When <see langword="true"/> (default: <see langword="false"/>), <see cref="MapperConfiguration.Validate"/>
    /// reports every destination member that has no matching source member or explicit <c>Map</c>/<c>Ignore</c>.
    /// </summary>
    bool RequireDestinationMemberSource { get; set; }
}

internal sealed class MapperConfigurationExpression : IMapperConfigurationExpression
{
    internal List<Internal.MapDefinition> Maps { get; set; } = new();

    internal MapperConfiguration? Configuration { get; set; }

    public bool RequireDestinationMemberSource { get; set; }

    public MappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        where TSource : notnull
        where TDestination : notnull
    {
        var definition = new Internal.MapDefinition(typeof(TSource), typeof(TDestination)) { Owner = this };
        Maps.Add(definition);
        return new MappingExpression<TSource, TDestination>(definition);
    }

    public void CreateMap(Type sourceType, Type destinationType)
    {
        if (sourceType is null) throw new ArgumentNullException(nameof(sourceType));
        if (destinationType is null) throw new ArgumentNullException(nameof(destinationType));
        Maps.Add(new Internal.MapDefinition(sourceType, destinationType) { Owner = this });
    }
}
