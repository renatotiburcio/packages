using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Utility;

/// <summary>
/// Enterprise AI risk badge component. Displays a color-coded risk indicator
/// with Low/Medium/High/Critical levels and business context awareness.
/// </summary>
public partial class XFAiRiskBadge : XForgeComponentBase
{
    protected override string BaseClass => "xf-ai-risk-badge";

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Risk severity level.</summary>
    [Parameter]
    public XFAiRiskLevel Level { get; set; } = XFAiRiskLevel.Low;

    /// <summary>Business context for risk coloring.</summary>
    [Parameter]
    public XFAiRiskContext Context { get; set; } = XFAiRiskContext.General;

    /// <summary>Custom label override. If empty, uses level-based default.</summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>Optional numeric risk score (0.0–1.0).</summary>
    [Parameter]
    public double? Score { get; set; }

    /// <summary>Whether to show the numeric score.</summary>
    [Parameter]
    public bool ShowScore { get; set; }

    /// <summary>Size variant.</summary>
    [Parameter]
    public XFAiRiskBadgeSize Size { get; set; } = XFAiRiskBadgeSize.Md;

    /// <summary>Custom badge template.</summary>
    [Parameter]
    public RenderFragment<XFAiRiskBadgeContext>? BadgeTemplate { get; set; }

    // ── Computed ────────────────────────────────────────────────────────

    private string ComputedLabel => Label ?? LevelLabel;

    private string LevelLabel => Level switch
    {
        XFAiRiskLevel.Low => "Baixo",
        XFAiRiskLevel.Medium => "Medio",
        XFAiRiskLevel.High => "Alto",
        XFAiRiskLevel.Critical => "Critico",
        _ => "Desconhecido"
    };

    private string ContextLabel => Context switch
    {
        XFAiRiskContext.Credit => "Credito",
        XFAiRiskContext.Fiscal => "Fiscal",
        XFAiRiskContext.Stock => "Estoque",
        XFAiRiskContext.Fraud => "Fraude",
        XFAiRiskContext.Delinquency => "Inadimplencia",
        _ => "Geral"
    };

    // ── Styling ─────────────────────────────────────────────────────────

    protected override string GetComponentClass()
    {
        return GetClass($"{BaseClass} {SizeClass} inline-flex items-center gap-1.5 rounded-full font-medium {LevelBgClass} {LevelTextClass}");
    }

    private string RootClass => GetComponentClass();

    private string SizeClass => Size switch
    {
        XFAiRiskBadgeSize.Sm => "px-2 py-0.5 text-[10px]",
        XFAiRiskBadgeSize.Lg => "px-3.5 py-1.5 text-sm",
        _ => "px-2.5 py-1 text-xs"
    };

    private string LevelBgClass => Level switch
    {
        XFAiRiskLevel.Low => "bg-emerald-50 ring-1 ring-emerald-200",
        XFAiRiskLevel.Medium => "bg-amber-50 ring-1 ring-amber-200",
        XFAiRiskLevel.High => "bg-orange-50 ring-1 ring-orange-200",
        XFAiRiskLevel.Critical => "bg-red-50 ring-1 ring-red-200",
        _ => "bg-slate-50 ring-1 ring-slate-200"
    };

    private string LevelTextClass => Level switch
    {
        XFAiRiskLevel.Low => "text-emerald-700",
        XFAiRiskLevel.Medium => "text-amber-700",
        XFAiRiskLevel.High => "text-orange-700",
        XFAiRiskLevel.Critical => "text-red-700",
        _ => "text-slate-600"
    };

    private string DotClass => Size switch
    {
        XFAiRiskBadgeSize.Sm => $"h-1.5 w-1.5 rounded-full {LevelDotColorClass}",
        XFAiRiskBadgeSize.Lg => $"h-2.5 w-2.5 rounded-full {LevelDotColorClass}",
        _ => $"h-2 w-2 rounded-full {LevelDotColorClass}"
    };

    private string LevelDotColorClass => Level switch
    {
        XFAiRiskLevel.Low => "bg-emerald-500",
        XFAiRiskLevel.Medium => "bg-amber-500",
        XFAiRiskLevel.High => "bg-orange-500",
        XFAiRiskLevel.Critical => "bg-red-500",
        _ => "bg-slate-400"
    };

    private string LabelClass => "leading-none";

    private string ScoreClass => "ml-0.5 tabular-nums opacity-75";
}

/// <summary>
/// Size variants for XFAiRiskBadge.
/// </summary>
public enum XFAiRiskBadgeSize
{
    /// <summary>Small badge.</summary>
    Sm,

    /// <summary>Medium badge (default).</summary>
    Md,

    /// <summary>Large badge.</summary>
    Lg
}

/// <summary>
/// Context passed to the BadgeTemplate render fragment.
/// </summary>
public sealed class XFAiRiskBadgeContext
{
    /// <summary>Risk level.</summary>
    public XFAiRiskLevel Level { get; init; }

    /// <summary>Business context.</summary>
    public XFAiRiskContext Context { get; init; }

    /// <summary>Computed or custom label.</summary>
    public string Label { get; init; } = string.Empty;
}
