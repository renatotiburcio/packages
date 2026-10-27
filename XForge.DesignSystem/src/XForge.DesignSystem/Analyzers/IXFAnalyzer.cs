// T054: Blazorise.Analyzers tests (Roslyn analyzer registry).
// P2, 16h, depends T002 (Roslyn Analyzers).
using Microsoft.CodeAnalysis;

namespace XForge.DesignSystem.Analyzers;

/// <summary>
/// Lightweight analyzer descriptor used by the XForge analyzers test harness.
/// In production each implementation is a real <c>DiagnosticAnalyzer</c> registered via Roslyn.
/// </summary>
public interface IXFAnalyzer
{
    /// <summary>Stable diagnostic id (e.g. "XF001").</summary>
    string Id { get; }
    /// <summary>Short title shown in IDE / dotnet build output.</summary>
    string Title { get; }
    /// <summary>Default severity.</summary>
    DiagnosticSeverity DefaultSeverity { get; }
    /// <summary>True if the analyzer should be enabled by default.</summary>
    bool IsEnabledByDefault { get; }
}

/// <summary>Catalog of analyzers shipped with the XForge meta-package.</summary>
public static class XFCatalogedAnalyzers
{
    public static readonly IXFAnalyzer[] All =
    {
        new ParameterStateAnalyzerDescriptor(),
        new MissingParameterAnalyzerDescriptor(),
        new ComponentNamingAnalyzerDescriptor(),
        new EditFormAnalyzerDescriptor(),
    };

    public const int Count = 4;
}

public sealed class ParameterStateAnalyzerDescriptor : IXFAnalyzer
{
    public string Id => "XF001";
    public string Title => "Use ParameterState for derived values";
    public DiagnosticSeverity DefaultSeverity => DiagnosticSeverity.Info;
    public bool IsEnabledByDefault => true;
}

public sealed class MissingParameterAnalyzerDescriptor : IXFAnalyzer
{
    public string Id => "XF002";
    public string Title => "Mark public component fields with [Parameter]";
    public DiagnosticSeverity DefaultSeverity => DiagnosticSeverity.Warning;
    public bool IsEnabledByDefault => true;
}

public sealed class ComponentNamingAnalyzerDescriptor : IXFAnalyzer
{
    public string Id => "XF003";
    public string Title => "Component classes should start with X";
    public DiagnosticSeverity DefaultSeverity => DiagnosticSeverity.Info;
    public bool IsEnabledByDefault => false;
}

public sealed class EditFormAnalyzerDescriptor : IXFAnalyzer
{
    public string Id => "XF004";
    public string Title => "Prefer XFForm over plain EditForm for LGPD audit";
    public DiagnosticSeverity DefaultSeverity => DiagnosticSeverity.Hidden;
    public bool IsEnabledByDefault => true;
}