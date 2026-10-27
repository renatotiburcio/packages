using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace XForge.Core.State;

/// <summary>
/// Auto-tracking property state (MudBlazor-inspired).
/// Recomputes derived value when base parameter changes.
/// </summary>
public sealed class ParameterState<T>
{
    private readonly Func<Task<T>> _getter;
    private T _value = default!;
    private bool _initialized;

    public ParameterState(Func<Task<T>> getter)
    {
        _getter = getter ?? throw new ArgumentNullException(nameof(getter));
    }

    public T Value
    {
        get
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("ParameterState not initialized. Call InitializeAsync first.");
            }
            return _value;
        }
    }

    public async Task InitializeAsync()
    {
        _value = await _getter();
        _initialized = true;
    }

    public async Task SetValueAsync(T newValue)
    {
        _value = newValue;
        _initialized = true;
        await Task.CompletedTask;
    }

    public void Reset()
    {
        _value = default!;
        _initialized = false;
    }
}