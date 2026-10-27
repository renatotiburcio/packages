using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Carousel
{
    public partial class XFCarousel : XForgeComponentBase, IAsyncDisposable
    {
        private readonly List<XFCarouselSlide> slides = new();
        private CancellationTokenSource? autoplayCancellation;
        private int currentIndex;
        private int previousIndex;
        private int? previousSelectedIndexParameter;
        private bool isPointerPaused;
        private bool isFocusPaused;
        private bool autoplayRunning;
        private double? swipeStartX;
        private double? swipeStartY;

        [Parameter] public int SelectedIndex { get; set; }
        [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }
        [Parameter] public EventCallback<XFCarouselChangeEventArgs> Change { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? PreviousIcon { get; set; }
        [Parameter] public RenderFragment? NextIcon { get; set; }
        [Parameter] public bool Autoplay { get; set; }
        [Parameter] public int Interval { get; set; } = 5000;
        [Parameter] public bool Loop { get; set; } = true;
        [Parameter] public bool ShowControls { get; set; } = true;
        [Parameter] public bool ShowIndicators { get; set; } = true;
        [Parameter] public bool PauseOnHover { get; set; } = true;
        [Parameter] public bool PauseOnFocus { get; set; } = true;
        [Parameter] public bool RespectReducedMotion { get; set; } = true;
        [Parameter] public bool SwipeEnabled { get; set; } = true;
        [Parameter] public double SwipeThreshold { get; set; } = 48;
        [Parameter] public XFCarouselIndicatorPosition IndicatorPosition { get; set; } = XFCarouselIndicatorPosition.Bottom;
        [Parameter] public XFCarouselTransition Transition { get; set; } = XFCarouselTransition.Slide;
        [Parameter] public string PreviousLabel { get; set; } = "Slide anterior";
        [Parameter] public string NextLabel { get; set; } = "Proximo slide";
        [Parameter] public string PauseLabel { get; set; } = "A reproducao automatica pausa ao passar o mouse ou focar.";
        [Parameter] public string IndicatorsLabel { get; set; } = "Slides";
        [Parameter] public string IndicatorLabelFormat { get; set; } = "Ir para slide {0}";
        [Parameter] public string? AriaLabel { get; set; }

        internal int CurrentIndex => currentIndex;
        internal int SlideCount => slides.Count;

        private bool HasSlides => slides.Count > 0;
        private bool CanGoPrevious => HasSlides && (Loop || currentIndex > 0);
        private bool CanGoNext => HasSlides && (Loop || currentIndex < slides.Count - 1);
        private bool IsAutoplayPaused => Disabled || isPointerPaused || isFocusPaused;
        private bool ShouldAutoplay => Autoplay && HasSlides && slides.Count > 1;
        private string LiveMode => Autoplay ? "off" : "polite";
        private string ReducedMotionClass => RespectReducedMotion ? "motion-reduce:transition-none motion-reduce:transform-none" : "";
        private string PauseDescriptionId => $"{Id}-pause";

        private string RootClass =>
            Join(
                "xf-carousel relative isolate overflow-hidden rounded-lg border border-border-light bg-bg-surface text-text-primary shadow-[0_1px_2px_rgb(15_23_42/.04)]",
                Disabled ? "opacity-60" : "",
                Class);

        private string ViewportClass =>
            Join(
                "relative min-h-48 overflow-hidden",
                SwipeEnabled && !Disabled ? "touch-pan-y select-none" : "");

        private string TrackClass =>
            Transition switch
            {
                XFCarouselTransition.Fade => "relative min-h-48",
                XFCarouselTransition.None => "relative min-h-48",
                _ => Join("flex min-h-48 transition-transform duration-500 ease-out", ReducedMotionClass)
            };

        private string ControlsClass => "pointer-events-none absolute inset-x-3 top-1/2 z-20 flex -translate-y-1/2 items-center justify-between";

        private string ControlButtonClass =>
            "pointer-events-auto inline-flex h-9 w-9 items-center justify-center rounded-full border border-border-light bg-bg-surface/90 text-text-primary shadow-[0_8px_18px_rgb(15_23_42/.12)] outline-none backdrop-blur transition-all duration-150 hover:bg-surface-100 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-45";

        private string IndicatorShellClass =>
            IndicatorPosition switch
            {
                XFCarouselIndicatorPosition.Top => "flex items-center justify-center gap-2 border-b border-border-light bg-bg-surface px-4 py-3",
                XFCarouselIndicatorPosition.Overlay => "absolute inset-x-0 bottom-0 z-20 flex items-center justify-center gap-2 bg-gradient-to-t from-surface-950/45 to-transparent px-4 py-4",
                _ => "flex items-center justify-center gap-2 border-t border-border-light bg-bg-surface px-4 py-3"
            };

        private string GetTrackStyle()
        {
            return Transition == XFCarouselTransition.Slide
                ? $"transform: translateX(-{currentIndex * 100}%);"
                : string.Empty;
        }

        internal void Register(XFCarouselSlide slide)
        {
            if (!slides.Contains(slide))
            {
                slides.Add(slide);
                NormalizeCurrentIndex();
                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal void Unregister(XFCarouselSlide slide)
        {
            if (slides.Remove(slide))
            {
                NormalizeCurrentIndex();
                _ = InvokeAsync(StateHasChanged);
            }
        }

        internal int GetSlideIndex(XFCarouselSlide slide) => slides.IndexOf(slide);

        internal string GetSlideDomId(int index) => $"{Id}-slide-{index}";

        protected override void OnInitialized()
        {
            currentIndex = Math.Max(0, SelectedIndex);
            previousSelectedIndexParameter = SelectedIndex;
        }

        protected override void OnParametersSet()
        {
            if (previousSelectedIndexParameter != SelectedIndex)
            {
                currentIndex = Math.Max(0, SelectedIndex);
                previousSelectedIndexParameter = SelectedIndex;
                NormalizeCurrentIndex();
            }

            ConfigureAutoplay();
        }

        private async Task GoToAsync(int index)
        {
            if (Disabled || !HasSlides)
            {
                return;
            }

            var nextIndex = ResolveIndex(index);

            if (nextIndex == currentIndex)
            {
                return;
            }

            previousIndex = currentIndex;
            currentIndex = nextIndex;
            previousSelectedIndexParameter = SelectedIndex;

            if (SelectedIndexChanged.HasDelegate)
            {
                await SelectedIndexChanged.InvokeAsync(currentIndex);
            }

            if (Change.HasDelegate)
            {
                await Change.InvokeAsync(new XFCarouselChangeEventArgs(currentIndex, slides[currentIndex].SlideId, previousIndex));
            }
        }

        private Task PreviousAsync()
        {
            return GoToAsync(currentIndex - 1);
        }

        private Task NextAsync()
        {
            return GoToAsync(currentIndex + 1);
        }

        private Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            return args.Key switch
            {
                "ArrowLeft" => PreviousAsync(),
                "ArrowRight" => NextAsync(),
                "Home" => GoToAsync(0),
                "End" => GoToAsync(slides.Count - 1),
                _ => Task.CompletedTask
            };
        }

        private void HandlePointerDown(PointerEventArgs args)
        {
            if (!CanHandleSwipe())
            {
                return;
            }

            swipeStartX = args.ClientX;
            swipeStartY = args.ClientY;
        }

        private async Task HandlePointerUpAsync(PointerEventArgs args)
        {
            if (!CanHandleSwipe() || swipeStartX is null || swipeStartY is null)
            {
                ClearSwipe();
                return;
            }

            var deltaX = args.ClientX - swipeStartX.Value;
            var deltaY = args.ClientY - swipeStartY.Value;
            ClearSwipe();

            if (Math.Abs(deltaX) < Math.Max(1, SwipeThreshold) || Math.Abs(deltaX) <= Math.Abs(deltaY))
            {
                return;
            }

            if (deltaX > 0)
            {
                await PreviousAsync();
            }
            else
            {
                await NextAsync();
            }
        }

        private void HandlePointerCancel()
        {
            ClearSwipe();
        }

        private bool CanHandleSwipe()
        {
            return SwipeEnabled && !Disabled && HasSlides && slides.Count > 1;
        }

        private void ClearSwipe()
        {
            swipeStartX = null;
            swipeStartY = null;
        }

        private void SetPointerPaused(bool value)
        {
            if (PauseOnHover)
            {
                isPointerPaused = value;
            }
        }

        private void SetFocusPaused(bool value)
        {
            if (PauseOnFocus)
            {
                isFocusPaused = value;
            }
        }

        private int ResolveIndex(int index)
        {
            if (slides.Count == 0)
            {
                return 0;
            }

            if (Loop)
            {
                return (index + slides.Count) % slides.Count;
            }

            return Math.Clamp(index, 0, slides.Count - 1);
        }

        private void NormalizeCurrentIndex()
        {
            if (slides.Count == 0)
            {
                currentIndex = 0;
                return;
            }

            currentIndex = ResolveIndex(currentIndex);
        }

        private string GetIndicatorClass(int index)
        {
            return index == currentIndex
                ? "h-2.5 w-7 rounded-full bg-primary outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2"
                : "h-2.5 w-2.5 rounded-full bg-surface-300 outline-none transition-all duration-150 hover:bg-primary/70 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2";
        }

        private string GetIndicatorLabel(int index)
        {
            return string.Format(CultureInfo.CurrentCulture, IndicatorLabelFormat, index + 1);
        }

        private void ConfigureAutoplay()
        {
            if (!ShouldAutoplay)
            {
                StopAutoplay();
                return;
            }

            autoplayCancellation ??= new CancellationTokenSource();
            if (!autoplayCancellation.IsCancellationRequested && !autoplayRunning)
            {
                _ = RunAutoplayAsync(autoplayCancellation.Token);
            }
        }

        private async Task RunAutoplayAsync(CancellationToken cancellationToken)
        {
            autoplayRunning = true;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(Math.Max(1000, Interval), cancellationToken);

                    if (!ShouldAutoplay || IsAutoplayPaused)
                    {
                        continue;
                    }

                    await InvokeAsync(NextAsync);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                autoplayRunning = false;
            }
        }

        private void StopAutoplay()
        {
            if (autoplayCancellation is null)
            {
                return;
            }

            autoplayCancellation.Cancel();
            autoplayCancellation.Dispose();
            autoplayCancellation = null;
            autoplayRunning = false;
        }

        public ValueTask DisposeAsync()
        {
            StopAutoplay();
            return ValueTask.CompletedTask;
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
