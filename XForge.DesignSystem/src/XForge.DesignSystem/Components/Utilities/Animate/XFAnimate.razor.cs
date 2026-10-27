using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Utilities.Animate
{
    public partial class XFAnimate : XForgeComponentBase, IAsyncDisposable
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool KeepMounted { get; set; }
        [Parameter] public XFAnimateEffect Effect { get; set; } = XFAnimateEffect.Fade;
        [Parameter] public XFAnimateTrigger Trigger { get; set; }
        [Parameter] public int Duration { get; set; } = 180;
        [Parameter] public int Delay { get; set; }
        [Parameter] public string? Easing { get; set; } = "cubic-bezier(0.4,0,0.2,1)";
        [Parameter] public bool Once { get; set; } = true;
        [Parameter] public bool Mirror { get; set; }
        [Parameter] public double Threshold { get; set; } = 0.12;
        [Parameter] public string RootMargin { get; set; } = "0px 0px -10% 0px";

        /// <summary>Fires when element becomes visible in viewport. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnVisibleIn { get; set; }

        /// <summary>Fires when element leaves viewport. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback OnVisibleOut { get; set; }


        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DotNetObjectReference<XFAnimate>? dotNetReference;
        private bool observerActive;
        private bool viewportVisible;
        private ElementReference RootReference { get; set; }

        private bool IsViewportTriggered => Trigger == XFAnimateTrigger.OnVisible;
        private bool EffectiveVisible => IsViewportTriggered ? viewportVisible : Visible;
        private bool ShouldRenderContent => IsViewportTriggered || Visible || KeepMounted;
        private string StateValue => EffectiveVisible ? "entered" : "exited";
        private string AriaHidden => (!EffectiveVisible).ToString().ToLowerInvariant();

        private string RootClass => Join(
            "xf-animate transform-gpu transition-[opacity,transform,filter] will-change-[opacity,transform,filter] [backface-visibility:hidden]",
            EffectClass,
            EffectiveVisible ? EnteredClass : ExitedClass,
            Class);

        private string InlineStyle => JoinStyle(
            $"transition-duration:{Math.Max(0, Duration)}ms",
            $"transition-delay:{Math.Max(0, Delay)}ms",
            !string.IsNullOrWhiteSpace(Easing) ? $"transition-timing-function:{Easing}" : null,
            EffectTransformStyle,
            Style);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsViewportTriggered)
            {
                if (observerActive)
                {
                    await ReleaseObserverAsync();
                }

                return;
            }

            if (!observerActive)
            {
                dotNetReference ??= DotNetObjectReference.Create(this);

                try
                {
                    await JSRuntime.InvokeVoidAsync(
                        "xfAnimate.observe",
                        Id,
                        dotNetReference,
                        new
                        {
                            once = Once,
                            mirror = Mirror,
                            threshold = Math.Clamp(Threshold, 0, 1),
                            rootMargin = RootMargin
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

                observerActive = true;
            }
        }

        [JSInvokable]
        public async Task SetViewportVisibleAsync(bool visible)
        {
            if (visible == viewportVisible)
            {
                return;
            }

            viewportVisible = visible;

            if (visible && OnVisibleIn.HasDelegate)
            {
                await OnVisibleIn.InvokeAsync();
            }
            else if (!visible && OnVisibleOut.HasDelegate)
            {
                await OnVisibleOut.InvokeAsync();
            }

            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            await ReleaseObserverAsync();
            dotNetReference?.Dispose();
        }

        private async Task ReleaseObserverAsync()
        {
            if (!observerActive)
            {
                return;
            }

            try
            {
                await JSRuntime.InvokeVoidAsync("xfAnimate.release", Id);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            observerActive = false;
        }

        private string EffectClass => Effect switch
        {
            XFAnimateEffect.Pulse => "animate-pulse",
            _ => ""
        };

        private string EnteredClass => Effect switch
        {
            XFAnimateEffect.SlideUp or XFAnimateEffect.SlideDown or XFAnimateEffect.SlideLeft or XFAnimateEffect.SlideRight
                or XFAnimateEffect.SlideUpLeft or XFAnimateEffect.SlideUpRight or XFAnimateEffect.SlideDownLeft or XFAnimateEffect.SlideDownRight
                or XFAnimateEffect.Rise or XFAnimateEffect.Drop => "opacity-100 translate-x-0 translate-y-0",
            XFAnimateEffect.Scale or XFAnimateEffect.ScaleSoft or XFAnimateEffect.ZoomIn or XFAnimateEffect.ZoomOut or XFAnimateEffect.Pop or XFAnimateEffect.SoftBounce => "opacity-100 scale-100",
            XFAnimateEffect.FadeBlur => "opacity-100 blur-0",
            XFAnimateEffect.FlipUp or XFAnimateEffect.FlipDown or XFAnimateEffect.FlipLeft or XFAnimateEffect.FlipRight => "opacity-100",
            _ => "opacity-100"
        };

        private string ExitedClass => Effect switch
        {
            XFAnimateEffect.SlideUp => "pointer-events-none opacity-0 translate-y-3",
            XFAnimateEffect.SlideDown => "pointer-events-none opacity-0 -translate-y-3",
            XFAnimateEffect.SlideLeft => "pointer-events-none opacity-0 translate-x-3",
            XFAnimateEffect.SlideRight => "pointer-events-none opacity-0 -translate-x-3",
            XFAnimateEffect.SlideUpLeft => "pointer-events-none opacity-0 translate-x-3 translate-y-3",
            XFAnimateEffect.SlideUpRight => "pointer-events-none opacity-0 -translate-x-3 translate-y-3",
            XFAnimateEffect.SlideDownLeft => "pointer-events-none opacity-0 translate-x-3 -translate-y-3",
            XFAnimateEffect.SlideDownRight => "pointer-events-none opacity-0 -translate-x-3 -translate-y-3",
            XFAnimateEffect.Rise => "pointer-events-none opacity-0 translate-y-6",
            XFAnimateEffect.Drop => "pointer-events-none opacity-0 -translate-y-6",
            XFAnimateEffect.Scale => "pointer-events-none opacity-0 scale-95",
            XFAnimateEffect.ScaleSoft => "pointer-events-none opacity-0 scale-[.98]",
            XFAnimateEffect.ZoomIn => "pointer-events-none opacity-0 scale-90",
            XFAnimateEffect.ZoomOut => "pointer-events-none opacity-0 scale-110",
            XFAnimateEffect.Pop => "pointer-events-none opacity-0 scale-75",
            XFAnimateEffect.SoftBounce => "pointer-events-none opacity-0 scale-95 translate-y-2",
            XFAnimateEffect.FadeBlur => "pointer-events-none opacity-0 blur-sm",
            XFAnimateEffect.FlipUp or XFAnimateEffect.FlipDown or XFAnimateEffect.FlipLeft or XFAnimateEffect.FlipRight => "pointer-events-none opacity-0",
            _ => "pointer-events-none opacity-0"
        };

        private string? EffectTransformStyle => Effect switch
        {
            XFAnimateEffect.FlipUp => FlipTransform("top", EffectiveVisible ? "0deg" : "18deg", "X"),
            XFAnimateEffect.FlipDown => FlipTransform("bottom", EffectiveVisible ? "0deg" : "-18deg", "X"),
            XFAnimateEffect.FlipLeft => FlipTransform("left", EffectiveVisible ? "0deg" : "18deg", "Y"),
            XFAnimateEffect.FlipRight => FlipTransform("right", EffectiveVisible ? "0deg" : "-18deg", "Y"),
            _ => null
        };

        private static string FlipTransform(string origin, string angle, string axis)
        {
            return $"transform-origin:{origin};transform:perspective(900px) rotate{axis}({angle})";
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
