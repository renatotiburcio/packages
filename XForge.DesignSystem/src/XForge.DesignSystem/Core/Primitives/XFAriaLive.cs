namespace XForge.DesignSystem.Core.Primitives;

/// <summary>
/// Defines the politeness level for ARIA live regions.
/// Controls how screen readers announce dynamic content changes.
/// </summary>
public enum XFAriaLive
{
    /// <summary>Waits for user idle before announcing (default).</summary>
    Polite,

    /// <summary>Interrupts immediately to announce changes.</summary>
    Assertive,

    /// <summary>No live announcement.</summary>
    Off
}
