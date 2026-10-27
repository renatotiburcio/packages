using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XForge.Analyzers;

public interface IXFAnalyzerDescriptor
{
    string Id { get; }
    string Title { get; }
    string MessageFormat { get; }
    DiagnosticSeverity DefaultSeverity { get; }
    bool IsEnabledByDefault { get; }
    DiagnosticDescriptor Descriptor { get; }
}

public sealed record XFCatalogedAnalyzer(string Id, string Title, string MessageFormat, DiagnosticSeverity Severity, bool Enabled) : IXFAnalyzerDescriptor
{
    public string MessageFormat { get; } = MessageFormat;
    public DiagnosticSeverity DefaultSeverity => Severity;
    public bool IsEnabledByDefault => Enabled;
    public DiagnosticDescriptor Descriptor => new(Id, Title, MessageFormat, "XForge", Severity, Enabled);
}

public static class XFCatalogedAnalyzers
{
    public static ImmutableArray<IXFAnalyzerDescriptor> All { get; } = ImmutableArray.Create<IXFAnalyzerDescriptor>(
        new XFCatalogedAnalyzer("XF001", "Use ParameterState for derived values", "Use ParameterState<T> to manage derived value state", DiagnosticSeverity.Info, true),
        new XFCatalogedAnalyzer("XF002", "Component should have Category attribute", "Apply [Category] attribute to organize the component", DiagnosticSeverity.Info, true),
        new XFCatalogedAnalyzer("XF003", "Missing XML documentation on public Parameter", "Public Parameter should have XML docs", DiagnosticSeverity.Warning, false),
        new XFCatalogedAnalyzer("XF004", "EventCallback should follow On{Action} naming", "Rename callback to On{Action} convention", DiagnosticSeverity.Info, true)
    );
    public static int Count => All.Length;
}