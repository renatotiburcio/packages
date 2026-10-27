using Microsoft.JSInterop;


using System.Threading.Tasks;

namespace XForge.Core.Dom;

/// <summary>
/// Subscribes to a DOM event on a given element and returns a disposable handle.
/// </summary>
public interface IDomEventListener
{
    Task<IAsyncDisposable> SubscribeAsync(string eventName, string elementId, DotNetObjectReference<object> callback);
}
