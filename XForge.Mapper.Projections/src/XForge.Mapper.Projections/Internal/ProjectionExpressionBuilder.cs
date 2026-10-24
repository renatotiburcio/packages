using System.Linq.Expressions;
using System.Reflection;
using XForge.Mapper;
using XForge.Mapper.Internal;

namespace XForge.Mapper.Projections.Internal;

/// <summary>
/// Builds Func of (TSource → TDestination) projection expressions from the fluent configuration:
/// convention members, nested objects (recursive MemberInit), collections (Select + ToList/ToArray),
/// user expressions (Map) and constructor-bound immutable destinations (records).
/// </summary>
internal static class ProjectionExpressionBuilder
{
    public static LambdaExpression Build(Type sourceType, Type destinationType, MapperConfiguration configuration)
    {
        var definition = configuration.FindDefinition(sourceType, destinationType) ?? new MapDefinition(sourceType, destinationType);
        return BuildCore(definition, configuration);
    }

    private static LambdaExpression BuildCore(MapDefinition definition, MapperConfiguration configuration)
    {
        var sourceMembers = ReflectionHelper.GetReadableMembers(definition.SourceType);
        var parameter = Expression.Parameter(definition.SourceType, "s");
        var body = BuildDestinationExpression(definition, parameter, sourceMembers, overrides: CollectOverrides(definition, configuration), configuration);

        var lambdaType = typeof(Func<,>).MakeGenericType(definition.SourceType, definition.DestinationType);
        return Expression.Lambda(lambdaType, body, parameter);
    }

    private static Expression BuildDestinationExpression(MapDefinition definition, Expression sourceParameter,
        List<MemberInfo> sourceMembers, List<MemberOverride> overrides, MapperConfiguration configuration)
    {
        var destinationType = definition.DestinationType;
        var destinationMembers = ReflectionHelper.GetWritableMembers(destinationType);
        var constructor = ResolveConstructor(destinationType);
        var constructorParameters = constructor?.GetParameters() ?? Array.Empty<ParameterInfo>();

        var memberBindings = new List<MemberBinding>();
        var ctorArguments = new Expression[constructorParameters.Length];

        foreach (var destinationMember in destinationMembers)
        {
            var memberOverride = FindOverride(overrides, destinationMember.Name);

            if (memberOverride is not null && memberOverride.Ignore && memberOverride.IgnoreCondition is null)
            {
                continue;
            }

            var valueExpression = BuildMemberValueExpression(
                destinationMember, memberOverride, sourceMembers, sourceParameter, configuration);

            if (valueExpression is null)
            {
                continue; // sem fonte e sem override → membro fica default na projeção
            }

            var memberType = destinationMember.GetMemberType();

            if (valueExpression.Type != memberType)
            {
                valueExpression = Expression.Convert(valueExpression, memberType);
            }

            memberBindings.Add(Expression.Bind(destinationMember, valueExpression));
        }

        // ----- argumentos de construtor não cobertos por membros (records/imutáveis) -----
        var memberNames = new HashSet<string>(destinationMembers.Select(m => m.Name), StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < constructorParameters.Length; i++)
        {
            var parameter = constructorParameters[i];

            if (memberNames.Contains(parameter.Name!, StringComparer.OrdinalIgnoreCase))
            {
                continue; // já projetado como membro
            }

            var memberOverride = FindOverride(overrides, parameter.Name!);
            Expression? valueExpression = memberOverride is not null && memberOverride.SourceExpression is not null
                ? RebindOverride(memberOverride.SourceExpression, parameter.ParameterType, sourceParameter)
                : BuildConventionValue(sourceMembers, parameter.Name!, parameter.ParameterType, sourceParameter);

            if (valueExpression is null)
            {
                valueExpression = parameter.HasDefaultValue
                    ? Expression.Constant(parameter.DefaultValue, parameter.ParameterType)
                    : Expression.Constant(parameter.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null, parameter.ParameterType);
            }

            if (valueExpression.Type != parameter.ParameterType)
            {
                valueExpression = Expression.Convert(valueExpression, parameter.ParameterType);
            }

            ctorArguments[i] = valueExpression;
        }

        NewExpression newInstance = constructorParameters.Length == 0
            ? Expression.New(constructor ?? throw new MappingException(
                $"Destination '{destinationType}' has no accessible constructor for projection."))
            : Expression.New(constructor!, ctorArguments);

        return memberBindings.Count > 0
            ? Expression.MemberInit(newInstance, memberBindings)
            : newInstance;
    }

    private static Expression? BuildMemberValueExpression(
        MemberInfo destinationMember, MemberOverride? memberOverride,
        List<MemberInfo> sourceMembers, Expression sourceParameter, MapperConfiguration configuration)
    {
        var destinationMemberType = destinationMember.GetMemberType();

        // ----- explicit Map (expressão do usuário, rebinada ao parâmetro da projeção) -----
        if (memberOverride is not null && memberOverride.SourceExpression is not null)
        {
            return RebindOverride(memberOverride.SourceExpression, destinationMemberType, sourceParameter);
        }

        if (memberOverride is not null && memberOverride.SourceValue is not null)
        {
            throw new MappingException(
                $"Member '{destinationMember.Name}' was mapped with MapWith (a plain delegate), which cannot be " +
                "translated to a projection expression. Use Map with an expression instead.");
        }

        if (memberOverride is not null && memberOverride.IgnoreCondition is not null)
        {
            throw new MappingException(
                $"Member '{destinationMember.Name}' uses IgnoreIf, which has no projection translation " +
                "(IQueryable members must map unconditionally).");
        }

        // ----- convention: source member com o mesmo nome (case-insensitive) -----
        var sourceMember = FindSourceMember(sourceMembers, destinationMember.Name);

        if (sourceMember is null)
        {
            return null;
        }

        var sourceValueType = sourceMember.GetMemberType();
        var sourceAccess = BuildSourceAccess(sourceMember, sourceParameter);

        // ----- collection → collection (Select com projeção de elemento + null-guard) -----
        // (verificar ANTES do nested: List<T> passa em IsMappableClass)
        var sourceItem = GetEnumerableElementType(sourceValueType);
        var destinationItem = GetCollectionElementType(destinationMemberType);

        if (sourceItem is not null && destinationItem is not null)
        {
            var collectionProjection = BuildCollectionProjection(
                sourceValueType, destinationMemberType, sourceItem, destinationItem, sourceAccess, configuration);

            var nullCollection = Expression.Constant(null, destinationMemberType);
            return Expression.Condition(
                Expression.Equal(sourceAccess, Expression.Constant(null, sourceValueType)),
                nullCollection,
                collectionProjection);
        }

        // ----- nested class → class (MemberInit recursivo com null-guard) -----
        if (IsMappableClass(sourceValueType) && IsMappableClass(destinationMemberType))
        {
            var nestedLambda = Build(sourceValueType, destinationMemberType, configuration);
            var nestedBody = ParameterReplacer.Replace(nestedLambda.Body, (ParameterExpression)nestedLambda.Parameters[0], sourceAccess);

            // null-guard: EF propaga null; LINQ-to-Objects precisa do teste explícito
            var nullDestination = Expression.Constant(null, destinationMemberType);
            return Expression.Condition(
                Expression.Equal(sourceAccess, Expression.Constant(null, sourceValueType)),
                nullDestination,
                nestedBody);
        }

        // ----- mesmo tipo / conversões simples (Translate-friendly: Expression.Convert) -----
        if (sourceValueType != destinationMemberType)
        {
            try
            {
                return Expression.Convert(sourceAccess, destinationMemberType);
            }
            catch (Exception)
            {
                return null; // conversão não projetável → membro fica default
            }
        }

        return sourceAccess;
    }

    private static Expression BuildCollectionProjection(Type sourceCollectionType, Type destinationCollectionType,
        Type sourceItem, Type destinationItem, Expression sourceAccess, MapperConfiguration configuration)
    {
        var itemLambda = Build(sourceItem, destinationItem, configuration);
        var itemParameter = (ParameterExpression)itemLambda.Parameters[0];
        var loopVariable = Expression.Parameter(sourceItem, "i");
        var itemBody = ParameterReplacer.Replace(itemLambda.Body, itemParameter, loopVariable);
        var typedItemLambda = Expression.Lambda(itemBody, loopVariable);

        var selectCall = Expression.Call(
            typeof(Enumerable), nameof(Enumerable.Select),
            new[] { sourceItem, destinationItem },
            sourceAccess, typedItemLambda);

        // materialização conforme o tipo de destino
        if (destinationCollectionType.IsArray)
        {
            return Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray),
                new[] { destinationItem }, selectCall);
        }

        if (destinationCollectionType.IsGenericType)
        {
            var definition = destinationCollectionType.GetGenericTypeDefinition();

            if (definition == typeof(List<>) || definition == typeof(HashSet<>) ||
                definition == typeof(ICollection<>) || definition == typeof(IReadOnlyCollection<>) ||
                definition == typeof(IReadOnlyList<>) || definition == typeof(IEnumerable<>))
            {
                if (definition == typeof(HashSet<>))
                {
                    var enumerableType = typeof(IEnumerable<>).MakeGenericType(destinationItem);
                    var ctor = destinationCollectionType.GetConstructor(new[] { enumerableType });

                    if (ctor is not null)
                    {
                        return Expression.New(ctor, selectCall);
                    }
                }

                var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(destinationItem);
                return Expression.Call(toListMethod, selectCall);
            }
        }

        foreach (var interfaceType in destinationCollectionType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return selectCall;
            }
        }

        throw new MappingException(
            $"Cannot project collection of type '{destinationCollectionType}'. Supported: List<T>, arrays, HashSet<T>, " +
            "IReadOnlyList<T>/IReadOnlyCollection<T>/ICollection<T>/IEnumerable<T>.");
    }

    private static Expression? BuildConventionValue(List<MemberInfo> sourceMembers, string name, Type targetType,
        Expression sourceParameter)
    {
        var sourceMember = FindSourceMember(sourceMembers, name);

        if (sourceMember is null)
        {
            return null;
        }

        var access = BuildSourceAccess(sourceMember, sourceParameter);

        if (sourceMember.GetMemberType() == targetType)
        {
            return access;
        }

        try
        {
            return Expression.Convert(access, targetType);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Expression RebindOverride(LambdaExpression sourceExpression, Type destinationMemberType,
        Expression sourceParameter)
    {
        var originalParameter = (ParameterExpression)sourceExpression.Parameters[0];
        var replaced = ParameterReplacer.Replace(sourceExpression.Body, originalParameter, sourceParameter);

        if (replaced.Type != destinationMemberType)
        {
            replaced = Expression.Convert(replaced, destinationMemberType);
        }

        return replaced;
    }

    private static Expression BuildSourceAccess(MemberInfo sourceMember, Expression sourceParameter)
    {
        return sourceMember switch
        {
            PropertyInfo property => Expression.Property(sourceParameter, property),
            FieldInfo field => Expression.Field(sourceParameter, field),
            _ => throw new InvalidOperationException($"Member '{sourceMember.Name}' is not a property or field."),
        };
    }

    private static ConstructorInfo? ResolveConstructor(Type destinationType)
    {
        var parameterless = destinationType.GetConstructor(Type.EmptyTypes);

        if (parameterless is not null && parameterless.IsPublic)
        {
            return parameterless;
        }

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

        return bestScore >= 0 ? best : null;
    }

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
                found = candidate;
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

    private static Type? GetEnumerableElementType(Type type)
    {
        if (type == typeof(string))
        {
            return null; // string é tipo folha, não coleção de char
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() == 1 ? type.GetElementType() : null;
        }

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(Dictionary<,>))
            {
                return null; // dicionários não são projetáveis em v0.1
            }

            if (definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>) ||
                definition == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }

        return null;
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (type == typeof(string))
        {
            return null; // string é tipo folha, não coleção de char
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() == 1 ? type.GetElementType() : null;
        }

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(Dictionary<,>))
            {
                return null; // dicionários não são projetáveis em v0.1
            }

            if (definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>) ||
                definition == typeof(List<>) || definition == typeof(HashSet<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }

        return null;
    }

    private static bool IsMappableClass(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return !effective.IsPrimitive && effective != typeof(decimal) && effective != typeof(string) &&
               effective != typeof(DateTime) && effective != typeof(DateTimeOffset) && effective != typeof(Guid) &&
               effective != typeof(TimeSpan) && effective != typeof(object) && !effective.IsEnum;
    }
}
