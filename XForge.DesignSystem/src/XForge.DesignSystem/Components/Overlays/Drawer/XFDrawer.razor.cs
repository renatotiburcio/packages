using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Overlays.Drawer
{
    public partial class XFDrawer : XForgeComponentBase, IAsyncDisposable
    {
        [Parameter] public bool Open { get; set; }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>Fires when the drawer is closed. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnClose { get; set; }

        [Parameter] public EventCallback Opened { get; set; }
        [Parameter] public EventCallback Closed { get; set; }
        [Parameter] public Func<XFDrawerOpeningEventArgs, Task>? Opening { get; set; }
        [Parameter] public Func<XFDrawerClosingEventArgs, Task>? Closing { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? AriaLabelledBy { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public XFDrawerPlacement Placement { get; set; } = XFDrawerPlacement.Right;
        [Parameter] public XFOverlaySize Size { get; set; } = XFOverlaySize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFDrawerResponsiveBreakpoint ResponsiveBelow { get; set; } = XFDrawerResponsiveBreakpoint.None;
        [Parameter] public bool Modal { get; set; } = true;
        [Parameter] public bool TrapFocus { get; set; } = true;
        [Parameter] public bool CloseOnEscape { get; set; } = true;
        [Parameter] public bool CloseOnOverlayClick { get; set; } = true;
        [Parameter] public bool ShowCloseButton { get; set; } = true;
        [Parameter] public string CloseLabel { get; set; } = "Fechar painel";
        [Parameter] public int AnimationDuration { get; set; } = 200;

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private bool TrapActive { get; set; }
        private bool ShouldRenderDrawer { get; set; }
        private bool IsOpening { get; set; }
        private bool IsClosing { get; set; }
        private bool PreviousOpen { get; set; }
        private bool openedFired;
        private CancellationTokenSource? transitionCancellation;
        private string TitleId => $"{Id}-title";
        private string DescriptionId => $"{Id}-description";
        private string? EffectiveAriaLabelledBy => !string.IsNullOrWhiteSpace(AriaLabelledBy)
            ? AriaLabelledBy
            : !string.IsNullOrWhiteSpace(Title) && Header is null ? TitleId : null;
        private string? EffectiveAriaLabel => string.IsNullOrWhiteSpace(EffectiveAriaLabelledBy) ? AriaLabel : null;
        private string? AriaDescribedBy => !string.IsNullOrWhiteSpace(Description) && Header is null ? DescriptionId : null;
        private bool ShowHeader => Header is not null || !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Description) || ShowCloseButton;
        private bool IsHorizontal => Placement is XFDrawerPlacement.Left or XFDrawerPlacement.Right;
        private bool UseResponsiveSheet => IsHorizontal && ResponsiveBelow != XFDrawerResponsiveBreakpoint.None;

        private string OverlayClass => Join(
            "fixed inset-0 z-50 flex transition-opacity ease-out motion-reduce:transition-none",
            AnimationDurationClass,
            IsClosing || IsOpening ? "opacity-0" : "opacity-100",
            Modal ? "bg-surface-950/35 backdrop-blur-sm" : "pointer-events-none bg-transparent",
            UseResponsiveSheet ? ResponsiveOverlayClass : null,
            !UseResponsiveSheet && Placement == XFDrawerPlacement.Right ? "items-stretch justify-end" : null,
            !UseResponsiveSheet && Placement == XFDrawerPlacement.Left ? "items-stretch justify-start" : null,
            Placement == XFDrawerPlacement.Top ? "items-start justify-stretch" : null,
            Placement == XFDrawerPlacement.Bottom ? "items-end justify-stretch" : null);

        private string PanelClass => Join(
            "pointer-events-auto flex min-h-0 flex-col overflow-hidden border bg-bg-surface shadow-[0_24px_70px_rgb(15_23_42/.22)] outline-none ring-1 transition-transform ease-out motion-reduce:transition-none",
            AnimationDurationClass,
            PanelMotionClass,
            PlacementClass,
            SizeClass,
            ToneClass,
            Class);

        private string HeaderClass => "flex items-start justify-between gap-4 border-b border-border-light px-5 py-4";
        private string BodyClass => "min-h-0 flex-1 overflow-auto px-5 py-4 text-sm leading-6 text-text-secondary";
        private string FooterClass => "flex flex-wrap items-center justify-end gap-2 border-t border-border-light px-5 py-4";
        private string TitleClass => "text-lg font-semibold leading-6 text-text-primary";
        private string DescriptionClass => "text-sm leading-5 text-text-secondary";
        private string CloseButtonClass => "inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-transparent text-text-muted outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] active:scale-95";

        private string PlacementClass => Placement switch
        {
            XFDrawerPlacement.Left => UseResponsiveSheet ? ResponsivePlacementClass : "h-full rounded-r-xl border-l-0",
            XFDrawerPlacement.Top => "w-full rounded-b-xl border-t-0",
            XFDrawerPlacement.Bottom => "w-full rounded-t-xl border-b-0",
            _ => UseResponsiveSheet ? ResponsivePlacementClass : "h-full rounded-l-xl border-r-0"
        };

        private string SizeClass => (Placement, Size) switch
        {
            _ when UseResponsiveSheet => ResponsiveSizeClass,
            (_, XFOverlaySize.Full) => IsHorizontal ? "w-full max-w-full" : "h-[100dvh]",
            (_, XFOverlaySize.Sm) => IsHorizontal ? "w-full max-w-sm" : "max-h-[18rem]",
            (_, XFOverlaySize.Lg) => IsHorizontal ? "w-full max-w-2xl" : "max-h-[34rem]",
            (_, XFOverlaySize.Xl) => IsHorizontal ? "w-full max-w-4xl" : "max-h-[42rem]",
            _ => IsHorizontal ? "w-full max-w-md" : "max-h-[28rem]"
        };

        private string ResponsiveOverlayClass => ResponsiveBelow switch
        {
            XFDrawerResponsiveBreakpoint.Sm => Placement == XFDrawerPlacement.Left ? "items-end justify-stretch sm:items-stretch sm:justify-start" : "items-end justify-stretch sm:items-stretch sm:justify-end",
            XFDrawerResponsiveBreakpoint.Lg => Placement == XFDrawerPlacement.Left ? "items-end justify-stretch lg:items-stretch lg:justify-start" : "items-end justify-stretch lg:items-stretch lg:justify-end",
            _ => Placement == XFDrawerPlacement.Left ? "items-end justify-stretch md:items-stretch md:justify-start" : "items-end justify-stretch md:items-stretch md:justify-end"
        };

        private string ResponsivePlacementClass => ResponsiveBelow switch
        {
            XFDrawerResponsiveBreakpoint.Sm => Placement == XFDrawerPlacement.Left ? "w-full rounded-t-xl border-b-0 sm:h-full sm:rounded-r-xl sm:rounded-t-none sm:border-l-0" : "w-full rounded-t-xl border-b-0 sm:h-full sm:rounded-l-xl sm:rounded-t-none sm:border-r-0",
            XFDrawerResponsiveBreakpoint.Lg => Placement == XFDrawerPlacement.Left ? "w-full rounded-t-xl border-b-0 lg:h-full lg:rounded-r-xl lg:rounded-t-none lg:border-l-0" : "w-full rounded-t-xl border-b-0 lg:h-full lg:rounded-l-xl lg:rounded-t-none lg:border-r-0",
            _ => Placement == XFDrawerPlacement.Left ? "w-full rounded-t-xl border-b-0 md:h-full md:rounded-r-xl md:rounded-t-none md:border-l-0" : "w-full rounded-t-xl border-b-0 md:h-full md:rounded-l-xl md:rounded-t-none md:border-r-0"
        };

        private string ResponsiveSizeClass => (ResponsiveBelow, Size) switch
        {
            (_, XFOverlaySize.Full) => "w-full max-h-[100dvh] max-w-full",
            (XFDrawerResponsiveBreakpoint.Sm, XFOverlaySize.Sm) => "w-full max-h-[90dvh] sm:max-h-none sm:max-w-sm",
            (XFDrawerResponsiveBreakpoint.Sm, XFOverlaySize.Lg) => "w-full max-h-[90dvh] sm:max-h-none sm:max-w-2xl",
            (XFDrawerResponsiveBreakpoint.Sm, XFOverlaySize.Xl) => "w-full max-h-[90dvh] sm:max-h-none sm:max-w-4xl",
            (XFDrawerResponsiveBreakpoint.Sm, _) => "w-full max-h-[90dvh] sm:max-h-none sm:max-w-md",
            (XFDrawerResponsiveBreakpoint.Lg, XFOverlaySize.Sm) => "w-full max-h-[90dvh] lg:max-h-none lg:max-w-sm",
            (XFDrawerResponsiveBreakpoint.Lg, XFOverlaySize.Lg) => "w-full max-h-[90dvh] lg:max-h-none lg:max-w-2xl",
            (XFDrawerResponsiveBreakpoint.Lg, XFOverlaySize.Xl) => "w-full max-h-[90dvh] lg:max-h-none lg:max-w-4xl",
            (XFDrawerResponsiveBreakpoint.Lg, _) => "w-full max-h-[90dvh] lg:max-h-none lg:max-w-md",
            (_, XFOverlaySize.Sm) => "w-full max-h-[90dvh] md:max-h-none md:max-w-sm",
            (_, XFOverlaySize.Lg) => "w-full max-h-[90dvh] md:max-h-none md:max-w-2xl",
            (_, XFOverlaySize.Xl) => "w-full max-h-[90dvh] md:max-h-none md:max-w-4xl",
            _ => "w-full max-h-[90dvh] md:max-h-none md:max-w-md"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Primary => "border-[color-mix(in_oklab,var(--color-primary)_25%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]",
            XFSemanticColor.Success => "border-[color-mix(in_oklab,var(--color-success)_25%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-success)_12%,transparent)]",
            XFSemanticColor.Warning => "border-[color-mix(in_oklab,var(--color-warning)_30%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-warning)_16%,transparent)]",
            XFSemanticColor.Danger => "border-[color-mix(in_oklab,var(--color-danger)_28%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-danger)_14%,transparent)]",
            _ => "border-surface-200 ring-surface-200/80"
        };

        private string AnimationDurationClass => AnimationDuration <= 0
            ? "duration-0"
            : AnimationDuration <= 150
                ? "duration-150"
                : AnimationDuration <= 200
                    ? "duration-200"
                    : AnimationDuration <= 300
                        ? "duration-300"
                        : "duration-500";

        private string PanelMotionClass
        {
            get
            {
                if (!IsOpening && !IsClosing)
                {
                    return "translate-x-0 translate-y-0";
                }

                return Placement switch
                {
                    XFDrawerPlacement.Left => UseResponsiveSheet ? ResponsiveClosedTransformClass : "-translate-x-full",
                    XFDrawerPlacement.Top => "-translate-y-full",
                    XFDrawerPlacement.Bottom => "translate-y-full",
                    _ => UseResponsiveSheet ? ResponsiveClosedTransformClass : "translate-x-full"
                };
            }
        }

        private string ResponsiveClosedTransformClass => ResponsiveBelow switch
        {
            XFDrawerResponsiveBreakpoint.Sm => Placement == XFDrawerPlacement.Left ? "translate-y-full sm:-translate-x-full sm:translate-y-0" : "translate-y-full sm:translate-x-full sm:translate-y-0",
            XFDrawerResponsiveBreakpoint.Lg => Placement == XFDrawerPlacement.Left ? "translate-y-full lg:-translate-x-full lg:translate-y-0" : "translate-y-full lg:translate-x-full lg:translate-y-0",
            _ => Placement == XFDrawerPlacement.Left ? "translate-y-full md:-translate-x-full md:translate-y-0" : "translate-y-full md:translate-x-full md:translate-y-0"
        };

        protected override void OnInitialized()
        {
            PreviousOpen = Open;
            ShouldRenderDrawer = Open;
            IsOpening = Open;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Open && !PreviousOpen)
            {
                await StartOpenTransitionAsync();
            }
            else if (!Open && PreviousOpen)
            {
                await StartCloseTransitionAsync(XFDrawerCloseReason.Programmatic);
            }
            else if (Open && !ShouldRenderDrawer)
            {
                await StartOpenTransitionAsync();
            }

            PreviousOpen = Open;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsOpening)
            {
                await Task.Yield();
                IsOpening = false;

                if (!openedFired)
                {
                    openedFired = true;
                    if (Opened.HasDelegate)
                    {
                        await Opened.InvokeAsync();
                    }
                }

                StateHasChanged();
                return;
            }

            if (ShouldRenderDrawer && Open && TrapFocus && !TrapActive && !IsClosing)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfDialog.trap", Id);
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
            else if ((!Open || !TrapFocus) && TrapActive)
            {
                await ReleaseTrapAsync();
            }
        }

        private async Task HandleOverlayClickAsync()
        {
            if (CloseOnOverlayClick)
            {
                await CloseAsync(XFDrawerCloseReason.Overlay);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (CloseOnEscape && args.Key == "Escape")
            {
                await CloseAsync(XFDrawerCloseReason.Escape);
            }
        }

        private async Task CloseAsync(XFDrawerCloseReason reason = XFDrawerCloseReason.User)
        {
            if (Closing is not null)
            {
                var closingArgs = new XFDrawerClosingEventArgs(reason);
                await Closing(closingArgs);
                if (closingArgs.Cancel)
                {
                    return;
                }
            }

            Open = false;
            PreviousOpen = false;
            await StartCloseTransitionAsync(reason);
            await ReleaseTrapAsync();

            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(false);
            }

            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync();
            }

            if (Closed.HasDelegate)
            {
                await Closed.InvokeAsync();
            }
        }

        private async Task StartOpenTransitionAsync()
        {
            if (Opening is not null)
            {
                var openingArgs = new XFDrawerOpeningEventArgs();
                await Opening(openingArgs);
                if (openingArgs.Cancel)
                {
                    Open = false;
                    PreviousOpen = false;
                    if (OpenChanged.HasDelegate)
                    {
                        await OpenChanged.InvokeAsync(false);
                    }
                    return;
                }
            }

            transitionCancellation?.Cancel();
            transitionCancellation?.Dispose();
            transitionCancellation = null;

            ShouldRenderDrawer = true;
            IsOpening = true;
            IsClosing = false;
            openedFired = false;
        }

        private async Task StartCloseTransitionAsync(XFDrawerCloseReason reason)
        {
            if (!ShouldRenderDrawer || IsClosing)
            {
                return;
            }

            IsOpening = false;
            IsClosing = true;
            _ = CompleteCloseTransitionAsync();
        }

        private async Task CompleteCloseTransitionAsync()
        {
            transitionCancellation?.Cancel();
            transitionCancellation?.Dispose();
            transitionCancellation = new CancellationTokenSource();
            var cancellationToken = transitionCancellation.Token;

            try
            {
                await Task.Delay(Math.Max(0, AnimationDuration), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (Open || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            ShouldRenderDrawer = false;
            IsClosing = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ReleaseTrapAsync()
        {
            if (!TrapActive)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfDialog.release", Id);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
            TrapActive = false;
        }

        public async ValueTask DisposeAsync()
        {
            transitionCancellation?.Cancel();
            transitionCancellation?.Dispose();

            if (TrapActive)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("xfDialog.release", Id);
                }
                catch (JSDisconnectedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}