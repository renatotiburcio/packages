using System;
using System.Threading.Tasks;

namespace XForge.Core.Dom;

/// <summary>
/// Handle returned from a DOM event subscription. Dispose to unsubscribe.
/// </summary>
public sealed class DomEventSubscription : IAsyncDisposable
{
    private readonly Func<ValueTask> _unsubscribe;
    private bool _disposed;

    public DomEventSubscription(string eventName, string elementId, Func<ValueTask> unsubscribe)
    {
        EventName = eventName;
        ElementId = elementId;
        _unsubscribe = unsubscribe;
    }

    public string EventName { get; }
    public string ElementId { get; }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await _unsubscribe();
    }
}