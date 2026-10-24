using System.Linq.Expressions;
using System.Reflection;
using XForge.Mapper.Internal;

namespace XForge.Mapper;

/// <summary>
/// Fluent builder for a source→destination mapping. Obtained from
/// <see cref="IMapperConfigurationExpression.CreateMap{TSource, TDestination}"/>.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public sealed class MappingExpression<TSource, TDestination>
    where TSource : notnull
    where TDestination : notnull
{
    private readonly MapDefinition _definition;

    internal MappingExpression(MapDefinition definition)
    {
        _definition = definition;
    }

    internal MapDefinition Definition => _definition;

    /// <summary>
    /// Maps a destination member from a source expression: <c>.Map(d => d.FullName, s => s.FirstName + " " + s.LastName)</c>.
    /// Also accepts direct member paths on both sides: <c>.Map(d => d.City, s => s.Address.City)</c>.
    /// </summary>
    /// <typeparam name="TMember">The destination member type.</typeparam>
    /// <param name="destinationMember">The destination member selector.</param>
    /// <param name="sourceMember">The source value selector (compiled once, reused on every map).</param>
    /// <returns>The same mapping expression (fluent).</returns>
    public MappingExpression<TSource, TDestination> Map<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Expression<Func<TSource, TMember>> sourceMember)
    {
        var destMember = GetDestinationMember(destinationMember);
        _definition.MemberOverrides.Add(new MemberOverride
        {
            DestinationMemberName = destMember.Name,
            DestinationMemberType = destMember.GetMemberType(),
            DestinationExpression = destinationMember,
            SourceValue = sourceMember.Compile(),
            SourceExpression = sourceMember,
        });
        return this;
    }

    /// <summary>
    /// Maps a destination member from a plain function: <c>.MapWith(d => d.Codigo, s => $"ID-{s.Id}")</c>.
    /// </summary>
    public MappingExpression<TSource, TDestination> MapWith<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Func<TSource, TMember> sourceValue)
    {
        var destMember = GetDestinationMember(destinationMember);
        _definition.MemberOverrides.Add(new MemberOverride
        {
            DestinationMemberName = destMember.Name,
            DestinationMemberType = destMember.GetMemberType(),
            DestinationExpression = destinationMember,
            SourceValue = sourceValue,
        });
        return this;
    }

    /// <summary>Excludes a destination member from mapping: <c>.Ignore(d => d.Id)</c>.</summary>
    public MappingExpression<TSource, TDestination> Ignore<TMember>(Expression<Func<TDestination, TMember>> destinationMember)
    {
        var destMember = GetDestinationMember(destinationMember);
        _definition.MemberOverrides.Add(new MemberOverride
        {
            DestinationMemberName = destMember.Name,
            DestinationMemberType = destMember.GetMemberType(),
            DestinationExpression = destinationMember,
            Ignore = true,
        });
        return this;
    }

    /// <summary>Excludes a destination member by name (non-generic scenario / open mapping).</summary>
    public MappingExpression<TSource, TDestination> Ignore(string destinationMemberName)
    {
        _definition.MemberOverrides.Add(new MemberOverride
        {
            DestinationMemberName = destinationMemberName,
            Ignore = true,
        });
        return this;
    }

    /// <summary>
    /// Conditionally excludes a destination member: when <paramref name="condition"/> returns
    /// <see langword="true"/> for the source, the member keeps its default value.
    /// </summary>
    public MappingExpression<TSource, TDestination> IgnoreIf<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Func<TSource, bool> condition)
    {
        var destMember = GetDestinationMember(destinationMember);
        _definition.MemberOverrides.Add(new MemberOverride
        {
            DestinationMemberName = destMember.Name,
            DestinationMemberType = destMember.GetMemberType(),
            DestinationExpression = destinationMember,
            Ignore = true,
            IgnoreCondition = condition,
        });
        return this;
    }

    /// <summary>
    /// Custom destination factory: <c>.ConstructUsing(s => new Order(s.Id))</c>.
    /// Required for immutable destinations (records / constructor-only types) with custom construction.
    /// </summary>
    public MappingExpression<TSource, TDestination> ConstructUsing(Func<TSource, TDestination> factory)
    {
        _definition.ConstructUsing = factory;
        return this;
    }

    /// <summary>Callback executed before member mapping: <c>.BeforeMap((s, d) => ...)</c>.</summary>
    public MappingExpression<TSource, TDestination> BeforeMap(Action<TSource, TDestination> action)
    {
        _definition.BeforeMap.Add(action);
        return this;
    }

    /// <summary>Callback executed after all member mapping: <c>.AfterMap((s, d) => ...)</c>.</summary>
    public MappingExpression<TSource, TDestination> AfterMap(Action<TSource, TDestination> action)
    {
        _definition.AfterMap.Add(action);
        return this;
    }

    /// <summary>Callback executed before member mapping with mapper access (for nested maps).</summary>
    public MappingExpression<TSource, TDestination> BeforeMap(Action<TSource, TDestination, IMapper> action)
    {
        _definition.BeforeMap.Add(action);
        return this;
    }

    /// <summary>Callback executed after all member mapping with mapper access (for nested maps).</summary>
    public MappingExpression<TSource, TDestination> AfterMap(Action<TSource, TDestination, IMapper> action)
    {
        _definition.AfterMap.Add(action);
        return this;
    }

    /// <summary>
    /// Registers a derived map used when the source runtime type matches:
    /// <c>.Include&lt;Employee, EmployeeDto&gt;()</c>.
    /// </summary>
    public MappingExpression<TSource, TDestination> Include<TDerivedSource, TDerivedDestination>()
        where TDerivedSource : TSource
        where TDerivedDestination : TDestination
    {
        _definition.Includes.Add((typeof(TDerivedSource), typeof(TDerivedDestination)));
        return this;
    }

    /// <summary>Inherits the member configuration of a base map (base bindings apply as fallback).</summary>
    public MappingExpression<TSource, TDestination> IncludeBase<TBaseSource, TBaseDestination>()
    {
        if (!typeof(TBaseSource).IsAssignableFrom(typeof(TSource)) ||
            !typeof(TBaseDestination).IsAssignableFrom(typeof(TDestination)))
        {
            throw new ArgumentException(
                $"IncludeBase requires '{typeof(TSource)}' to derive from '{typeof(TBaseSource)}' and " +
                $"'{typeof(TDestination)}' to derive from '{typeof(TBaseDestination)}'.");
        }

        _definition.IncludeBases.Add((typeof(TBaseSource), typeof(TBaseDestination)));
        return this;
    }

    /// <summary>Guards against runaway recursion on deep/cyclic object graphs.</summary>
    public MappingExpression<TSource, TDestination> MaxDepth(int depth)
    {
        _definition.MaxDepth = depth;
        return this;
    }

    /// <summary>
    /// Registers the reverse map (<c>TDestination → TSource</c>). Conventional members reverse automatically;
    /// explicit <c>Map</c> bindings reverse when the source selector is a direct member path.
    /// </summary>
    public MappingExpression<TDestination, TSource> ReverseMap()
    {
        var reverse = _definition.Owner!.Configuration!.RegisterReverse(_definition);
        return new MappingExpression<TDestination, TSource>(reverse);
    }

    /// <summary>Registers both directions of the mapping.</summary>
    public void TwoWays()
    {
        ReverseMap();
    }

    internal static MemberInfo GetDestinationMember<TMember>(Expression<Func<TDestination, TMember>> destinationMember)
    {
        var body = destinationMember.Body;
        while (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked ||
               body.NodeType == ExpressionType.Quote)
        {
            body = ((UnaryExpression)body).Operand;
        }

        if (body is MemberExpression memberExpression)
        {
            return memberExpression.Member;
        }

        throw new ArgumentException(
            $"Expression '{destinationMember}' must resolve to a destination property or field.", nameof(destinationMember));
    }
}
