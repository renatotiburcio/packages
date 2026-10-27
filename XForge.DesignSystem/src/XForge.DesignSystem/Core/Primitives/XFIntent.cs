namespace XForge.DesignSystem.Core.Primitives;

/// <summary>
/// Defines the visual intent (color semantic) for UI components.
/// Used across Alerts, Toasts, Badges, and other status-aware components.
/// </summary>
public enum XFIntent
{
    /// <summary>Default/standard appearance.</summary>
    Default,

    /// <summary>Informational intent (blue).</summary>
    Info,

    /// <summary>Success/confirmation intent (green).</summary>
    Success,

    /// <summary>Warning/caution intent (yellow/orange).</summary>
    Warning,

    /// <summary>Error/danger intent (red).</summary>
    Danger,

    /// <summary>Primary brand color intent.</summary>
    Primary,

    /// <summary>Secondary brand color intent.</summary>
    Secondary,

    /// <summary>Neutral/gray intent.</summary>
    Neutral
}
