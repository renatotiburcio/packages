using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Tooling;

/// <summary>
/// Visual theme customization editor with color, typography, spacing panels,
/// live preview, presets, and CSS/JSON export. Pure Blazor, no JS interop.
/// </summary>
public partial class XFThemeEditor : XForgeComponentBase, IDisposable
{
    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Component title.</summary>
    [Parameter]
    public string Title { get; set; } = "Theme Editor";

    /// <summary>ARIA label.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Editor de temas";

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show the preset selector.</summary>
    [Parameter]
    public bool ShowPresetSelector { get; set; } = true;

    /// <summary>Whether to show the dark mode toggle.</summary>
    [Parameter]
    public bool ShowDarkModeToggle { get; set; } = true;

    /// <summary>Whether to show the export panel.</summary>
    [Parameter]
    public bool ShowExportPanel { get; set; } = true;

    /// <summary>Available theme presets.</summary>
    [Parameter]
    public IReadOnlyList<XFThemePreset> Presets { get; set; } = Array.Empty<XFThemePreset>();

    /// <summary>Initial colors.</summary>
    [Parameter]
    public IReadOnlyList<XFThemeColor>? InitialColors { get; set; }

    /// <summary>Initial typography.</summary>
    [Parameter]
    public IReadOnlyList<XFThemeTypography>? InitialTypography { get; set; }

    /// <summary>Initial spacing.</summary>
    [Parameter]
    public IReadOnlyList<XFThemeSpacing>? InitialSpacing { get; set; }

    /// <summary>Fired when any theme value changes.</summary>
    [Parameter]
    public EventCallback OnThemeChanged { get; set; }

    // ── Lifecycle ───────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        _colors = InitialColors?.ToList() ?? GetDefaultColors();
        _typography = InitialTypography?.ToList() ?? GetDefaultTypography();
        _spacing = InitialSpacing?.ToList() ?? GetDefaultSpacing();
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Gets current theme colors.</summary>
    public IReadOnlyList<XFThemeColor> GetColors() => _colors.AsReadOnly();

    /// <summary>Gets current typography.</summary>
    public IReadOnlyList<XFThemeTypography> GetTypography() => _typography.AsReadOnly();

    /// <summary>Gets current spacing.</summary>
    public IReadOnlyList<XFThemeSpacing> GetSpacing() => _spacing.AsReadOnly();

    /// <summary>Exports theme as CSS custom properties.</summary>
    public string ExportCss()
    {
        ExportAsCss();
        return _exportOutput;
    }

    /// <summary>Exports theme as JSON.</summary>
    public string ExportJson()
    {
        ExportAsJson();
        return _exportOutput;
    }

    // ── Defaults ────────────────────────────────────────────────────────

    private static List<XFThemeColor> GetDefaultColors() => new()
    {
        new() { Name = "Primary", Value = "#6366f1", Channel = XFThemeColorChannel.Primary, CssVariable = "--xf-color-primary" },
        new() { Name = "Secondary", Value = "#8b5cf6", Channel = XFThemeColorChannel.Secondary, CssVariable = "--xf-color-secondary" },
        new() { Name = "Success", Value = "#10b981", Channel = XFThemeColorChannel.Success, CssVariable = "--xf-color-success" },
        new() { Name = "Danger", Value = "#ef4444", Channel = XFThemeColorChannel.Danger, CssVariable = "--xf-color-danger" },
        new() { Name = "Warning", Value = "#f59e0b", Channel = XFThemeColorChannel.Warning, CssVariable = "--xf-color-warning" },
        new() { Name = "Info", Value = "#3b82f6", Channel = XFThemeColorChannel.Info, CssVariable = "--xf-color-info" },
        new() { Name = "Background", Value = "#ffffff", Channel = XFThemeColorChannel.Background, CssVariable = "--xf-color-bg" },
        new() { Name = "Surface", Value = "#f8fafc", Channel = XFThemeColorChannel.Surface, CssVariable = "--xf-color-surface" },
        new() { Name = "Text", Value = "#0f172a", Channel = XFThemeColorChannel.Text, CssVariable = "--xf-color-text" },
    };

    private static List<XFThemeTypography> GetDefaultTypography() => new()
    {
        new() { Name = "Heading", FontFamily = "Inter, sans-serif", FontSize = "1.5rem", FontWeight = "700", LineHeight = "1.3", LetterSpacing = "-0.025em", CssVariable = "--xf-font-heading" },
        new() { Name = "Body", FontFamily = "Inter, sans-serif", FontSize = "0.875rem", FontWeight = "400", LineHeight = "1.5", LetterSpacing = "0", CssVariable = "--xf-font-body" },
        new() { Name = "Mono", FontFamily = "JetBrains Mono, monospace", FontSize = "0.8125rem", FontWeight = "400", LineHeight = "1.6", LetterSpacing = "0", CssVariable = "--xf-font-mono" },
    };

    private static List<XFThemeSpacing> GetDefaultSpacing() => new()
    {
        new() { Name = "Xs", Value = "0.25rem", CssVariable = "--xf-space-xs" },
        new() { Name = "Sm", Value = "0.5rem", CssVariable = "--xf-space-sm" },
        new() { Name = "Md", Value = "1rem", CssVariable = "--xf-space-md" },
        new() { Name = "Lg", Value = "1.5rem", CssVariable = "--xf-space-lg" },
        new() { Name = "Xl", Value = "2rem", CssVariable = "--xf-space-xl" },
        new() { Name = "2xl", Value = "3rem", CssVariable = "--xf-space-2xl" },
    };

    // ── Styling ─────────────────────────────────────────────────────────

    private string RootClass => string.Join(" ",
        "xf-theme-editor overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-sm",
        Class).Trim();

    // ── IDisposable ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
