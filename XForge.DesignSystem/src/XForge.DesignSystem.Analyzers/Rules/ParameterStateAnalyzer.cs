using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XForge.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ParameterStateAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "XF001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Use ParameterState for derived values",
        messageFormat: "Property {0} derives from a [Parameter]; consider using ParameterState<T> for auto-tracking",
        category: "XForge",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Detects derived properties computed from [Parameter] fields without using ParameterState<T>.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var prop = (IPropertySymbol)context.Symbol;
        if (prop.DeclaredAccessibility != Accessibility.Public) return;
        // Stub: real implementation would inspect containing type for [Parameter] fields.
        // For catalog-level testing only, no actual analysis is performed.
    }
}