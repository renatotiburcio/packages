using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace XForge.Components.Base;

/// <summary>
/// Queue of actions to be executed after the next render.
/// Inspired by antd-blazor CallAfterRender pattern.
/// </summary>
public sealed class XFFormForwardRef<T>
{
    private T? _ref;
    private bool _hasRef;

    public T? Current => _hasRef ? _ref : default;

    public void Capture(T value)
    {
        _ref = value;
        _hasRef = true;
    }

    public void Release()
    {
        _ref = default;
        _hasRef = false;
    }
}

/// <summary>
/// AfterRender action queue; actions run once after next render then are cleared.
/// </summary>
public abstract class XFFormForwardRefComponentBase : ComponentBase
{
    private readonly Queue<Func<Task>> _afterRenderQueue = new();
    private bool _hasRendered;

    protected void CallAfterRender(Func<Task> action)
    {
        if (_hasRendered)
        {
            _ = action();
        }
        else
        {
            _afterRenderQueue.Enqueue(action);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _hasRendered = true;
        }
        while (_afterRenderQueue.Count > 0)
        {
            var action = _afterRenderQueue.Dequeue();
            try
            {
                await action();
            }
            catch
            {
                // swallow; after-render actions are best-effort
            }
        }
    }
}