using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Toast
{
    /// <summary>
    /// Defines the visual intent (color semantic) for XFToast.
    /// </summary>
    [Obsolete("Use XFIntent from XForge.DesignSystem.Core.Primitives instead. This enum will be removed in v2.0.")]
    public enum XFToastIntent
    {
        Default,
        Info,
        Success,
        Warning,
        Danger,
        Primary,
        Neutral
    }

    /// <summary>
    /// Defines the color mode for XFToast.
    /// </summary>
    public enum XFToastColorMode
    {
        Default,
        Rich,
        Solid
    }

    /// <summary>
    /// Defines the placement position for XFToast.
    /// </summary>
    public enum XFToastPlacement
    {
        TopStart,
        Top,
        TopEnd,
        BottomStart,
        Bottom,
        BottomEnd
    }

    /// <summary>
    /// Defines the swipe dismiss direction for XFToast.
    /// </summary>
    public enum XFToastSwipeDirection
    {
        Auto,
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// Defines the ARIA live region politeness for XFToast.
    /// </summary>
    [Obsolete("Use XFAriaLive from XForge.DesignSystem.Core.Primitives instead. This enum will be removed in v2.0.")]
    public enum XFToastLive
    {
        Polite,
        Assertive,
        Off
    }
}
