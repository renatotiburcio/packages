using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Pre-defined animated icons. Uses CSS keyframes from xf-designsystem.css.
/// Honors prefers-reduced-motion (pauses animation).
/// </summary>
public static class AnimatedIcons
{
    public static readonly IReadOnlyDictionary<string, (XFIconAnimation Animation, string Svg)> Catalog =
        new Dictionary<string, (XFIconAnimation, string)>
        {
            ["loader"] = (XFIconAnimation.Spin, "<svg viewBox=\"0 0 24 24\"><circle cx=\"12\" cy=\"12\" r=\"8\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-dasharray=\"32\"/></svg>"),
            ["spinner"] = (XFIconAnimation.Spin, "<svg viewBox=\"0 0 24 24\"><path d=\"M12 2a10 10 0 1 0 10 10\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"/></svg>"),
            ["pulse-dot"] = (XFIconAnimation.Pulse, "<svg viewBox=\"0 0 24 24\"><circle cx=\"12\" cy=\"12\" r=\"4\" fill=\"currentColor\"/></svg>"),
            ["bounce-arrow"] = (XFIconAnimation.Bounce, "<svg viewBox=\"0 0 24 24\"><path d=\"M12 4v16M5 13l7 7 7-7\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"/></svg>"),
            ["rotate-180"] = (XFIconAnimation.Rotate, "<svg viewBox=\"0 0 24 24\"><path d=\"M12 4l8 8H4z\" fill=\"currentColor\"/></svg>"),
            ["check-success"] = (XFIconAnimation.None, "<svg viewBox=\"0 0 24 24\"><path d=\"M5 12l5 5L20 7\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"/></svg>"),
            ["alert-pulse"] = (XFIconAnimation.Pulse, "<svg viewBox=\"0 0 24 24\"><path d=\"M12 2L2 22h20L12 2z\" fill=\"currentColor\"/></svg>"),
        };

    public static IReadOnlyList<string> Names => new List<string>(Catalog.Keys);
}