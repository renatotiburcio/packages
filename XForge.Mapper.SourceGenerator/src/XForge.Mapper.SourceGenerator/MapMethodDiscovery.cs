using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XForge.Mapper.Generators;

/// <summary>Forma do metodo parcial declarado na classe [XForgeMap].</summary>
internal enum MapMethodKind
{
    /// <summary>partial Dest Map(Src source) — cria E popula.</summary>
    Create,

    /// <summary>partial void Map(Src source, Dest destination) — popula instancia existente.</summary>
    Fill,

    /// <summary>partial List&lt;Dest&gt; Map(IEnumerable&lt;Src&gt; source) — lista inteira.</summary>
    CreateList,
}

/// <summary>Um metodo parcial descoberto pelo generator.</summary>
internal sealed class MapMethodModel
{
    public required string MethodName { get; init; }
    public required MapMethodKind Kind { get; init; }
    public required ITypeSymbol SourceType { get; init; }
    public required ITypeSymbol DestinationType { get; init; }
    public required MethodDeclarationSyntax Declaration { get; init; }
}

/// <summary>
/// Descobre metodos parciais em uma classe [XForgeMap] e classifica sua forma:
/// <c>Dest Map(Src)</c> (Create), <c>void Map(Src, Dest)</c> (Fill) ou
/// <c>List&lt;Dest&gt; Map(IEnumerable&lt;Src&gt;)</c> (CreateList).
/// Metodos parciais que nao correspondem a nenhuma forma sao reportados (XFG004).
/// </summary>
internal static class MapMethodDiscovery
{
    private const string ListDefinition = "System.Collections.Generic.List<T>";

    public static List<MapMethodModel> Discover(
        ClassDeclarationSyntax declaration, INamedTypeSymbol containing,
        Compilation compilation, SourceProductionContext context)
    {
        var methods = new List<MapMethodModel>();
        var semanticModel = compilation.GetSemanticModel(declaration.SyntaxTree);

        foreach (var member in declaration.Members)
        {
            if (member is not MethodDeclarationSyntax methodDeclaration)
            {
                continue;
            }

            var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;

            if (symbol is null)
            {
                continue;
            }

            var model = Classify(methodDeclaration, symbol, context);

            if (model is not null)
            {
                methods.Add(model);
            }
        }

        return methods;

        MapMethodModel? Classify(MethodDeclarationSyntax method, IMethodSymbol methodSymbol,
            SourceProductionContext productionContext)
        {
            var isPartial = methodSymbol.IsPartialDefinition ||
                            method.Modifiers.Any(token => token.IsKind(SyntaxKind.PartialKeyword));

            if (!isPartial)
            {
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnsupportedSignature, method.Identifier.GetLocation(),
                    methodSymbol.ToDisplayString()));
                return null;
            }

            var parameterTypes = methodSymbol.Parameters.Select(static parameter => parameter.Type).ToArray();
            var returnType = methodSymbol.ReturnType;

            // forma 1: Dest Map(Src source) — destino nao-lista
            if (parameterTypes.Length == 1 && !TryListElement(returnType, out _))
            {
                return new MapMethodModel
                {
                    MethodName = methodSymbol.Name,
                    Kind = MapMethodKind.Create,
                    SourceType = parameterTypes[0],
                    DestinationType = returnType,
                    Declaration = method,
                };
            }

            // forma 3: List<Dest> Map(IEnumerable<Src> source)
            if (parameterTypes.Length == 1 &&
                TryListElement(returnType, out var destinationElement) &&
                MappingPlanner.TryGetElementType(parameterTypes[0]) is { } sourceElement)
            {
                return new MapMethodModel
                {
                    MethodName = methodSymbol.Name,
                    Kind = MapMethodKind.CreateList,
                    SourceType = sourceElement,
                    DestinationType = destinationElement,
                    Declaration = method,
                };
            }

            // forma 2: void Map(Src source, Dest destination)
            if (parameterTypes.Length == 2 && returnType.SpecialType == SpecialType.System_Void)
            {
                return new MapMethodModel
                {
                    MethodName = methodSymbol.Name,
                    Kind = MapMethodKind.Fill,
                    SourceType = parameterTypes[0],
                    DestinationType = parameterTypes[1],
                    Declaration = method,
                };
            }

            productionContext.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.UnsupportedSignature, method.Identifier.GetLocation(),
                methodSymbol.ToDisplayString()));
            return null;
        }
    }

    private static bool TryListElement(ITypeSymbol type, out ITypeSymbol element)
    {
        if (type is INamedTypeSymbol { IsGenericType: true } named &&
            named.OriginalDefinition.ToDisplayString() == ListDefinition)
        {
            element = named.TypeArguments[0];
            return true;
        }

        element = null!;
        return false;
    }
}
