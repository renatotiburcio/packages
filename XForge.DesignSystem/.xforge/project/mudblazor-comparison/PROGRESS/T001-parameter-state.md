# T001 - ParameterState<T> for all XForge components

## Status: DONE (file exists from antd-blazor scope)

## Implementation
File: `src\XForge.DesignSystem\Core\State\ParameterState.cs`

```csharp
namespace XForge.Core.State;

public sealed class ParameterState<T>
{
    private readonly Func<Task<T>> _getter;
    private T _value = default!;
    private bool _initialized;
    public T Value
    {
        get
        {
            if (!_initialized)
                throw new InvalidOperationException("ParameterState not initialized. Call InitializeAsync() first.");
            return _value;
        }
    }
    public ParameterState(Func<Task<T>> getter) => _getter = getter ?? throw new ArgumentNullException(nameof(getter));
    public async Task InitializeAsync() { _value = await _getter(); _initialized = true; }
    public async Task SetValueAsync(T value) { _value = value; _initialized = true; await Task.CompletedTask; }
    public void Reset() { _value = default!; _initialized = false; }
}
```

## Criteria
- [x] Sealed class in XForge.Core.State namespace
- [x] Func<Task<T>> getter constructor
- [x] Value property throws InvalidOperationException when not initialized
- [x] InitializeAsync / SetValueAsync(T) / Reset() methods
- [x] Unit tests pass (8 tests in ParameterStateTests.cs)