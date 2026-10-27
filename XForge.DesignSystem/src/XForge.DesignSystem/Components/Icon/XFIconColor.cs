namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Maps XFIconColor enum to CSS custom properties for theme-aware rendering.
/// Aligned with XForge Design Tokens (T001) + T083.
/// </summary>
public static class XFIconColorMap
{
    public static string ToCssVariable(XFIconColor color) => color switch
    {
        XFIconColor.Accent => "var(--xf-color-accent)",
        XFIconColor.Lightweight => "var(--xf-color-text-secondary)",
        XFIconColor.Success => "var(--xf-color-success)",
        XFIconColor.Warning => "var(--xf-color-warning)",
        XFIconColor.Error => "var(--xf-color-error)",
        XFIconColor.Important => "var(--xf-color-important)",
        XFIconColor.Custom => "var(--xf-icon-custom-color, currentColor)",
        _ => "currentColor",
    };
}