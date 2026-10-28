using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Overlays.Dialog
{
    public partial class XFDialog : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public bool Open { get; set; }
        [Parameter] public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>Fires when the dialog is closed. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnClose { get; set; }

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public XFOverlaySize Size { get; set; } = XFOverlaySize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFDialogPlacement Placement { get; set; } = XFDialogPlacement.Center;
        [Parameter] public bool Alert { get; set; }
        [Parameter] public bool CloseOnEscape { get; set; } = true;
        [Parameter] public bool CloseOnOverlayClick { get; set; } = true;
        [Parameter] public bool ShowCloseButton { get; set; } = true;
        [Parameter] public string CloseLabel { get; set; } = default!;
        [Parameter] public Func<Task<bool>>? CanClose { get; set; }
        [Parameter] public bool Draggable { get; set; }
        [Parameter] public bool Resizable { get; set; }
        [Parameter] public bool Maximizable { get; set; }
        [Parameter] public bool Maximized { get; set; }
        [Parameter] public EventCallback<bool> MaximizedChanged { get; set; }
        [Parameter] public string DragHandleLabel { get; set; } = default!;
        [Parameter] public string ResizeHandleLabel { get; set; } = default!;
        [Parameter] public string MaximizeLabel { get; set; } = default!;
        [Parameter] public string RestoreLabel { get; set; } = default!;
        [Parameter] public XFDialogScrollMode ScrollMode { get; set; } = XFDialogScrollMode.Body;
        [Parameter] public string? MinWidth { get; set; }
        [Parameter] public string? MaxWidth { get; set; }
        [Parameter] public string? MinHeight { get; set; }
        [Parameter] public string? MaxHeight { get; set; }
        [Parameter] public string? BodyMaxHeight { get; set; }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(CloseLabel)) CloseLabel = L("dialog.close", CloseLabel);
            if (string.IsNullOrWhiteSpace(DragHandleLabel)) DragHandleLabel = L("dialog.move", DragHandleLabel);
            if (string.IsNullOrWhiteSpace(ResizeHandleLabel)) ResizeHandleLabel = L("dialog.resize", ResizeHandleLabel);
            if (string.IsNullOrWhiteSpace(MaximizeLabel)) MaximizeLabel = L("dialog.maximize", MaximizeLabel);
            if (string.IsNullOrWhiteSpace(RestoreLabel)) RestoreLabel = L("dialog.restore", RestoreLabel);
            base.OnInitialized();
        }

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private ElementReference PanelReference { get; set; }
        private bool TrapActive { get; set; }
        private string TitleId => $"{Id}-title";
        private string DescriptionId => $"{Id}-description";
        private string? AriaLabelledBy => !string.IsNullOrWhiteSpace(Title) && Header is null ? TitleId : null;
        private string? AriaDescribedBy => !string.IsNullOrWhiteSpace(Description) && Header is null ? DescriptionId : null;
        private bool ShowHeader => Header is not null || !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Description) || ShowCloseButton;
        private bool EffectiveMaximized => Maximized && Maximizable;

        private string OverlayClass => Join(
            "fixed inset-0 z-50 flex bg-surface-950/35 p-4 backdrop-blur-sm",
            EffectiveMaximized ? "items-stretch justify-stretch" : null,
            !EffectiveMaximized && Placement == XFDialogPlacement.Center ? "items-center justify-center" : null,
            Placement == XFDialogPlacement.Right ? "items-stretch justify-end" : null,
            Placement == XFDialogPlacement.Left ? "items-stretch justify-start" : null,
            Placement == XFDialogPlacement.Top ? "items-start justify-center" : null,
            Placement == XFDialogPlacement.Bottom ? "items-end justify-center" : null);

        private string PanelClass => Join(
            "relative flex min-h-0 flex-col rounded-xl border bg-bg-surface shadow-[0_24px_70px_rgb(15_23_42/.22)] outline-none ring-1",
            EffectiveMaximized ? "h-full w-full max-w-none" : SizeClass,
            ToneClass,
            ScrollMode == XFDialogScrollMode.Panel ? "overflow-auto" : "overflow-hidden",
            EffectiveMaximized ? "rounded-lg" : null,
            !EffectiveMaximized && (Placement == XFDialogPlacement.Right || Placement == XFDialogPlacement.Left) ? "h-full rounded-lg" : null,
            Resizable && !EffectiveMaximized ? "resize overflow-auto" : null,
            Class);

        private string PanelStyle => JoinInlineStyles(Style, ConstraintStyle);
        private string ConstraintStyle => JoinInlineStyles(
            MinWidth is null ? null : $"min-width:{MinWidth}",
            MaxWidth is null ? null : $"max-width:{MaxWidth}",
            MinHeight is null ? null : $"min-height:{MinHeight}",
            MaxHeight is null ? null : $"max-height:{MaxHeight}");
        private string BodyStyle => BodyMaxHeight is null ? string.Empty : $"max-height:{BodyMaxHeight}";

        private string HeaderClass => Join(
            "flex items-start justify-between gap-4 border-b border-border-light px-5 py-4",
            Draggable ? "cursor-move select-none" : null);
        private string BodyClass => Join(
            "min-h-0 px-5 py-4 text-sm leading-6 text-text-secondary",
            ScrollMode == XFDialogScrollMode.Body ? "overflow-auto" : null,
            ScrollMode == XFDialogScrollMode.None ? "overflow-visible" : null);
        private string FooterClass => "flex flex-wrap items-center justify-end gap-2 border-t border-border-light px-5 py-4";
        private string TitleClass => "text-lg font-semibold leading-6 text-text-primary";
        private string DescriptionClass => "text-sm leading-5 text-text-secondary";
        private string CloseButtonClass => "inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-transparent text-text-muted outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] hover:text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] active:scale-95";
        private string HeaderActionClass => CloseButtonClass;
        private string ResizeHandleClass => "absolute bottom-1 right-1 h-4 w-4 cursor-nwse-resize rounded-sm border-b-2 border-r-2 border-text-subtle/60 outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";

        private string SizeClass => Size switch
        {
            XFOverlaySize.Sm => "w-full max-w-sm",
            XFOverlaySize.Lg => "w-full max-w-3xl",
            XFOverlaySize.Xl => "w-full max-w-5xl",
            XFOverlaySize.Full => Placement == XFDialogPlacement.Center ? "h-[calc(100dvh-2rem)] w-full max-w-6xl" : "w-full max-w-full",
            _ => "w-full max-w-xl"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Primary => "border-[color-mix(in_oklab,var(--color-primary)_25%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-primary)_12%,transparent)]",
            XFSemanticColor.Success => "border-[color-mix(in_oklab,var(--color-success)_25%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-success)_12%,transparent)]",
            XFSemanticColor.Warning => "border-[color-mix(in_oklab,var(--color-warning)_30%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-warning)_16%,transparent)]",
            XFSemanticColor.Danger => "border-[color-mix(in_oklab,var(--color-danger)_28%,var(--color-border))] ring-[color-mix(in_oklab,var(--color-danger)_14%,transparent)]",
            _ => "border-surface-200 ring-surface-200/80"
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Open && !TrapActive)
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
            else if (!Open && TrapActive)
            {
                await ReleaseTrapAsync();
            }
        }

        private async Task HandleOverlayClickAsync()
        {
            if (CloseOnOverlayClick)
            {
                await CloseAsync();
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (CloseOnEscape && args.Key == "Escape")
            {
                await CloseAsync();
            }
        }

        private async Task CloseAsync()
        {
            if (CanClose is not null)
            {
                var canProceed = await CanClose();
                if (!canProceed)
                {
                    return;
                }
            }

            Open = false;
            await ReleaseTrapAsync();

            if (OpenChanged.HasDelegate)
            {
                await OpenChanged.InvokeAsync(false);
            }

            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync();
            }
        }

        private async Task ToggleMaximizedAsync()
        {
            if (!Maximizable)
            {
                return;
            }

            Maximized = !Maximized;
            if (MaximizedChanged.HasDelegate)
            {
                await MaximizedChanged.InvokeAsync(Maximized);
            }
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

        private static string JoinInlineStyles(params string?[] values)
        {
            return string.Join("; ", values.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value!.Trim().TrimEnd(';')));
        }
    }
}