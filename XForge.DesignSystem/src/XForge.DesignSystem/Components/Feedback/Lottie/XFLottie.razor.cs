using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.Lottie
{
    /// <summary>
    /// Lottie animation player component. Renders JSON-based Lottie animations
    /// using lottie-web via JS interop.
    /// </summary>
    public partial class XFLottie : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private IJSObjectReference? _module;
        private IJSObjectReference? _animation;
        private DotNetObjectReference<XFLottie>? _dotNetRef;
        private ElementReference _containerRef;
        private bool _initialized;
        private string? _lastPath;

        #region Parameters

        /// <summary>
        /// URL or path to the Lottie JSON animation file. Required.
        /// </summary>
        [Parameter, EditorRequired]
        public string Path { get; set; } = default!;

        /// <summary>
        /// Whether the animation should start playing automatically. Default: true.
        /// </summary>
        [Parameter]
        public bool Autoplay { get; set; } = true;

        /// <summary>
        /// Whether the animation should loop infinitely. Default: true.
        /// </summary>
        [Parameter]
        public bool Loop { get; set; } = true;

        /// <summary>
        /// Playback speed multiplier. Default: 1.0 (normal speed).
        /// </summary>
        [Parameter]
        public double Speed { get; set; } = 1.0;

        /// <summary>
        /// Playback direction. Default: Forward.
        /// </summary>
        [Parameter]
        public XFLottieDirection Direction { get; set; } = XFLottieDirection.Forward;

        /// <summary>
        /// Renderer type for the animation. Default: Svg.
        /// </summary>
        [Parameter]
        public XFLottieRenderer Renderer { get; set; } = XFLottieRenderer.Svg;

        /// <summary>
        /// Whether the animation is paused. Default: false.
        /// </summary>
        [Parameter]
        public bool Paused { get; set; }

        /// <summary>
        /// Background color of the animation container. Optional.
        /// </summary>
        [Parameter]
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Accessible label for the animation. Default: "Animação".
        /// </summary>
        [Parameter]
        public string AriaLabel { get; set; } = "Animação";

        /// <summary>
        /// Custom CSS class for the container.
        /// </summary>
        [Parameter]
        public string? ContainerClass { get; set; }

        /// <summary>
        /// Event raised when the animation finishes loading.
        /// </summary>
        [Parameter]
        public EventCallback<XFLottieLoadedEventArgs> OnLoaded { get; set; }

        /// <summary>
        /// Event raised when the animation completes (non-looping).
        /// </summary>
        [Parameter]
        public EventCallback OnCompleted { get; set; }

        /// <summary>
        /// Event raised when a loop iteration completes.
        /// </summary>
        [Parameter]
        public EventCallback OnLoopCompleted { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resumes animation playback.
        /// </summary>
        public async Task PlayAsync()
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("play");
                Paused = false;
            }
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public async Task PauseAsync()
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("pause");
                Paused = true;
            }
        }

        /// <summary>
        /// Stops the animation and resets to the first frame.
        /// </summary>
        public async Task StopAsync()
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("stop");
                Paused = true;
            }
        }

        /// <summary>
        /// Seeks to a specific frame.
        /// </summary>
        /// <param name="frame">Target frame number.</param>
        /// <param name="play">Whether to play after seeking.</param>
        public async Task GoToFrameAsync(double frame, bool play = false)
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("goToAndStop", frame, play);
            }
        }

        /// <summary>
        /// Sets the playback speed.
        /// </summary>
        /// <param name="speed">Speed multiplier (1.0 = normal).</param>
        public async Task SetSpeedAsync(double speed)
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("setSpeed", speed);
                Speed = speed;
            }
        }

        /// <summary>
        /// Sets the playback direction.
        /// </summary>
        /// <param name="direction">Forward or Backward.</param>
        public async Task SetDirectionAsync(XFLottieDirection direction)
        {
            if (_animation is not null)
            {
                await _animation.InvokeVoidAsync("setDirection", (int)direction);
                Direction = direction;
            }
        }

        #endregion

        #region Lifecycle

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/XForge.DesignSystem/js/xf-lottie.js");
                    _dotNetRef = DotNetObjectReference.Create(this);
                    await InitializeAnimationAsync();
                    _initialized = true;
                }
                catch
                {
                    // JS not available during prerender
                }
            }
            else if (_initialized && _module is not null && _lastPath != Path)
            {
                // Path changed — reinitialize
                await InitializeAnimationAsync();
            }
        }

        private async Task InitializeAnimationAsync()
        {
            if (_module is null || _dotNetRef is null) return;

            // Destroy previous animation
            if (_animation is not null)
            {
                try { await _animation.InvokeVoidAsync("destroy"); }
                catch (Microsoft.JSInterop.JSDisconnectedException) { }
                catch (InvalidOperationException) { }
                try { await _animation.DisposeAsync(); }
                catch (Microsoft.JSInterop.JSDisconnectedException) { }
                _animation = null;
            }

            var rendererStr = Renderer.ToString().ToLowerInvariant();
            _animation = await _module.InvokeAsync<IJSObjectReference>(
                "createAnimation",
                _containerRef,
                Path,
                Autoplay && !Paused,
                Loop,
                Speed,
                (int)Direction,
                rendererStr,
                _dotNetRef);

            _lastPath = Path;
        }

        #endregion

        #region JS Callbacks

        /// <summary>
        /// Called from JS when the animation finishes loading.
        /// </summary>
        [JSInvokable]
        public async Task NotifyLoaded(double totalFrames, double currentFrame, double frameRate)
        {
            if (OnLoaded.HasDelegate)
            {
                await OnLoaded.InvokeAsync(new XFLottieLoadedEventArgs
                {
                    TotalFrames = totalFrames,
                    CurrentFrame = currentFrame,
                    FrameRate = frameRate
                });
            }
        }

        /// <summary>
        /// Called from JS when the animation completes (non-looping).
        /// </summary>
        [JSInvokable]
        public async Task NotifyCompleted()
        {
            if (OnCompleted.HasDelegate)
                await OnCompleted.InvokeAsync();
        }

        /// <summary>
        /// Called from JS when a loop iteration completes.
        /// </summary>
        [JSInvokable]
        public async Task NotifyLoopCompleted()
        {
            if (OnLoopCompleted.HasDelegate)
                await OnLoopCompleted.InvokeAsync();
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            if (_animation is not null)
            {
                try
                {
                    await _animation.InvokeVoidAsync("destroy");
                    await _animation.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
                _animation = null;
            }

            if (_module is not null)
            {
                try { await _module.DisposeAsync(); } catch (JSDisconnectedException) { }
                _module = null;
            }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }

        #endregion
    }
}
