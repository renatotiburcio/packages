using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Utilities.FocusTrap
{
    public partial class XFFocusTrap : XForgeComponentBase, IAsyncDisposable
    {
        protected override string BaseClass => "xf-focus-trap";

        private bool trapActive;

        [Parameter] public bool Active { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool RestoreFocus { get; set; } = true;
        [Parameter] public bool FocusOnActivate { get; set; } = true;
        [Parameter] public bool EscapeReleases { get; set; }
        [Parameter] public string? InitialFocusSelector { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        /// <summary>Fires when Escape is pressed. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnEscape { get; set; }


        [Parameter] public EventCallback<bool> ActiveChanged { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} min-h-0 min-w-0 outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_40%,transparent)] focus-visible:ring-offset-2");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Visible && Active && !trapActive)
            {
                await ActivateAsync();
            }
            else if ((!Visible || !Active) && trapActive)
            {
                await ReleaseAsync(RestoreFocus);
            }
        }

        public async Task ActivateAsync()
        {
            if (trapActive)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfFocusTrap.activate", Id, new
                {
                    restoreFocus = RestoreFocus,
                    focusOnActivate = FocusOnActivate,
                    initialFocusSelector = InitialFocusSelector
                });
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
                return;
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
                return;
            }
            trapActive = true;
        }

        public async Task FocusInitialAsync()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("xfFocusTrap.focusInitial", Id, InitialFocusSelector);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
        }

        public async Task ReleaseAsync(bool restoreFocus = true)
        {
            if (!trapActive)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", Id, restoreFocus);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
            trapActive = false;
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key != "Escape")
            {
                return;
            }

            if (OnEscape.HasDelegate)
            {
                await OnEscape.InvokeAsync();
            }

            if (EscapeReleases)
            {
                Active = false;
                await ActiveChanged.InvokeAsync(false);
                await ReleaseAsync(RestoreFocus);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!trapActive)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", Id, RestoreFocus);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
