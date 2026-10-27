using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Components.Layout;
using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Layout.Splitter
{
    public partial class XFSplitter : XForgeComponentBase, IAsyncDisposable
    {
        private double currentSize = 50;
        private double lastExternalPrimarySize = double.NaN;
        private DotNetObjectReference<XFSplitter>? dotNetReference;
        private bool splitterRegistered;

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter] public RenderFragment? PrimaryContent { get; set; }
        [Parameter] public RenderFragment? SecondaryContent { get; set; }
        [Parameter] public XFOrientation Orientation { get; set; }
        [Parameter] public double PrimarySize { get; set; } = 50;
        [Parameter] public EventCallback<double> PrimarySizeChanged { get; set; }
        [Parameter] public EventCallback<XFSplitterChangedEventArgs> Change { get; set; }
        [Parameter] public double Min { get; set; } = 15;
        [Parameter] public double Max { get; set; } = 85;
        [Parameter] public double Step { get; set; } = 1;
        [Parameter] public bool Collapsed { get; set; }
        [Parameter] public bool CollapsePrimary { get; set; }
        [Parameter] public string? AriaLabel { get; set; } = "Redimensionar paineis";

        private double CurrentSize => currentSize;
        private double SecondarySize => 100 - currentSize;
        private string OrientationValue => Orientation == XFOrientation.Vertical ? "vertical" : "horizontal";
        private string HandleId => $"{Id}-handle";

        protected override void OnParametersSet()
        {
            // Only reset when parent externally changes PrimarySize (not during internal drag/keyboard)
            if (double.IsNaN(lastExternalPrimarySize) || Math.Abs(PrimarySize - lastExternalPrimarySize) > double.Epsilon)
            {
                currentSize = Clamp(PrimarySize);
                lastExternalPrimarySize = PrimarySize;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Collapsed || Disabled)
            {
                await ReleaseSplitterAsync();
                return;
            }

            await RegisterSplitterAsync();
        }

        private string RootClass => Join(
            "xf-splitter min-w-0 overflow-hidden rounded-lg border border-border-light bg-bg-surface select-none",
            Orientation == XFOrientation.Vertical ? "grid min-h-80" : "grid min-h-64",
            splitterRegistered ? "xf-splitter--ready" : "",
            Disabled ? "opacity-60" : "",
            Class);

        private string RootStyle => JoinStyle(
            Orientation == XFOrientation.Vertical
                ? $"grid-template-rows:{PrimaryTrack} auto {SecondaryTrack}"
                : $"grid-template-columns:{PrimaryTrack} auto {SecondaryTrack}",
            Style);

        private string PrimaryTrack => Collapsed && CollapsePrimary ? "0fr" : $"{currentSize}fr";
        private string SecondaryTrack => Collapsed && !CollapsePrimary ? "0fr" : $"{SecondarySize}fr";

        private string PrimaryPaneClass => Join("min-w-0 min-h-0 overflow-auto", Collapsed && CollapsePrimary ? "hidden" : "");
        private string SecondaryPaneClass => Join("min-w-0 min-h-0 overflow-auto", Collapsed && !CollapsePrimary ? "hidden" : "");
        private string PrimaryPaneStyle => Collapsed && CollapsePrimary ? "display:none" : "";
        private string SecondaryPaneStyle => Collapsed && !CollapsePrimary ? "display:none" : "";

        private string SeparatorWrapClass => Orientation == XFOrientation.Vertical
            ? "relative flex h-4 items-center justify-center bg-surface-50"
            : "relative flex w-4 items-center justify-center bg-surface-50";

        private string HandleClass => Orientation == XFOrientation.Vertical
            ? "group touch-none absolute inset-x-0 -top-2 -bottom-2 flex cursor-row-resize items-center justify-center border-0 bg-transparent p-0 text-surface-500 outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-bg-surface disabled:cursor-not-allowed"
            : "group touch-none absolute inset-y-0 -left-2 -right-2 flex cursor-col-resize items-center justify-center border-0 bg-transparent p-0 text-surface-500 outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-bg-surface disabled:cursor-not-allowed";

        private string HandleVisualClass => Orientation == XFOrientation.Vertical
            ? "flex h-2 w-16 items-center justify-center rounded-full bg-surface-200 transition-colors group-hover:bg-primary group-focus-visible:bg-primary"
            : "flex h-16 w-2 items-center justify-center rounded-full bg-surface-200 transition-colors group-hover:bg-primary group-focus-visible:bg-primary";

        private string GripClass => Orientation == XFOrientation.Vertical
            ? "block h-px w-8 rounded-full bg-current opacity-70 group-hover:text-white group-focus-visible:text-white"
            : "block h-8 w-px rounded-full bg-current opacity-70 group-hover:text-white group-focus-visible:text-white";

        [JSInvokable]
        public async Task SetSizeFromJsAsync(double next, bool notifyChanged)
        {
            await SetSizeAsync(next, notifyChanged);
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            var delta = Math.Max(Step, 1);
            var next = args.Key switch
            {
                "ArrowRight" or "ArrowDown" => currentSize + delta,
                "ArrowLeft" or "ArrowUp" => currentSize - delta,
                "PageUp" => currentSize + delta * 10,
                "PageDown" => currentSize - delta * 10,
                "Home" => Min,
                "End" => Max,
                _ => double.NaN
            };

            if (!double.IsNaN(next))
            {
                await SetSizeAsync(next, notifyChanged: true);
            }
        }

        private async Task ResetAsync(MouseEventArgs args)
        {
            if (!Disabled)
            {
                await SetSizeAsync(PrimarySize, notifyChanged: true);
            }
        }

        private async Task SetSizeAsync(double next, bool notifyChanged)
        {
            currentSize = Clamp(next);

            // Only invoke changed events when the value actually changes
            if (Math.Abs(currentSize - PrimarySize) > double.Epsilon)
            {
                if (PrimarySizeChanged.HasDelegate)
                {
                    await PrimarySizeChanged.InvokeAsync(currentSize);
                }
            }

            if (notifyChanged && Change.HasDelegate)
            {
                await Change.InvokeAsync(new XFSplitterChangedEventArgs(currentSize, SecondarySize, Orientation));
            }

            // Only re-render on final/notified changes (e.g. drag end, keyboard, double-click).
            // During JS pointermove, the JS code sets CSS grid directly for zero-latency feedback.
            if (notifyChanged)
            {
                StateHasChanged();
            }
        }

        private async Task RegisterSplitterAsync()
        {
            // Already registered, nothing to do
            if (splitterRegistered)
            {
                return;
            }

            dotNetReference ??= DotNetObjectReference.Create(this);

            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "xfSplitter.register",
                    Id,
                    HandleId,
                    dotNetReference,
                    new
                    {
                        orientation = OrientationValue,
                        min = Min,
                        max = Max,
                        step = Step
                    });

                splitterRegistered = true;
                StateHasChanged();
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender — retry on next render
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected (Blazor Server) — retry on next render
            }
            catch (JSException)
            {
                // JS runtime error — retry on next render
            }
        }

        private async Task ReleaseSplitterAsync()
        {
            if (!splitterRegistered)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfSplitter.release", HandleId);
            }
            catch (InvalidOperationException)
            {
                // JS not available during prerender
            }
            catch (JSDisconnectedException)
            {
                // JS disconnected
            }
            splitterRegistered = false;
        }

        public async ValueTask DisposeAsync()
        {
            await ReleaseSplitterAsync();
            dotNetReference?.Dispose();
        }

        private double Clamp(double value)
        {
            var min = Math.Clamp(Min, 0, 100);
            var max = Math.Clamp(Max, min, 100);
            return Math.Clamp(value, min, max);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string JoinStyle(params string?[] values)
        {
            return string.Join(";", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
