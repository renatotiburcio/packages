using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using XForge.Mapper.Internal;

namespace XForge.Mapper;

/// <summary>
/// Holds the mapping configuration (created via the fluent DSL) and builds/caches the compiled
/// mapping plans per type pair. Thread-safe for concurrent mapping after construction.
/// </summary>
public sealed class MapperConfiguration
{
    private readonly List<MapDefinition> _maps = new();
    private readonly bool _requireDestinationMemberSource;
    private readonly ConcurrentDictionary<(Type Source, Type Destination), IMappingPlan> _plans;
    private readonly Func<(Type Source, Type Destination), IMappingPlan> _planFactory;

    /// <summary>Creates the configuration: <c>new MapperConfiguration(cfg => cfg.CreateMap&lt;A,B&gt;()...)</c>.</summary>
    public MapperConfiguration(Action<IMapperConfigurationExpression> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        // the expression shares the configuration's map list (RegisterReverse works during configuration)
        var expression = new MapperConfigurationExpression { Configuration = this, Maps = _maps };
        configure(expression);
        _requireDestinationMemberSource = expression.RequireDestinationMemberSource;
        _plans = new ConcurrentDictionary<(Type, Type), IMappingPlan>();
        _planFactory = key => BuildPlan(key.Source, key.Destination);
    }

    /// <summary>Factory helper: <c>var config = MapperConfiguration.Create(cfg => ...);</c></summary>
    public static MapperConfiguration Create(Action<IMapperConfigurationExpression> configure)
    {
        return new MapperConfiguration(configure);
    }

    /// <summary>Creates an <see cref="IMapper"/> bound to this configuration.</summary>
    public IMapper CreateMapper()
    {
        return new XForgeMapper(this);
    }

    /// <summary>
    /// Builds every registered mapping plan and reports configuration problems
    /// (unmapped destination members when <c>RequireDestinationMemberSource</c> is enabled,
    /// unmappable constructors, invalid member selectors).
    /// </summary>
    /// <exception cref="MappingValidationException">When any problem is found.</exception>
    public void Validate()
    {
        var errors = new List<string>();

        foreach (var map in _maps)
        {
            try
            {
                GetPlan(map.SourceType, map.DestinationType);
            }
            catch (MappingValidationException exception)
            {
                errors.AddRange(exception.Errors);
            }
            catch (Exception exception)
            {
                errors.Add($"{map.SourceType} -> {map.DestinationType}: {exception.Message}");
            }
        }

        if (errors.Count > 0)
        {
            throw new MappingValidationException(errors);
        }
    }

    internal IMappingPlan GetPlan(Type sourceType, Type destinationType)
    {
        var key = (sourceType, destinationType);

        if (_plans.TryGetValue(key, out var cached))
        {
            return cached;
        }

        return _plans.GetOrAdd(key, _planFactory);
    }

    private IMappingPlan BuildPlan(Type sourceType, Type destinationType)
    {
        var definition = FindDefinition(sourceType, destinationType) ?? new MapDefinition(sourceType, destinationType);
        return PlanBuilder.Build(definition, this, _requireDestinationMemberSource);
    }

    /// <summary>
    /// Convention-first plan resolution (used by all dispatch paths): uses the explicit map when registered,
    /// otherwise synthesizes a convention-only plan. When nothing maps at all (no CreateMap AND zero
    /// convention-compatible members), a clear <see cref="MappingException"/> is thrown.
    /// </summary>
    internal IMappingPlan GetOrSynthesizePlan(Type sourceType, Type destinationType)
    {
        var key = (sourceType, destinationType);

        if (_plans.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var definition = FindDefinition(sourceType, destinationType) ?? new MapDefinition(sourceType, destinationType);
        var plan = PlanBuilder.Build(definition, this, requireDestinationMemberSource: false);

        if (definition.MemberOverrides.Count == 0 &&
            definition.ConstructUsing is null &&
            plan.BindingCount == 0 &&
            ReflectionHelper.GetWritableMembers(destinationType).Count > 0)
        {
            throw new MappingException(
                $"Missing map: no CreateMap registered for '{sourceType}' -> '{destinationType}' and no " +
                "convention-compatible members were found. Register a map with CreateMap (and use Map/Ignore " +
                "for members that cannot be mapped by name).");
        }

        _plans.TryAdd(key, plan);
        return plan;
    }

    /// <summary>Finds the best plan for a runtime source type (walks the source type hierarchy).</summary>
    internal IMappingPlan ResolvePlan(Type runtimeSourceType, Type destinationType)
    {
        var key = (runtimeSourceType, destinationType);

        if (_plans.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var best = FindBestDefinition(runtimeSourceType, destinationType) ?? new MapDefinition(runtimeSourceType, destinationType);
        var plan = PlanBuilder.Build(best, this, _requireDestinationMemberSource);

        if (best.MemberOverrides.Count == 0 &&
            best.ConstructUsing is null &&
            plan.BindingCount == 0 &&
            ReflectionHelper.GetWritableMembers(destinationType).Count > 0)
        {
            throw new MappingException(
                $"Missing map: no CreateMap registered for '{runtimeSourceType}' -> '{destinationType}' and no " +
                "convention-compatible members were found. Register a map with CreateMap (and use Map/Ignore " +
                "for members that cannot be mapped by name).");
        }

        _plans.TryAdd(key, plan);
        return plan;
    }

    private MapDefinition? FindBestDefinition(Type runtimeSourceType, Type destinationType)
    {
        MapDefinition? best = null;
        var bestSourceDistance = int.MaxValue;
        var bestDestinationDistance = int.MaxValue;

        foreach (var map in _maps)
        {
            // source must accept the runtime type; destination may be a DERIVED type of the requested one
            // (polymorphic: Include<Derived, DerivedDto> serves Base -> BaseDto requests)
            if (!map.SourceType.IsAssignableFrom(runtimeSourceType) ||
                !destinationType.IsAssignableFrom(map.DestinationType))
            {
                continue;
            }

            var sourceDistance = 0;
            var current = runtimeSourceType;

            while (current != map.SourceType && current is not null)
            {
                current = current.BaseType;
                sourceDistance++;
            }

            var destinationDistance = 0;
            current = map.DestinationType;

            while (current != destinationType && current is not null)
            {
                current = current.BaseType;
                destinationDistance++;
            }

            // lexicographic: closest source wins first, then closest destination
            if (sourceDistance < bestSourceDistance ||
                (sourceDistance == bestSourceDistance && destinationDistance < bestDestinationDistance))
            {
                best = map;
                bestSourceDistance = sourceDistance;
                bestDestinationDistance = destinationDistance;
            }
        }

        return best;
    }

    internal MapDefinition? FindDefinition(Type sourceType, Type destinationType)
    {
        foreach (var map in _maps)
        {
            if (map.SourceType == sourceType && map.DestinationType == destinationType)
            {
                return map;
            }
        }

        return null;
    }

    /// <summary>Registers the reverse map for <paramref name="forward"/> (used by <c>ReverseMap</c>/<c>TwoWays</c>).</summary>
    internal MapDefinition RegisterReverse(MapDefinition forward)
    {
        var reverse = new MapDefinition(forward.DestinationType, forward.SourceType);

        foreach (var memberOverride in forward.MemberOverrides)
        {
            // Reversal works when both sides are direct member selectors (d => d.X, s => s.Y):
            // the reversed destination member is the forward source member and vice-versa.
            var forwardSourceMember = TryGetDirectMember(memberOverride.SourceExpression);

            if (memberOverride.Ignore || memberOverride.SourceValue is null || forwardSourceMember is null)
            {
                continue; // computed lambdas / ignores are not reversed (documented limitation)
            }

            var forwardDestinationMember = TryGetDirectMember(memberOverride.DestinationExpression);

            if (forwardDestinationMember is null)
            {
                continue;
            }

            // reversed: destination = forward source member; source value = forward destination member
            var reversedParameter = memberOverride.SourceExpression!.Parameters[0];
            var reversedDestinationExpression = Expression.Lambda(
                memberOverride.SourceExpression.Body, reversedParameter);

            var reversedSourceParameter = (ParameterExpression)forwardDestinationMember.Value.memberExpression.Expression!;
            var reversedSourceExpression = Expression.Lambda(
                forwardDestinationMember.Value.memberExpression, reversedSourceParameter);

            var compiledSourceType = reversedSourceExpression.Parameters[0].Type;
            var compiledReturnType = ((LambdaExpression)reversedSourceExpression).ReturnType;
            var compiledType = typeof(Func<,>).MakeGenericType(compiledSourceType, compiledReturnType);
            var sourceValue = reversedSourceExpression.Compile();

            reverse.MemberOverrides.Add(new MemberOverride
            {
                DestinationMemberName = forwardSourceMember.Value.member.Name,
                DestinationMemberType = forwardSourceMember.Value.member.GetMemberType(),
                DestinationExpression = reversedDestinationExpression,
                SourceValue = (Delegate)sourceValue,
                SourceExpression = reversedSourceExpression,
            });
        }

        _maps.Add(reverse);
        return reverse;
    }

    private static (MemberExpression memberExpression, MemberInfo member)? TryGetDirectMember(LambdaExpression? expression)
    {
        if (expression is null)
        {
            return null;
        }

        var body = expression.Body;
        while (body.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.Quote)
        {
            body = ((UnaryExpression)body).Operand;
        }

        if (body is MemberExpression { Expression: ParameterExpression } memberExpression)
        {
            return (memberExpression, memberExpression.Member);
        }

        return null;
    }
}
