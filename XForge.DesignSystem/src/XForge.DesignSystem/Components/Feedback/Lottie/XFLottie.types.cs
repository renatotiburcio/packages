namespace XForge.DesignSystem.Components.Feedback.Lottie
{
    /// <summary>
    /// Renderer type for Lottie animations.
    /// </summary>
    public enum XFLottieRenderer
    {
        /// <summary>
        /// SVG-based renderer (default, best quality).
        /// </summary>
        Svg,

        /// <summary>
        /// Canvas-based renderer (better performance for complex animations).
        /// </summary>
        Canvas,

        /// <summary>
        /// HTML-based renderer.
        /// </summary>
        Html
    }

    /// <summary>
    /// Playback direction for Lottie animations.
    /// </summary>
    public enum XFLottieDirection
    {
        /// <summary>
        /// Play forward (default).
        /// </summary>
        Forward = 1,

        /// <summary>
        /// Play backward.
        /// </summary>
        Backward = -1
    }

    /// <summary>
    /// Event arguments when a Lottie animation finishes loading.
    /// </summary>
    public record XFLottieLoadedEventArgs
    {
        /// <summary>
        /// Total number of frames in the animation.
        /// </summary>
        public double TotalFrames { get; init; }

        /// <summary>
        /// Current frame at load time.
        /// </summary>
        public double CurrentFrame { get; init; }

        /// <summary>
        /// Frame rate of the animation.
        /// </summary>
        public double FrameRate { get; init; }
    }
}
