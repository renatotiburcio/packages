using XForge.DesignSystem.Components.Icon;

namespace XForge.DesignSystem.Components.Icon.XFFluentIconTypes;

/// <summary>Parameter documentation for <see cref="XFFluentIcon{TIcon}"/>.</summary>
public static class XFFluentIconParameters
{
    public const string ValueDoc = "Icon instance. Type name is used as catalog key.";
    public const string VariantDoc = "Visual style: Regular, Filled, Light, Color.";
    public const string SizeDoc = "Size in pixels (10/12/16/20/24/28/32/48/Custom).";
    public const string ColorDoc = "Semantic color: Accent/Lightweight/Success/Warning/Error/Important/Custom.";
    public const string CustomColorDoc = "CSS color string. Only valid when Color=Custom.";
    public const string WidthDoc = "Override width in pixels; null uses Size token.";
    public const string OnClickDoc = "Fired when icon is clicked (or Enter/Space when Focusable).";
    public const string FocusableDoc = "When true, icon becomes a button (tabindex=0, role=button, keyboard handler).";
    public const string AriaLabelDoc = "Accessible label. Defaults to type name.";
    public const string ChildContentDoc = "Optional slot replacing the default SVG.";
}