using System.Linq.Expressions;
using System.Reflection;

namespace XForge.Mapper.Internal;

/// <summary>Helpers compartilhados entre o caminho inline e o clássico do PlanBuilder.</summary>
internal static partial class PlanBuilder
{
    // ------------------------------------------------------------------
    // Overrides / resolução de membros
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
    // Construtores / conversão de argumentos
    // ------------------------------------------------------------------

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

    /// <summary>Conversão de argumento de construtor — paridade com <c>ResolveFactory</c> clássico.</summary>
    private static Expression ConvertCtorArgument(Expression access, Type sourceType, Type targetType,
        MapperConfiguration configuration, List<string> errors, string contextDescription)
    {
        if (sourceType == targetType)
        {
            return access;
        }

        var converter = ConverterFactory.TryCreate(sourceType, targetType, configuration, out _);

        if (converter is not null)
        {
            return Expression.Invoke(Expression.Constant(converter), access);
        }

        try
        {
            return Expression.Convert(access, targetType);
        }
        catch (Exception exception)
        {
            errors.Add($"Cannot convert {sourceType} to {targetType} for {contextDescription}: {exception.Message}");
            return Expression.Constant(targetType.IsValueType ? Activator.CreateInstance(targetType) : null, targetType);
        }
    }

    // ------------------------------------------------------------------
    // Classificação de tipos
    // ------------------------------------------------------------------

    private static bool IsMappableClass(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return !effective.IsPrimitive && effective != typeof(decimal) && effective != typeof(string) &&
               effective != typeof(DateTime) && effective != typeof(DateTimeOffset) && effective != typeof(Guid) &&
               effective != typeof(TimeSpan) && effective != typeof(object) && !effective.IsEnum;
    }

    /// <summary>Qualquer coleção (genérica, não genérica, array ou dicionário) — usada para impedir recursão de inlining em pares de coleções aninhadas.</summary>
    private static bool IsEnumerableType(Type type)
    {
        return type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
    }

    private static Type? GetEnumerableElementType(Type type)
    {
        if (type == typeof(string))
        {
            return null;
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
                return null; // dicionários: conversor dedicado no caminho lento
            }

            if (definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>) ||
                definition == typeof(IList<>) || definition == typeof(List<>) || definition == typeof(HashSet<>))
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

    private static (InlineCollectionKind Kind, Type ItemType)? GetInlineCollectionTarget(Type type)
    {
        if (type == typeof(string))
        {
            return null; // string implementa IEnumerable<char> — não é coleção de destino
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() == 1
                ? (InlineCollectionKind.ArrayKind, type.GetElementType()!)
                : ((InlineCollectionKind, Type)?)null;
        }

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();
            var arguments = type.GetGenericArguments();

            if (definition == typeof(List<>))
            {
                return (InlineCollectionKind.ListKind, arguments[0]);
            }

            if (definition == typeof(HashSet<>))
            {
                return (InlineCollectionKind.HashSetKind, arguments[0]);
            }

            if (definition == typeof(Dictionary<,>))
            {
                return null; // dicionários: caminho lento (v0.1 parity)
            }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (!interfaceType.IsGenericType)
            {
                continue;
            }

            var definition = interfaceType.GetGenericTypeDefinition();

            if (definition == typeof(IEnumerable<>) || definition == typeof(ICollection<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
            {
                return (InlineCollectionKind.InterfaceKind, interfaceType.GetGenericArguments()[0]);
            }
        }

        return null;
    }

    /// <summary>
    /// Mapas aninhados com callbacks, MaxDepth ou ConstructUsing exigem execução com contexto
    /// (o clássico resolve via <c>GetOrSynthesizePlan</c>) — não são inlináveis.
    /// </summary>
    private static bool NestedDefinitionRequiresClassicPath(MapDefinition definition, MapperConfiguration configuration)
    {
        if (definition.ConstructUsing is not null || definition.MaxDepth is not null)
        {
            return true;
        }

        if (definition.BeforeMap.Count > 0 || definition.AfterMap.Count > 0)
        {
            return true;
        }

        foreach (var (baseSource, baseDestination) in definition.IncludeBases)
        {
            var baseDefinition = configuration.FindDefinition(baseSource, baseDestination);

            if (baseDefinition is not null &&
                (baseDefinition.BeforeMap.Count > 0 || baseDefinition.AfterMap.Count > 0 ||
                 baseDefinition.ConstructUsing is not null || baseDefinition.MaxDepth is not null))
            {
                return true;
            }
        }

        return false;
    }
}
