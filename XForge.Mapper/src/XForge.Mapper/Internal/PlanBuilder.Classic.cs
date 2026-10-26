using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>
/// Caminho clássico (v0.1): bindings por membro — contexto-free compilados em um único
/// <c>fastAction</c>; contexto-dependentes (ciclos via cache de instâncias, IgnoreIf, MaxDepth,
/// callbacks com mapper) como dispatch por membro. Usado quando o inlining recursivo não é viável.
/// </summary>
internal static partial class PlanBuilder
{
    private static IMappingPlan BuildClassicPlan<TSource, TDestination>(
        MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
        where TSource : notnull
        where TDestination : notnull
    {
        var errors = new List<string>();

        var sourceMembers = ReflectionHelper.GetReadableMembers(typeof(TSource));
        var destinationMembers = ReflectionHelper.GetWritableMembers(typeof(TDestination));
        var overrides = CollectOverrides(definition, configuration);

        // ----- destination factory -----
        var factory = ResolveFactory<TSource, TDestination>(definition, configuration, sourceMembers, errors);

        // ----- member bindings: fast (compiled block) vs slow (context-dependent dispatch) -----
        var sourceParameter = Expression.Parameter(typeof(TSource), "source");
        var destinationParameter = Expression.Parameter(typeof(TDestination), "destination");
        var fastStatements = new List<Expression>();
        var slowBindings = new List<IMemberBinding<TSource, TDestination>>();
        var slowNeedsContext = false;

        foreach (var destinationMember in destinationMembers)
        {
            var destinationMemberType = destinationMember.GetMemberType();
            var memberOverride = FindOverride(overrides, destinationMember.Name);

            // pure ignore → member intentionally not mapped
            if (memberOverride is not null && memberOverride.Ignore && memberOverride.IgnoreCondition is null)
            {
                continue;
            }

            var hasExplicitMap = memberOverride is { SourceValue: not null };
            var conventionSourceMember = hasExplicitMap ? null : FindSourceMember(sourceMembers, destinationMember.Name);

            if (!hasExplicitMap && conventionSourceMember is null)
            {
                if (requireDestinationMemberSource)
                {
                    errors.Add($"Destination member '{destinationMember.Name}' on '{typeof(TDestination)}' has no matching " +
                               "source member and no Map/Ignore override (RequireDestinationMemberSource).");
                }

                continue;
            }

            var sourceValueType = hasExplicitMap
                ? memberOverride!.SourceValue!.GetType().GetGenericArguments()[1]
                : conventionSourceMember!.GetMemberType();

            var destinationAccess = MemberAccess(destinationParameter, destinationMember);

            // source access: convention member or user selector invoke
            Expression sourceAccess;

            if (hasExplicitMap)
            {
                sourceAccess = Expression.Invoke(
                    Expression.Constant(memberOverride!.SourceValue!), sourceParameter);

                if (sourceAccess.Type != sourceValueType)
                {
                    sourceAccess = Expression.Convert(sourceAccess, sourceValueType);
                }
            }
            else
            {
                sourceAccess = MemberAccess(sourceParameter, conventionSourceMember!);
            }

            // value conversion chain (context-free)
            Expression valueExpression = sourceAccess;

            if (sourceValueType != destinationMemberType)
            {
                var converter = ConverterFactory.TryCreate(sourceValueType, destinationMemberType, configuration, out var converterRequiresContext);

                // nested class → class: needs mapping context (cycles) → slow path
                if (converterRequiresContext)
                {
                    valueExpression = null!;
                }
                else if (converter is null)
                {
                    continue; // incompatible — skipped silently (AutoMapper-like behavior)
                }
                else
                {
                    var invokeArgument = sourceAccess.Type == sourceValueType
                        ? sourceAccess
                        : Expression.Convert(sourceAccess, sourceValueType);
                    valueExpression = Expression.Invoke(Expression.Constant(converter), invokeArgument);
                }
            }

            var isConditional = memberOverride is not null && memberOverride.IgnoreCondition is not null;
            var isFast = valueExpression is not null && !isConditional;

            if (isFast && valueExpression is not null)
            {
                if (valueExpression.Type != destinationMemberType)
                {
                    valueExpression = Expression.Convert(valueExpression, destinationMemberType);
                }

                fastStatements.Add(Expression.Assign(destinationAccess, valueExpression));
                continue;
            }

            // ----- slow path: conditional ignores and nested class→class (context-aware) -----
            var slowBinding = (IMemberBinding<TSource, TDestination>?)DispatchSlowBinding(
                typeof(TSource), typeof(TDestination), destinationMember.Name!, destinationMember,
                conventionSourceMember, memberOverride, sourceValueType, destinationMemberType, configuration);

            if (slowBinding is null)
            {
                continue;
            }

            if (isNestedClass(sourceValueType, destinationMemberType))
            {
                slowNeedsContext = true;
            }

            slowBindings.Add(slowBinding);
        }

        if (errors.Count > 0)
        {
            throw new MappingValidationException(errors);
        }

        // ----- compiled fast action (single delegate for all context-free assignments) -----
        Action<TSource, TDestination>? fastAction = null;

        if (fastStatements.Count > 0)
        {
            var block = Expression.Block(typeof(void), fastStatements);
            fastAction = Expression.Lambda<Action<TSource, TDestination>>(
                block, sourceParameter, destinationParameter).Compile();
        }

        // ----- callbacks (base first, then own) -----
        var (before, after) = CollectCallbacks(definition, configuration);

        var needsContextForCallbacks = before.Concat(after).Any(callback => callback is not Action<TSource, TDestination>);
        var requiresContext = slowNeedsContext || slowBindings.Count > 0 ||
                              definition.MaxDepth is not null || needsContextForCallbacks;

        return new MappingPlan<TSource, TDestination>(
            factory, null, fastAction, slowBindings, before, after, definition.MaxDepth, requiresContext);
    }

    // ------------------------------------------------------------------
    // Slow binding builder (conditional ignores, nested class→class with context)
    // ------------------------------------------------------------------

    private static IMemberBinding<TSource, TDestination>? BuildSlowBinding<TSource, TDestination, TSourceValue, TValue>(
        string destinationMemberName, MemberInfo destinationMember, MemberInfo? sourceMember,
        MemberOverride? memberOverride, MapperConfiguration configuration)
        where TSource : notnull
        where TDestination : notnull
    {
        var setter = ReflectionHelper.CreateSetter<TDestination, TValue>(destinationMember);
        IMemberBinding<TSource, TDestination>? inner = null;

        if (memberOverride is not null && memberOverride.SourceValue is not null)
        {
            // explicit user selector
            if (typeof(TSourceValue) != typeof(TValue))
            {
                return null;
            }

            var overrideGetter = (Func<TSource, TValue>)memberOverride.SourceValue;
            inner = new ValueBinding<TSource, TDestination, TValue, TValue>(
                destinationMemberName, overrideGetter, setter, IdentityTransform<TSource, TValue>());
        }
        else if (sourceMember is not null)
        {
            var actualSourceType = sourceMember.GetMemberType();
            var getter = ReflectionHelper.CreateGetter<TSource, TSourceValue>(sourceMember);

            if (actualSourceType == typeof(TValue))
            {
                // TSourceValue == TValue at runtime — same closed generic
                var identityGetter = (Func<TSource, TValue>)(object)getter;
                inner = new ValueBinding<TSource, TDestination, TValue, TValue>(
                    destinationMemberName, identityGetter, setter, IdentityTransform<TSource, TValue>());
            }
            else
            {
                var converter = ConverterFactory.TryCreate(actualSourceType, typeof(TValue), configuration, out var converterRequiresContext);

                if (converter is null)
                {
                    return null;
                }

                if (!converterRequiresContext)
                {
                    var typedConverter = (Func<TSourceValue, TValue>)converter;
                    inner = new ValueBinding<TSource, TDestination, TSourceValue, TValue>(
                        destinationMemberName, getter, setter, (source, value, context) => typedConverter(value));
                }
                else
                {
                    // nested class→class: context-aware resolver (cycles, MaxDepth)
                    var resolver = CreateNestedPlanResolver<TSourceValue, TValue>(configuration);
                    inner = new ValueBinding<TSource, TDestination, TSourceValue, TValue>(
                        destinationMemberName, getter, setter, (source, value, context) => resolver(value, context));
                }
            }
        }

        if (inner is null)
        {
            return null;
        }

        if (memberOverride is not null && memberOverride.IgnoreCondition is not null)
        {
            var condition = (Func<TSource, bool>)memberOverride.IgnoreCondition;
            return new ConditionalBinding<TSource, TDestination>(condition, inner);
        }

        return inner;
    }

    private static Func<TSource, TValue, MappingContext?, TValue> IdentityTransform<TSource, TValue>()
    {
        return static (source, value, context) => value;
    }

    /// <summary>Context-aware nested resolution: instance cache (cycles, cache-before-populate) + MaxDepth.</summary>
    private static Func<TSrcValue, MappingContext?, TDestValue> CreateNestedPlanResolver<TSrcValue, TDestValue>(
        MapperConfiguration configuration)
    {
        IMappingPlan? cachedPlan = null;

        return (value, context) =>
        {
            if (value is null)
            {
                return default!;
            }

            cachedPlan ??= configuration.GetOrSynthesizePlan(typeof(TSrcValue), typeof(TDestValue));
            var plan = cachedPlan;

            if (context is null)
            {
                return (TDestValue)plan.MapObject(value, null, null)!;
            }

            if (context.TryGetCreated(typeof(TSrcValue), typeof(TDestValue), value, out var existing))
            {
                return (TDestValue)existing!;
            }

            context.Depth++;

            if (context.Depth >= context.MaxDepth)
            {
                context.Depth--;
                return default!; // MaxDepth: nested levels beyond the limit stay at their default value
            }

            try
            {
                var destinationInstance = plan.CreateDestinationObject(value, context);
                context.CacheCreated(typeof(TSrcValue), typeof(TDestValue), value, destinationInstance);
                plan.PopulateObject(value, destinationInstance, context);
                return (TDestValue)destinationInstance;
            }
            finally
            {
                context.Depth--;
            }
        };
    }

    // ------------------------------------------------------------------
    // Destination factory (clássico)
    // ------------------------------------------------------------------

    private static Func<TSource, TDestination> ResolveFactory<TSource, TDestination>(
        MapDefinition definition, MapperConfiguration configuration, List<MemberInfo> sourceMembers, List<string> errors)
    {
        if (definition.ConstructUsing is Delegate customFactory)
        {
            return (Func<TSource, TDestination>)customFactory;
        }

        var destinationType = typeof(TDestination);

        var parameterless = destinationType.GetConstructor(Type.EmptyTypes);
        if (parameterless is not null && parameterless.IsPublic)
        {
            var emptySourceParameter = Expression.Parameter(typeof(TSource), "source");
            return Expression.Lambda<Func<TSource, TDestination>>(
                Expression.New(parameterless), emptySourceParameter).Compile();
        }

        var constructor = ChooseParameterizedConstructor(destinationType);

        if (constructor is null)
        {
            errors.Add($"Destination '{destinationType}' has no accessible constructor to create instances.");
            return source => throw new MappingException(
                $"Destination '{destinationType}' has no accessible constructor.");
        }

        var parameters = constructor.GetParameters();
        var arguments = new Expression[parameters.Length];
        var sourceParameter = Expression.Parameter(typeof(TSource), "source");

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var sourceMember = FindSourceMember(sourceMembers, parameter.Name!);

            if (sourceMember is null)
            {
                var defaultValue = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : Expression.Constant(parameter.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null, parameter.ParameterType);
                arguments[i] = defaultValue;
                continue;
            }

            var memberAccess = MemberAccess(sourceParameter, sourceMember);
            arguments[i] = ConvertCtorArgument(
                memberAccess, sourceMember.GetMemberType(), parameter.ParameterType, configuration, errors,
                $"constructor parameter '{parameter.Name}' of '{destinationType}'");
        }

        try
        {
            var body = Expression.New(constructor, arguments);
            return Expression.Lambda<Func<TSource, TDestination>>(body, sourceParameter).Compile();
        }
        catch (Exception exception)
        {
            errors.Add($"Destination '{destinationType}': cannot bind constructor '{constructor}': {exception.Message}");
            return source => throw new MappingException(
                $"Destination '{destinationType}' could not be constructed.");
        }
    }

    private static bool isNestedClass(Type sourceType, Type destinationType)
    {
        return IsMappableClass(Nullable.GetUnderlyingType(sourceType) ?? sourceType) &&
               IsMappableClass(Nullable.GetUnderlyingType(destinationType) ?? destinationType);
    }
}
