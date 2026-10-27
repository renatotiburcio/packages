using System;

namespace XForge.DesignSystem.Components.Icon;

/// <summary>
/// Mark an XF component with the icons it uses, enabling per-component registry + trim.
/// Used by <c>XFIconRegistryGenerator</c> (Roslyn SG) to build the bundle-trimmable index.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class FluentUIComponentAttribute : Attribute
{
    public string[] UsesIcons { get; set; } = Array.Empty<string>();
    public string? DisplayName { get; set; }
    public string? Category { get; set; }
}