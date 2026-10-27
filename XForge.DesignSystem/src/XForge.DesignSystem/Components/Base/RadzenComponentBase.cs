using XForge.DesignSystem.Core.Base;


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.Base;
namespace XForge.DesignSystem.Components.Base;
public abstract class RadzenComponentBase : XForgeComponentBase
{
    [Parameter] public EventCallback OnMouseEnter { get; set; }
    [Parameter] public EventCallback OnMouseLeave { get; set; }
    [Parameter] public EventCallback OnContextMenu { get; set; }
    [CascadingParameter] public CultureInfo? DefaultCulture { get; set; }
    public CultureInfo EffectiveCulture => DefaultCulture ?? CultureInfo.CurrentUICulture;
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public bool Visible { get; set; } = true;
    protected bool HasBeenVisible { get; private set; }
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (Visible) HasBeenVisible = true;
        else if (HasBeenVisible) OnBecameInvisible();
    }
    protected virtual void OnBecameInvisible() { }
    protected virtual string GetComponentCssClass() => string.Empty;
    public string GetCssClass()
    {
        var c = GetComponentCssClass();
        if (string.IsNullOrEmpty(CssClass)) return c;
        return string.IsNullOrEmpty(c) ? CssClass : $"{c} {CssClass}";
    }
    public string GetId() => Id;
    public Task MouseEnterAsync() => OnMouseEnter.InvokeAsync();
    public Task MouseLeaveAsync() => OnMouseLeave.InvokeAsync();
    public Task ContextMenuAsync() => OnContextMenu.InvokeAsync();
    public Dictionary<string,string> ParseStyle(string? style)
    {
        var d = new Dictionary<string,string>();
        if (string.IsNullOrEmpty(style)) return d;
        foreach (var part in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split(':', 2);
            if (kv.Length == 2) d[kv[0].Trim()] = kv[1].Trim();
        }
        return d;
    }
}
