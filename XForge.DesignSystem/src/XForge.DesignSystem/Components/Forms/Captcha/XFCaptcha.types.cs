namespace XForge.DesignSystem.Components.Forms.Captcha
{
    /// <summary>
    /// Theme for the reCAPTCHA widget.
    /// </summary>
    public enum XFCaptchaTheme
    {
        /// <summary>
        /// Light theme (default).
        /// </summary>
        Light,

        /// <summary>
        /// Dark theme.
        /// </summary>
        Dark
    }

    /// <summary>
    /// Size for the reCAPTCHA widget.
    /// </summary>
    public enum XFCaptchaSize
    {
        /// <summary>
        /// Normal size (default).
        /// </summary>
        Normal,

        /// <summary>
        /// Compact size.
        /// </summary>
        Compact
    }

    /// <summary>
    /// Badge position for invisible reCAPTCHA.
    /// </summary>
    public enum XFCaptchaBadgePosition
    {
        /// <summary>
        /// Bottom-right corner (default).
        /// </summary>
        BottomRight,

        /// <summary>
        /// Bottom-left corner.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Inline (positioned via CSS).
        /// </summary>
        Inline
    }

    /// <summary>
    /// State of the captcha (valid + response token).
    /// </summary>
    public record XFCaptchaState
    {
        /// <summary>
        /// Whether the captcha validation passed.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// The response token from the captcha API.
        /// </summary>
        public string Response { get; set; } = string.Empty;
    }
}
