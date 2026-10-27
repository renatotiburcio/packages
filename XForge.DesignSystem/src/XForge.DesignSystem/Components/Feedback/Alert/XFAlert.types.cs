using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Alert
{
    /// <summary>
    /// Defines the visual intent (color semantic) for XFAlert.
    /// </summary>
    [Obsolete("Use XFIntent from XForge.DesignSystem.Core.Primitives instead. This enum will be removed in v2.0.")]
    public enum XFAlertIntent
    {
        Info,
        Success,
        Warning,
        Danger,
        Primary,
        Secondary,
        Neutral
    }

    /// <summary>
    /// Defines the visual variant for XFAlert.
    /// </summary>
    public enum XFAlertVariant
    {
        Soft,
        Solid,
        Outline,
        Subtle
    }

    /// <summary>
    /// Defines the ARIA live region politeness for XFAlert.
    /// </summary>
    [Obsolete("Use XFAriaLive from XForge.DesignSystem.Core.Primitives instead. This enum will be removed in v2.0.")]
    public enum XFAlertLive
    {
        Polite,
        Assertive,
        Off
    }
}
