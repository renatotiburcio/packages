using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Performance;

/// <summary>
/// Bundle size analyzer with category breakdown, module list,
/// tree-shake analysis, and compression ratio. Pure Blazor, no JS interop.
/// </summary>
public partial class XFAiBundleAnalyzer : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Bundle Analyzer";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Analisador de tamanho do bundle";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the footer bar.</summary>
    [Parameter]
    public bool ShowFooter { get; set; } = true;

    /// <summary>Whether to show summary cards.</summary>
    [Parameter]
    public bool ShowSummary { get; set; } = true;

    /// <summary>Whether to show category breakdown.</summary>
    [Parameter]
    public bool ShowBreakdown { get; set; } = true;

    /// <summary>Whether to show module list table.</summary>
    [Parameter]
    public bool ShowModuleList { get; set; } = true;

    /// <summary>Maximum visible modules in the list.</summary>
    [Parameter]
    public int MaxVisibleModules { get; set; } = 50;

    /// <summary>Bundle analysis data.</summary>
    [Parameter]
    public XFAiBundleSummary? BundleData { get; set; }

    /// <summary>Fired when analysis is requested.</summary>
    [Parameter]
    public EventCallback OnAnalyze { get; set; }

    // ── Internal State ──────────────────────────────────────────────────

    private XFAiBundleSummary? _summary;

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (BundleData is not null)
            _summary = BundleData;
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Loads bundle analysis data.</summary>
    public void LoadData(XFAiBundleSummary data)
    {
        _summary = data;
        StateHasChanged();
    }

    /// <summary>Triggers analysis.</summary>
    public async Task AnalyzeAsync()
    {
        if (OnAnalyze.HasDelegate)
            await OnAnalyze.InvokeAsync();
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024.0):F2} MB"
    };

    private static string GetCategoryColor(XFAiBundleCategory category) => category switch
    {
        XFAiBundleCategory.Components => "bg-blue-500",
        XFAiBundleCategory.Services => "bg-purple-500",
        XFAiBundleCategory.Styles => "bg-pink-500",
        XFAiBundleCategory.Scripts => "bg-amber-500",
        XFAiBundleCategory.Icons => "bg-emerald-500",
        XFAiBundleCategory.Fonts => "bg-cyan-500",
        XFAiBundleCategory.ThirdParty => "bg-red-500",
        _ => "bg-slate-400"
    };

    private static string GetCategoryLabel(XFAiBundleCategory category) => category switch
    {
        XFAiBundleCategory.Components => "Componentes",
        XFAiBundleCategory.Services => "Servicos",
        XFAiBundleCategory.Styles => "Estilos",
        XFAiBundleCategory.Scripts => "Scripts",
        XFAiBundleCategory.Icons => "Icones",
        XFAiBundleCategory.Fonts => "Fontes",
        XFAiBundleCategory.ThirdParty => "Terceiros",
        _ => "Outros"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-ai-bundle-analyzer overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
