using System.Linq.Expressions;

namespace XForge.Mapper.Internal;

/// <summary>A custom member binding accumulated by the fluent <c>Map</c>/<c>Ignore</c>/<c>IgnoreIf</c> DSL.</summary>
internal sealed class MemberOverride
{
    /// <summary>Destination member name (property or field).</summary>
    public string DestinationMemberName { get; set; } = string.Empty;

    /// <summary>Destination member type (when known via expression).</summary>
    public Type? DestinationMemberType { get; set; }

    /// <summary>Destination member expression tree (kept for reverse mapping).</summary>
    public LambdaExpression? DestinationExpression { get; set; }

    /// <summary>Compiled source value factory: <c>Func&lt;TSource, TValue&gt;</c> (boxed storage).</summary>
    public Delegate? SourceValue { get; set; }

    /// <summary>Source expression tree (kept for reverse mapping when it is a direct member chain).</summary>
    public LambdaExpression? SourceExpression { get; set; }

    /// <summary>When <see langword="true"/>, the member is excluded from mapping.</summary>
    public bool Ignore { get; set; }

    /// <summary>Conditional ignore: a Func of (TSource, bool) — the member is skipped when it returns <see langword="true"/>.</summary>
    public Delegate? IgnoreCondition { get; set; }
}

/// <summary>
/// Internal, mutable definition of one source→destination map, accumulated by the fluent DSL
/// (<see cref="MappingExpression{TSource, TDestination}"/>) and consumed by the mapping plan builder.
/// </summary>
internal sealed class MapDefinition
{
    public MapDefinition(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }

    public Type SourceType { get; }
    public Type DestinationType { get; }

    /// <summary>Custom factory for creating the destination instance (<c>Func&lt;TSource, TDestination&gt;</c>).</summary>
    public Delegate? ConstructUsing { get; set; }

    public List<MemberOverride> MemberOverrides { get; } = new();

    /// <summary>Pre-mapping callbacks: <c>Action&lt;TSource, TDestination&gt;</c> or <c>Action&lt;TSource, TDestination, IMapper&gt;</c>.</summary>
    public List<Delegate> BeforeMap { get; } = new();

    /// <summary>Post-mapping callbacks: <c>Action&lt;TSource, TDestination&gt;</c> or <c>Action&lt;TSource, TDestination, IMapper&gt;</c>.</summary>
    public List<Delegate> AfterMap { get; } = new();

    /// <summary>Derived (subclasses) maps registered via <c>Include</c>.</summary>
    public List<(Type Source, Type Destination)> Includes { get; } = new();

    /// <summary>Base maps inherited via <c>IncludeBase</c>.</summary>
    public List<(Type Source, Type Destination)> IncludeBases { get; } = new();

    /// <summary>Maximum mapping depth for nested objects (guards against runaway recursion).</summary>
    public int? MaxDepth { get; set; }

    /// <summary>The configuration expression that owns this definition (for <c>ReverseMap</c>/<c>TwoWays</c>).</summary>
    internal MapperConfigurationExpression? Owner { get; set; }

    public MemberOverride? FindOverride(string destinationMemberName)
    {
        foreach (var memberOverride in MemberOverrides)
        {
            if (string.Equals(memberOverride.DestinationMemberName, destinationMemberName, StringComparison.OrdinalIgnoreCase))
            {
                return memberOverride;
            }
        }

        return null;
    }
}
