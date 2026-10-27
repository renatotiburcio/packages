using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Core.Base
{
    /// <summary>
    /// Base class for XForge form input components.
    /// Extends <see cref="XForgeComponentBase"/> with standardized:
    /// <list type="bullet">
    ///   <item><see cref="InputRef"/> — <see cref="ElementReference"/> to the primary input element</item>
    ///   <item><see cref="FocusAsync"/> — programmatic focus</item>
    ///   <item><see cref="SelectAsync"/> — programmatic text selection</item>
    ///   <item><see cref="BlurAsync"/> — programmatic blur</item>
    /// </list>
    /// </summary>
    public abstract class XFInputComponentBase : XForgeComponentBase, IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private IJSObjectReference? _focusModule;

        /// <summary>
        /// <see cref="ElementReference"/> to the primary native input element.
        /// Subclasses must assign this via <c>@@ref="InputRef"</c> in their Razor template.
        /// </summary>
        protected ElementReference InputRef { get; set; }

        /// <summary>
        /// Whether this component has an associated JS focus module loaded.
        /// </summary>
        private bool _focusModuleLoaded;

        /// <summary>
        /// Focus the primary input element.
        /// </summary>
        public async ValueTask FocusAsync()
        {
            try
            {
                var module = await GetFocusModuleAsync();
                if (module is not null)
                {
                    await module.InvokeVoidAsync("focus", InputRef);
                }
                else
                {
                    await InputRef.FocusAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // JS runtime not available (e.g., during prerender)
            }
            catch (JSException)
            {
                // Element not yet rendered
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected
            }
        }

        /// <summary>
        /// Select all text in the primary input element.
        /// Only effective on <c>&lt;input&gt;</c> and <c>&lt;textarea&gt;</c> elements.
        /// </summary>
        public async ValueTask SelectAsync()
        {
            try
            {
                var module = await GetFocusModuleAsync();
                if (module is not null)
                {
                    await module.InvokeVoidAsync("select", InputRef);
                }
            }
            catch (InvalidOperationException)
            {
                // JS runtime not available (e.g., during prerender)
            }
            catch (JSException)
            {
                // Element not yet rendered or does not support select
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected
            }
        }

        /// <summary>
        /// Remove focus from the primary input element.
        /// </summary>
        public async ValueTask BlurAsync()
        {
            try
            {
                var module = await GetFocusModuleAsync();
                if (module is not null)
                {
                    await module.InvokeVoidAsync("blur", InputRef);
                }
            }
            catch (InvalidOperationException)
            {
                // JS runtime not available (e.g., during prerender)
            }
            catch (JSException)
            {
                // Element not yet rendered
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected
            }
        }

        private async ValueTask<IJSObjectReference?> GetFocusModuleAsync()
        {
            if (!_focusModuleLoaded)
            {
                try
                {
                    _focusModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/XForge.DesignSystem/js/xf-focus.js");
                    _focusModuleLoaded = true;
                }
                catch
                {
                    _focusModuleLoaded = true; // don't retry
                }
            }

            return _focusModule;
        }

        /// <summary>
        /// Disposes the JS focus module reference.
        /// Override in subclasses but always call <c>await base.DisposeAsync()</c>.
        /// </summary>
        public virtual async ValueTask DisposeAsync()
        {
            if (_focusModule is not null)
            {
                try
                {
                    await _focusModule.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // Circuit already disconnected
                }
                _focusModule = null;
            }
        }
    }
}
