using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Overlays.Popover
{
    public partial class XFPopover : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public bool Open { get; set; }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }
        [Parameter] public string ButtonText { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(ButtonText)) ButtonText = L("popover.open", ButtonText);
            base.OnInitialized();
        }
        [Parameter] public string? PanelAriaLabel { get; set; }
        [Parameter] public string? PanelAriaLabelledBy { get; set; }
        [Parameter] public string Role { get; set; } = default!;
        [Parameter] public RenderFragment<XFPopoverTriggerContext>? Trigger { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFPopoverPlacement Placement { get; set; } = XFPopoverPlacement.Bottom;
        [Parameter] public XFPopoverAlignment Alignment { get; set; } = XFPopoverAlignment.Center;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public int Offset { get; set; } = 8;
        [Parameter] public int CollisionPadding { get; set; } = 12;
        [Parameter] public bool Flip { get; set; } = true;
        [Parameter] public bool CloseOnEscape { get; set; } = true;
        [Parameter] public bool CloseOnOutsideClick { get; set; } = true;
        [Parameter] public bool TrapFocus { get; set; } = true;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Open, ButtonText, PanelAriaLabel, PanelAriaLabelledBy, Role, Trigger, ChildContent, Placement, Alignment, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<XFPopover>? DotNetReference { get; set; }
        private bool Registered { get; set; }
        private bool TrapActive { get; set; }
        private string AnchorId => $"{Id}-anchor";
        private string PanelId => $"{Id}-panel";
        private string? EffectivePanelAriaLabel => string.IsNullOrWhiteSpace(PanelAriaLabelledBy)
            ? (string.IsNullOrWhiteSpace(PanelAriaLabel) ? ButtonText : PanelAriaLabel)
            : null;
        private XFPopoverTriggerContext TriggerContext => new(Open, ToggleAsync, OpenAsync, CloseAsync);

        private string RootClass => Join("xf-popover inline-flex", Class);
        private string DefaultTriggerClass => "inline-flex h-9 items-center justify-center rounded-lg border border-surface-200 bg-bg-surface px-3 text-sm font-semibold text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)] transition-all duration-150 hover:border-[color-mix(in_oklab,var(--color-primary)_35%,var(--color-border))] hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        private string PanelClass => Join(
            "fixed z-50 rounded-lg border border-surface-200 bg-bg-surface p-3 text-sm text-text-secondary shadow-[0_18px_40px_rgb(15_23_42/.14)] outline-none ring-1 ring-surface-200/80",
            SizeClass);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "w-56",
            XFSize.Lg => "w-96",
            _ => "w-72"
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Open)
            {
                DotNetReference ??= DotNetObjectReference.Create(this);
                try
                {
                    await JSRuntime.InvokeVoidAsync(
                        "xfPopover.register",
                        AnchorId,
                        PanelId,
                        DotNetReference,
                        new
                        {
                            placement = Placement.ToString().ToLowerInvariant(),
                            alignment = Alignment.ToString().ToLowerInvariant(),
                            offset = Offset,
                            collisionPadding = CollisionPadding,
                            flip = Flip,
                            closeOnEscape = CloseOnEscape,
                            closeOnOutsideClick = CloseOnOutsideClick
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
                Registered = true;

                if (TrapFocus && !TrapActive)
                {
                    try
                    {
                        await JSRuntime.InvokeVoidAsync("xfFocusTrap.activate", PanelId);
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
                    TrapActive = true;
                }
            }
            else if (Registered)
            {
                await ReleaseAsync();
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (CloseOnEscape && args.Key == "Escape")
            {
                await CloseAsync();
            }
        }

        private async Task ToggleAsync()
        {
            if (Open)
            {
                await CloseAsync();
            }
            else
            {
                await OpenAsync();
            }
        }

        private async Task OpenAsync()
        {
            Open = true;
            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(true);
            }
        }

        private async Task CloseAsync()
        {
            Open = false;
            if (TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", PanelId);
                }
                catch (InvalidOperationException) { }
                catch (JSDisconnectedException) { }
                TrapActive = false;
            }
            await ReleaseAsync();
            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(false);
            }
        }

        [JSInvokable]
        public async Task CloseFromJsAsync()
        {
            await CloseAsync();
        }

        private async Task ReleaseAsync()
        {
            if (!Registered)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfPopover.release", PanelId);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
            Registered = false;
        }

        public async ValueTask DisposeAsync()
        {
            if (TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfFocusTrap.release", PanelId);
                }
                catch (JSDisconnectedException) { }
                catch (InvalidOperationException) { }
            }

            if (Registered)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfPopover.release", PanelId);
                }
                catch (JSDisconnectedException) { }
                catch (InvalidOperationException) { }
            }

            DotNetReference?.Dispose();
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
