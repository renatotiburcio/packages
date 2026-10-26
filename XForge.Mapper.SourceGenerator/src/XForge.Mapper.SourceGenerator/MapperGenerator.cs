using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XForge.Mapper.Generators;

/// <summary>
/// Source generator do XForge.Mapper: descobre classes marcadas com
/// <see cref="XForgeMapAttribute"/> e emite o corpo dos metodos parciais de mapeamento usando as
/// mesmas convencoes do motor de runtime - mas como codigo C# compilado (zero runtime, zero reflection).
/// </summary>
[Generator]
public sealed class MapperGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "XForge.Mapper.Generators.XForgeMapAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeFullName,
            static (node, _) => node is ClassDeclarationSyntax,
            static (syntaxContext, _) =>
                (Symbol: syntaxContext.TargetSymbol as INamedTypeSymbol,
                 Declaration: (ClassDeclarationSyntax)syntaxContext.TargetNode))
            .Where(static pair => pair.Symbol is not null)
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(classes);

        context.RegisterSourceOutput(compilationAndClasses, static (productionContext, pair) =>
        {
            var (compilation, classes) = pair;
            Execute(productionContext, compilation, classes);
        });
    }

    private static void Execute(SourceProductionContext context, Compilation compilation,
        System.Collections.Immutable.ImmutableArray<(INamedTypeSymbol? Symbol, ClassDeclarationSyntax Declaration)> classes)
    {
        foreach (var (symbol, declaration) in classes)
        {
            if (symbol is null)
            {
                continue;
            }

            if (!declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.ClassMustBePartial, declaration.Identifier.GetLocation()));
                continue;
            }

            var methods = MapMethodDiscovery.Discover(declaration, symbol, compilation, context);

            if (methods.Count == 0)
            {
                continue;
            }

            var emitter = new CodeEmitter(methods);
            var source = emitter.EmitClass(symbol, declaration);

            foreach (var issue in emitter.Issues)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        issue.DiagnosticId,
                        issue.DiagnosticId switch
                        {
                            "XFG005" => "Ciclo detectado durante a geracao",
                            "XFG003" => "Membro de destino sem contraparte na origem",
                            _ => "Membro de destino com tipos incompativeis",
                        },
                        $"'{issue.MemberName}': {issue.Reason}",
                        "XForge.Mapper.SourceGenerator",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    declaration.Identifier.GetLocation()));
            }

            if (source is null)
            {
                continue;
            }

            context.AddSource(symbol.Name + ".g.cs", source.NormalizeWhitespace().ToFullString());
        }
    }
}
