using Microsoft.JSInterop;


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XForge.Core.Dom;

/// <summary>
/// Centralized DOM event subscription service. Singleton, registered in DI.
/// Components inject this service and subscribe to events via SubscribeAsync.
/// </summary>
public sealed class DomEventService : IDomEventListener
{
    private readonly ConcurrentDictionary<string, IDomEventListener> _listeners = new();
    private int _subscriptionCount;

    public int ActiveSubscriptionCount => _subscriptionCount;

    public void RegisterListener(string elementId, IDomEventListener listener)
    {
        _listeners[elementId] = listener;
    }

    public async Task<IAsyncDisposable> SubscribeAsync(string eventName, string elementId, DotNetObjectReference<object> callback)
    {
        if (!_listeners.TryGetValue(elementId, out var listener))
        {
            throw new InvalidOperationException($"No listener registered for element {elementId}");
        }
        var handle = await listener.SubscribeAsync(eventName, elementId, callback);
        System.Threading.Interlocked.Increment(ref _subscriptionCount);
        return new SubscriptionWrapper(handle, () =>
        {
            System.Threading.Interlocked.Decrement(ref _subscriptionCount);
            return ValueTask.CompletedTask;
        });
    }

    private sealed class SubscriptionWrapper : IAsyncDisposable
    {
        private readonly IAsyncDisposable _inner;
        private readonly Func<ValueTask> _onDispose;
        public SubscriptionWrapper(IAsyncDisposable inner, Func<ValueTask> onDispose) { _inner = inner; _onDispose = onDispose; }
        public async ValueTask DisposeAsync() { await _inner.DisposeAsync(); await _onDispose(); }
    }
}
