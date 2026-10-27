using Microsoft.AspNetCore.Components;

namespace XForge.Components.Base;

/// <summary>
/// Base for components needing theme/RTL/breakpoint cascade.
/// Inspired by MudBlazor MudComponentBase.
/// </summary>
public abstract class XFXFComponentBase : ComponentBase
{
    [Parameter] public string? Theme { get; set; }
    [Parameter] public bool RightToLeft { get; set; }
    [Parameter] public string? Breakpoint { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    protected string EffectiveTheme => Theme ?? "default";
    protected string EffectiveBreakpoint => Breakpoint ?? "md";

    /// <summary>True if user prefers dark mode (placeholder for system preference).</summary>
    protected bool IsDarkMode => Theme == "dark";
}