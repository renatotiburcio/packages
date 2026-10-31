using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace XForge.DesignSystem.Components.Base;

public class XFXFBaseButton : ComponentBase
{
    [Parameter] public string Variant { get; set; } = "solid";
    [Parameter] public string Size { get; set; } = "md";
    [Parameter] public string Color { get; set; } = "primary";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool FullWidth { get; set; }
    [Parameter] public string Type { get; set; } = "button";
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    protected virtual async Task HandleClickAsync(MouseEventArgs args)
    {
        if (Disabled) return;
        await OnClick.InvokeAsync(args);
    }
}