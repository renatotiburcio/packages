using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Captcha
{
    /// <summary>
    /// Google reCAPTCHA v2 component. Renders a captcha widget using JS interop.
    /// Requires a valid site key from https://www.google.com/recaptcha/admin.
    /// </summary>
    public partial class XFCaptcha : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private IJSObjectReference? _module;
        private DotNetObjectReference<XFCaptcha>? _dotNetRef;
        private ElementReference _containerRef;

        #region Parameters

        /// <summary>
        /// Google reCAPTCHA site key. Required.
        /// </summary>
        [Parameter, EditorRequired]
        public string SiteKey { get; set; } = default!;

        /// <summary>
        /// Color theme of the widget. Default: Light.
        /// </summary>
        [Parameter]
        public XFCaptchaTheme Theme { get; set; } = XFCaptchaTheme.Light;

        /// <summary>
        /// Size of the widget. Default: Normal.
        /// </summary>
        [Parameter]
        public XFCaptchaSize Size { get; set; } = XFCaptchaSize.Normal;

        /// <summary>
        /// Badge position for invisible reCAPTCHA. Default: BottomRight.
        /// </summary>
        [Parameter]
        public XFCaptchaBadgePosition BadgePosition { get; set; } = XFCaptchaBadgePosition.BottomRight;

        /// <summary>
        /// Language code for the widget (e.g., "pt-BR", "en"). Default: auto-detect.
        /// </summary>
        [Parameter]
        public string? LanguageCode { get; set; }

        /// <summary>
        /// Accessible label for the captcha region. Default: "Verificação de segurança".
        /// </summary>
        [Parameter]
        public string AriaLabel { get; set; } = "Verificação de segurança";

        /// <summary>
        /// CSS class for the container div.
        /// </summary>
        [Parameter]
        public string? ContainerClass { get; set; }

        /// <summary>
        /// Event raised when the captcha is solved. Provides the response token.
        /// </summary>
        [Parameter]
        public EventCallback<XFCaptchaState> OnSolved { get; set; }

        /// <summary>
        /// Event raised when the captcha expires.
        /// </summary>
        [Parameter]
        public EventCallback OnExpired { get; set; }

        #endregion

        #region Public Properties and Methods

        /// <summary>
        /// Current state of the captcha.
        /// </summary>
        public XFCaptchaState State { get; private set; } = new();

        /// <summary>
        /// Resets the captcha widget.
        /// </summary>
        public async Task ResetAsync()
        {
            State = new XFCaptchaState();
            if (_module is not null)
            {
                try
                {
                    await _module.InvokeVoidAsync("resetCaptcha", _containerRef);
                }
                catch (JSDisconnectedException) { }
            }
        }

        /// <summary>
        /// Executes invisible reCAPTCHA programmatically.
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (_module is not null)
            {
                try
                {
                    await _module.InvokeVoidAsync("executeCaptcha", _containerRef);
                }
                catch (JSDisconnectedException) { }
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
                        "import", "./_content/XForge.DesignSystem/js/xf-captcha.js");
                    _dotNetRef = DotNetObjectReference.Create(this);

                    var themeStr = Theme.ToString().ToLowerInvariant();
                    var sizeStr = Size.ToString().ToLowerInvariant();
                    var badgeStr = BadgePosition switch
                    {
                        XFCaptchaBadgePosition.BottomLeft => "bottomleft",
                        XFCaptchaBadgePosition.Inline => "inline",
                        _ => "bottomright"
                    };

                    await _module.InvokeVoidAsync("renderCaptcha",
                        _containerRef, SiteKey, themeStr, sizeStr, badgeStr, LanguageCode, _dotNetRef);
                }
                catch
                {
                    // JS not available during prerender
                }
            }
        }

        #endregion

        #region JS Callbacks

        /// <summary>
        /// Called from JS when captcha is solved.
        /// </summary>
        [JSInvokable]
        public async Task OnSuccessHandler(string response)
        {
            State.Response = response;
            State.Valid = true;

            if (OnSolved.HasDelegate)
                await OnSolved.InvokeAsync(State);
        }

        /// <summary>
        /// Called from JS when captcha expires.
        /// </summary>
        [JSInvokable]
        public async Task OnExpiredHandler()
        {
            State.Valid = false;
            State.Response = string.Empty;

            if (OnExpired.HasDelegate)
                await OnExpired.InvokeAsync();
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                try
                {
                    await _module.InvokeVoidAsync("destroyCaptcha", _containerRef);
                    await _module.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
                _module = null;
            }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }

        #endregion
    }
}
