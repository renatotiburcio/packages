using XForge.Core.Dom;
using Microsoft.JSInterop;


using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace XForge.Core.Base;

/// <summary>
/// Base class for components that need DOM event subscription via DomEventService.
/// </summary>
public abstract class XFDomComponentBase : ComponentBase
{
    [Inject] protected DomEventService? EventService { get; set; }

    protected Task<IAsyncDisposable> SubscribeDomEventAsync(string eventName, string elementId, DotNetObjectReference<object> callback)
    {
        if (EventService is null)
        {
            throw new System.InvalidOperationException("DomEventService not registered. Add services.AddXForgeDomEvents() in Program.cs.");
        }
        return EventService.SubscribeAsync(eventName, elementId, callback);
    }
}
