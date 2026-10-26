using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XForge.Mediator.Generators;

/// <summary>
/// Source generator do XForge.Mediator: descobre implementações de
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> (incluindo a variante
/// <c>IRequestHandler&lt;TRequest&gt;</c> = <c>Unit</c>) na compilação do usuário e emite:
/// um wrapper tipado por par (request, resposta) com a cadeia de behaviors resolvida do DI,
/// um registry de dispatch, um <c>ISender</c> gerado e a extensão
/// <c>AddXForgeMediatorGenerated(this IServiceCollection)</c>.
/// Classes marcadas com <c>[XForgeMediatorIgnore]</c> são ignoradas.
/// </summary>
[Generator]
public sealed class MediatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 },
                static (syntaxContext, _) =>
                    (Declaration: (ClassDeclarationSyntax)syntaxContext.Node,
                     SyntaxContext: syntaxContext))
            .Where(static pair => pair.Declaration is not null)
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(classes);

        context.RegisterSourceOutput(compilationAndClasses, (productionContext, pair) =>
        {
            var (compilation, classDeclarations) = pair;
            var handlers = DiscoverHandlers(compilation, classDeclarations);
            HandlersEmitter.Emit(productionContext, handlers, compilation);
        });
    }

    private static List<HandlerModel> DiscoverHandlers(
        Compilation compilation,
        System.Collections.Immutable.ImmutableArray<(ClassDeclarationSyntax Declaration, GeneratorSyntaxContext SyntaxContext)> declarations)
    {
        var handlers = new List<HandlerModel>();

        var requestHandler = compilation.GetTypeByMetadataName("XForge.Mediator.IRequestHandler`2");
        var requestHandlerUnit = compilation.GetTypeByMetadataName("XForge.Mediator.IRequestHandler`1");
        var ignoreAttribute = compilation.GetTypeByMetadataName("XForge.Mediator.XForgeMediatorIgnoreAttribute");

        if (requestHandler is null)
        {
            return handlers; // pacote Mediator não referenciado — nada a gerar
        }

        foreach (var (declaration, syntaxContext) in declarations)
        {
            if (syntaxContext.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol { IsAbstract: false, IsStatic: false } namedType)
            {
                continue;
            }

            if (ignoreAttribute is not null && HasAttribute(namedType, ignoreAttribute))
            {
                continue;
            }

            foreach (var interfaceType in namedType.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, requestHandler))
                {
                    handlers.Add(new HandlerModel(
                        namedType,
                        interfaceType.TypeArguments[0],
                        interfaceType.TypeArguments[1]));
                }
                else if (requestHandlerUnit is not null &&
                         SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, requestHandlerUnit))
                {
                    var unitType = compilation.GetTypeByMetadataName("XForge.Mediator.Unit")!;
                    handlers.Add(new HandlerModel(namedType, interfaceType.TypeArguments[0], unitType));
                }
            }
        }

        return handlers;
    }

    private static bool HasAttribute(INamedTypeSymbol symbol, INamedTypeSymbol attributeType)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is not null &&
                SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
            {
                return true;
            }
        }

        return false;
    }
}
