using Microsoft.CodeAnalysis;

namespace XForge.Mapper.Generators;

/// <summary>
/// Plano Roslyn do mapeamento: espelho das convencoes do motor de runtime
/// (nomes case-insensitive, propriedades/campos, aninhados MemberInit, colecoes com
/// materializador pre-dimensionado). v0.1: escalares de tipo identico, enum→enum (cast),
/// enum→string (ToString), aninhados classe→classe e colecoes com elementos suportados.
/// Membros sem contraparte ou de tipos incompativeis sao reportados como diagnostico
/// (XFG002/XFG003) e ignorados — mesma postura do motor de runtime.
/// </summary>
internal sealed partial class MappingPlanner
{
    private const string ListDefinition = "System.Collections.Generic.List<T>";

    public MappingPlanner(ITypeSymbol source, ITypeSymbol destination)
    {
        Source = source;
        Destination = destination;
    }

    public ITypeSymbol Source { get; }
    public ITypeSymbol Destination { get; }

    /// <summary>Membros writable do destino: propriedades com setter e campos publicos.</summary>
    public List<ISymbol> DestinationMembers()
    {
        var members = new List<ISymbol>();

        foreach (var member in Destination.GetMembers())
        {
            if (member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            if (member is IPropertySymbol { GetMethod.IsStatic: false, SetMethod: not null } property &&
                property.Parameters.Length == 0)
            {
                members.Add(property);
            }
            else if (member is IFieldSymbol { IsReadOnly: false, IsConst: false, IsStatic: false } field)
            {
                members.Add(field);
            }
        }

        return members;
    }

    /// <summary>Membros readable da origem: propriedades e campos publicos.</summary>
    public List<ISymbol> SourceMembers()
    {
        var members = new List<ISymbol>();

        foreach (var member in Source.GetMembers())
        {
            if (member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            if (member is IPropertySymbol { GetMethod.IsStatic: false, GetMethod: not null } property &&
                property.Parameters.Length == 0)
            {
                members.Add(property);
            }
            else if (member is IFieldSymbol { IsConst: false, IsStatic: false } field)
            {
                members.Add(field);
            }
        }

        return members;
    }

    /// <summary>Busca por nome exato e depois case-insensitive (mesma ordem do motor).</summary>
    public ISymbol? FindSourceMember(string name)
    {
        ISymbol? insensitive = null;

        foreach (var member in SourceMembers())
        {
            if (member.Name == name)
            {
                return member;
            }

            if (insensitive is null && string.Equals(member.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                insensitive = member;
            }
        }

        return insensitive;
    }

    public static ITypeSymbol MemberType(ISymbol member)
    {
        return member switch
        {
            IPropertySymbol property => property.Type,
            IFieldSymbol field => field.Type,
            _ => throw new ArgumentException($"Member '{member.Name}' is not a property or field.", nameof(member)),
        };
    }

    public static bool IsMappableClass(ITypeSymbol type)
    {
        var effective = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
            ? ((INamedTypeSymbol)type).TypeArguments[0]
            : type;

        return effective.TypeKind == TypeKind.Class &&
               effective.SpecialType != SpecialType.System_String &&
               effective.SpecialType != SpecialType.System_Object;
    }

    /// <summary>
    /// Par suportado em v0.1: tipos identicos, enum→enum (cast subjacente), enum→string,
    /// classe→classe e colecoes com elemento suportado. <paramref name="reason"/> descreve o
    /// caso incompativel para o diagnostico.
    /// </summary>
    public static bool IsSupportedPair(ITypeSymbol sourceType, ITypeSymbol destinationType, out string? reason)
    {
        if (SymbolEqualityComparer.Default.Equals(sourceType, destinationType))
        {
            reason = null;
            return true;
        }

        if (sourceType.TypeKind == TypeKind.Enum &&
            (destinationType.TypeKind == TypeKind.Enum ||
             destinationType.SpecialType == SpecialType.System_String))
        {
            reason = null;
            return true;
        }

        if (IsMappableClass(sourceType) && IsMappableClass(destinationType))
        {
            reason = null;
            return true;
        }

        var sourceElement = TryGetElementType(sourceType);
        var destinationElement = TryGetElementType(destinationType);

        if (sourceElement is not null && destinationElement is not null &&
            IsSupportedPair(sourceElement, destinationElement, out _))
        {
            reason = null;
            return true;
        }

        reason = $"{sourceType.ToDisplayString()} → {destinationType.ToDisplayString()}";
        return false;
    }

    /// <summary>Elemento de colecoes de origem (List, IReadOnlyList, ICollection, IEnumerable, array).</summary>
    public static ITypeSymbol? TryGetElementType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol { Rank: 1 } array)
        {
            return array.ElementType;
        }

        if (type is INamedTypeSymbol { IsGenericType: true } named)
        {
            var fullName = named.OriginalDefinition.ToDisplayString();
            return fullName is ListDefinition
                or "System.Collections.Generic.IReadOnlyList<T>"
                or "System.Collections.Generic.IReadOnlyCollection<T>"
                or "System.Collections.Generic.ICollection<T>"
                or "System.Collections.Generic.IEnumerable<T>"
                    ? named.TypeArguments[0]
                    : null;
        }

        return null;
    }

    /// <summary>Elemento de destino materializavel em v0.1: List&lt;T&gt; ou array T[].</summary>
    public static ITypeSymbol? TryGetDestinationElement(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol { Rank: 1 } array)
        {
            return array.ElementType;
        }

        if (type is INamedTypeSymbol { IsGenericType: true } named &&
            named.OriginalDefinition.ToDisplayString() == ListDefinition)
        {
            return named.TypeArguments[0];
        }

        return null;
    }
}
