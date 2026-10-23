using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>
/// Builds <see cref="MappingPlan{TSource, TDestination}"/>s from <see cref="MapDefinition"/>s:
/// destination factory (custom factory / parameterless ctor / constructor-parameter binding),
/// convention member matching (case-insensitive) and typed member bindings with converters.
/// </summary>
internal static class PlanBuilder
{
    private static readonly MethodInfo BuildGenericMethod =
        typeof(PlanBuilder).GetMethod(nameof(BuildGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo BuildSameTypeBindingMethod =
        typeof(PlanBuilder).GetMethod(nameof(BuildSameTypeBinding), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo BuildConverterBindingMethod =
        typeof(PlanBuilder).GetMethod(nameof(BuildConverterBinding), BindingFlags.NonPublic | BindingFlags.Static)!;

    public static IMappingPlan Build(MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
    {
        try
        {
            return (IMappingPlan)BuildGenericMethod
                .MakeGenericMethod(definition.SourceType, definition.DestinationType)
                .Invoke(null, new[] { (object)definition, (object)configuration, (object)requireDestinationMemberSource })!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private static IMappingPlan BuildGeneric<TSource, TDestination>(
        MapDefinition definition, MapperConfiguration configuration, bool requireDestinationMemberSource)
        where TSource : notnull
        where TDestination : notnull
    {
        var errors = new List<string>();
        var requiresContext = false;

        var sourceMembers = ReflectionHelper.GetReadableMembers(typeof(TSource));
        var destinationMembers = ReflectionHelper.GetWritableMembers(typeof(TDestination));
        var overrides = CollectOverrides(definition, configuration);

        // ----- destination factory -----
        var factory = ResolveFactory<TSource, TDestination>(definition, configuration, sourceMembers, errors);

        // ----- member bindings -----
        var bindings = new List<IMemberBinding<TSource, TDestination>>();
        var contextFlags = new List<bool>();

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

            IMemberBinding<TSource, TDestination>? binding;

            if (hasExplicitMap)
            {
                var overrideGetter = memberOverride!.SourceValue!;
                var overrideValueType = overrideGetter.GetType().GetGenericArguments()[1];

                if (overrideValueType == destinationMemberType)
                {
                    binding = (IMemberBinding<TSource, TDestination>?)BuildSameTypeBindingMethod
                        .MakeGenericMethod(typeof(TSource), typeof(TDestination), destinationMemberType)
                        .Invoke(null, new[] { (object)destinationMember.Name!, (object)destinationMember!, (object?)null!, (object)overrideGetter! });
                }
                else
                {
                    binding = (IMemberBinding<TSource, TDestination>?)BuildConverterBindingMethod
                        .MakeGenericMethod(typeof(TSource), typeof(TDestination), overrideValueType, destinationMemberType)
                        .Invoke(null, new[] { (object)destinationMember.Name!, (object)destinationMember!, (object?)null!, (object)overrideGetter!, (object)configuration!, (object)contextFlags! });
                }
            }
            else
            {
                var sourceValueType = conventionSourceMember!.GetMemberType();

                if (sourceValueType == destinationMemberType)
                {
                    binding = (IMemberBinding<TSource, TDestination>?)BuildSameTypeBindingMethod
                        .MakeGenericMethod(typeof(TSource), typeof(TDestination), destinationMemberType)
                        .Invoke(null, new[] { (object)destinationMember.Name!, (object)destinationMember!, (object)conventionSourceMember!, (object?)null! });
                }
                else
                {
                    binding = (IMemberBinding<TSource, TDestination>?)BuildConverterBindingMethod
                        .MakeGenericMethod(typeof(TSource), typeof(TDestination), sourceValueType, destinationMemberType)
                        .Invoke(null, new[] { (object)destinationMember.Name!, (object)destinationMember!, (object)conventionSourceMember!, (object?)null!, (object)configuration!, (object)contextFlags! });
                }
            }

            if (binding is null)
            {
                continue; // incompatible conversion — skipped silently (AutoMapper-like behavior)
            }

            if (memberOverride is not null && memberOverride.IgnoreCondition is not null)
            {
                var condition = (Func<TSource, bool>)memberOverride.IgnoreCondition;
                binding = new ConditionalBinding<TSource, TDestination>(condition, binding);
            }

            bindings.Add(binding);
        }

        if (errors.Count > 0)
        {
            throw new MappingValidationException(errors);
        }

        // ----- callbacks (base first, then own) -----
        var before = new List<Delegate>();
        var after = new List<Delegate>();

        foreach (var (baseSource, baseDestination) in definition.IncludeBases)
        {
            var baseDefinition = configuration.FindDefinition(baseSource, baseDestination);

            if (baseDefinition is not null)
            {
                before.AddRange(baseDefinition.BeforeMap);
                after.AddRange(baseDefinition.AfterMap);
            }
        }

        before.AddRange(definition.BeforeMap);
        after.AddRange(definition.AfterMap);

        if (before.Concat(after).Any(callback => callback is not Action<TSource, TDestination>))
        {
            requiresContext = true;
        }

        if (contextFlags.Any(static flag => flag))
        {
            requiresContext = true;
        }

        return new MappingPlan<TSource, TDestination>(factory, bindings, before, after, definition.MaxDepth, requiresContext);
    }

    // ------------------------------------------------------------------
    // Destination factory
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

            var memberAccess = Expression.PropertyOrField(sourceParameter, sourceMember.Name);
            arguments[i] = ConvertExpression(
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

    private static ConstructorInfo? ChooseParameterizedConstructor(Type destinationType)
    {
        ConstructorInfo? best = null;
        var bestScore = -1;

        foreach (var constructor in destinationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            var score = constructor.GetParameters().Length;
            if (score > bestScore)
            {
                best = constructor;
                bestScore = score;
            }
        }

        return bestScore > 0 ? best : null;
    }

    private static Expression ConvertExpression(Expression memberAccess, Type sourceType, Type targetType,
        MapperConfiguration configuration, List<string> errors, string contextDescription)
    {
        if (sourceType == targetType)
        {
            return memberAccess;
        }

        // nested objects: use a context-less nested converter delegate (constructor path has no mapping context)
        var converter = ConverterFactory.TryCreate(sourceType, targetType, configuration, out _);

        if (converter is not null)
        {
            return Expression.Invoke(Expression.Constant(converter), memberAccess);
        }

        try
        {
            return Expression.Convert(memberAccess, targetType);
        }
        catch (Exception exception)
        {
            errors.Add($"Cannot convert {sourceType} to {targetType} for {contextDescription}: {exception.Message}");
            return Expression.Constant(targetType.IsValueType ? Activator.CreateInstance(targetType) : null, targetType);
        }
    }

    // ------------------------------------------------------------------
    // Overrides / member resolution
    // ------------------------------------------------------------------

    private static List<MemberOverride> CollectOverrides(MapDefinition definition, MapperConfiguration configuration)
    {
        var overrides = new List<MemberOverride>();

        foreach (var (baseSource, baseDestination) in definition.IncludeBases)
        {
            var baseDefinition = configuration.FindDefinition(baseSource, baseDestination);

            if (baseDefinition is not null)
            {
                overrides.AddRange(CollectOverrides(baseDefinition, configuration));
            }
        }

        overrides.AddRange(definition.MemberOverrides);
        return overrides;
    }

    private static MemberOverride? FindOverride(List<MemberOverride> overrides, string destinationMemberName)
    {
        MemberOverride? found = null;

        foreach (var candidate in overrides)
        {
            if (string.Equals(candidate.DestinationMemberName, destinationMemberName, StringComparison.OrdinalIgnoreCase))
            {
                found = candidate; // last wins (derived overrides come after base overrides)
            }
        }

        return found;
    }

    private static MemberInfo? FindSourceMember(List<MemberInfo> sourceMembers, string name)
    {
        foreach (var member in sourceMembers)
        {
            if (member.Name == name)
            {
                return member;
            }
        }

        foreach (var member in sourceMembers)
        {
            if (string.Equals(member.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return member;
            }
        }

        return null;
    }

    // ------------------------------------------------------------------
    // Binding builders (invoked via MakeGenericMethod)
    // ------------------------------------------------------------------

    private static IMemberBinding<TSource, TDestination>? BuildSameTypeBinding<TSource, TDestination, TValue>(
        string destinationMemberName, MemberInfo destinationMember, MemberInfo? sourceMember, Delegate? overrideGetter)
    {
        var setter = ReflectionHelper.CreateSetter<TDestination, TValue>(destinationMember);
        var getter = overrideGetter is not null
            ? (Func<TSource, TValue>)overrideGetter
            : ReflectionHelper.CreateGetter<TSource, TValue>(sourceMember!);

        return new ValueBinding<TSource, TDestination, TValue, TValue>(destinationMemberName, getter, setter, IdentityTransform<TSource, TValue>());
    }

    private static IMemberBinding<TSource, TDestination>? BuildConverterBinding<TSource, TDestination, TSourceValue, TValue>(
        string destinationMemberName, MemberInfo destinationMember, MemberInfo sourceMember, Delegate? overrideGetter,
        MapperConfiguration configuration, List<bool> contextFlags)
    {
        var setter = ReflectionHelper.CreateSetter<TDestination, TValue>(destinationMember);
        var getter = overrideGetter is not null
            ? (Func<TSource, TSourceValue>)overrideGetter
            : ReflectionHelper.CreateGetter<TSource, TSourceValue>(sourceMember);

        var converter = ConverterFactory.TryCreate(typeof(TSourceValue), typeof(TValue), configuration, out var converterRequiresContext);
        contextFlags.Add(converterRequiresContext);

        if (converter is null)
        {
            return null; // incompatible — skipped silently (AutoMapper-like)
        }

        if (converterRequiresContext)
        {
            return new ValueBinding<TSource, TDestination, TSourceValue, TValue>(
                destinationMemberName, getter, setter,
                ConverterFactory.CreateNestedTransform<TSource, TSourceValue, TValue>(configuration));
        }

        var typedConverter = (Func<TSourceValue, TValue>)converter;
        return new ValueBinding<TSource, TDestination, TSourceValue, TValue>(
            destinationMemberName, getter, setter,
            (source, value, context) => typedConverter(value));
    }

    private static Func<TSource, TValue, MappingContext?, TValue> IdentityTransform<TSource, TValue>()
    {
        return static (source, value, context) => value;
    }
}
