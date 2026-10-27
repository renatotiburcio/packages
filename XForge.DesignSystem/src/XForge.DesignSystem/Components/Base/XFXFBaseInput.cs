using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace XForge.Components.Base;

public class XFXFBaseInput<T> : ComponentBase
{
    [Parameter] public T? Value { get; set; }
    [Parameter] public EventCallback<T?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<T?>>? ValueExpression { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    protected virtual async Task OnValueChangedAsync(T? value)
    {
        if (EqualityComparer<T?>.Default.Equals(Value, value)) return;
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}

