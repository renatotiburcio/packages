// T050: Extension abstraction (Blazorise 38 Extensions pattern).
// P1, 8h, no deps.
namespace XForge.DesignSystem.Extensions;

/// <summary>
/// Marker interface for opt-in extensions (DataGrid, Charts, Scheduler, etc.).
/// Implementations are discovered via <see cref="XFExtensionRegistry"/> and activated at runtime.
/// </summary>
public interface IXFExtension
{
    /// <summary>Unique extension id (e.g. "DataGrid", "Charts", "Scheduler").</summary>
    string Id { get; }
    /// <summary>Human-readable name.</summary>
    string Name { get; }
    /// <summary>Indicates whether the extension is currently enabled.</summary>
    bool IsEnabled { get; }
    /// <summary>Activate the extension (load JS, register components, etc.).</summary>
    void Activate(IServiceProvider services);
    /// <summary>Deactivate the extension.</summary>
    void Deactivate();
}

/// <summary>Top-level extension category. T050 keeps these as the curated 38-extension catalog.</summary>
public static class XFCatalogedExtensions
{
    public static readonly string[] KnownIds =
    {
        "DataGrid","Charts","Scheduler","RichTextEditor","PivotGrid","TreeList",
        "Diagram","Gauges","Map","QRCode","Barcode","Signature","Spreadsheet",
        "DocumentProcessing","Zip","TaskBoard","AnimationContainer","TileLayout",
        "StackLayout","GridLayout","PageTemplates","FloatingActionButton","DockManager",
        "SmartPasteButton","ThreeDCharts","SankeyDiagram","Ribbon","Stepper","QueryBuilder",
        "RangeSelector","BulletChart","TreeMap","Timeline","DashboardLayout","OTPInput",
        "SpeechToText","FigmaBridge"
    };
    public const int Count = 38;
}