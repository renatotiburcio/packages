using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Layout.Shells;

public enum HxSidebarPosition { Left, Right }

public class HxSidebar : XForgeComponentBase
{
    [Parameter] public HxSidebarPosition Position { get; set; } = HxSidebarPosition.Left;
    [Parameter] public bool Collapsed { get; set; }
    [Parameter] public int Width { get; set; } = 240;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnToggle { get; set; }
    public string EffectiveWidth => Collapsed ? "0" : Width.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public Task ToggleAsync() => OnToggle.InvokeAsync();
    public string PositionClass => Position == HxSidebarPosition.Left ? "xf-sidebar-left" : "xf-sidebar-right";
}

public class HxListLayout<T> : XForgeComponentBase
{
    [Parameter] public System.Collections.Generic.IEnumerable<T>? Items { get; set; }
    [Parameter] public RenderFragment<T>? ItemTemplate { get; set; }
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }
    [Parameter] public string? HeaderTitle { get; set; }
    public int Count => Items?.Count() ?? 0;
    public bool IsEmpty => Count == 0;
}
