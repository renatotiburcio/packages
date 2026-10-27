using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.Login
{
    /// <summary>
    /// Login form card component. Renders a styled login form with username/password inputs,
    /// optional remember-me checkbox, and configurable action buttons.
    /// </summary>
    public partial class XFLogin : XForgeComponentBase
    {
        /// <summary>
        /// The base CSS class(es) for this component.
        /// </summary>
        protected override string BaseClass => "xf-login";

        /// <summary>
        /// The username value bound to the username input field.
        /// </summary>
        [Parameter]
        public string? Username { get; set; }

        /// <summary>
        /// Callback raised when the username value changes.
        /// </summary>
        [Parameter]
        public EventCallback<string?> UsernameChanged { get; set; }

        /// <summary>
        /// The password value bound to the password input field.
        /// </summary>
        [Parameter]
        public string? Password { get; set; }

        /// <summary>
        /// Callback raised when the password value changes.
        /// </summary>
        [Parameter]
        public EventCallback<string?> PasswordChanged { get; set; }

        /// <summary>
        /// Whether the "remember me" checkbox is checked.
        /// </summary>
        [Parameter]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Callback raised when the "remember me" checkbox value changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> RememberMeChanged { get; set; }

        /// <summary>
        /// Whether to display the "remember me" checkbox. Defaults to <c>true</c>.
        /// </summary>
        [Parameter]
        public bool AllowRememberMe { get; set; } = true;

        /// <summary>
        /// Whether to display the login button. Defaults to <c>true</c>.
        /// </summary>
        [Parameter]
        public bool ShowLoginButton { get; set; } = true;

        /// <summary>
        /// Whether to display the register link. Defaults to <c>false</c>.
        /// </summary>
        [Parameter]
        public bool ShowRegister { get; set; }

        /// <summary>
        /// Whether to display the reset password link. Defaults to <c>false</c>.
        /// </summary>
        [Parameter]
        public bool ShowResetPassword { get; set; }

        /// <summary>
        /// The text displayed on the login button. Defaults to "Entrar".
        /// </summary>
        [Parameter]
        public string LoginText { get; set; } = "Entrar";

        /// <summary>
        /// Callback raised when the login button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<XFLoginArgs> OnLogin { get; set; }

        /// <summary>
        /// Callback raised when the register link is clicked.
        /// </summary>
        [Parameter]
        public EventCallback OnRegister { get; set; }

        /// <summary>
        /// Callback raised when the reset password link is clicked.
        /// Passes the current username value as the argument.
        /// </summary>
        [Parameter]
        public EventCallback<string?> OnResetPassword { get; set; }

        /// <summary>
        /// ARIA label for accessibility. Applied as <c>aria-label</c> on the form element.
        /// </summary>
        [Parameter]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Computes the final CSS class list for the component's root element.
        /// </summary>
        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} w-full max-w-sm rounded-xl border border-surface-200 bg-bg-surface p-6 shadow-sm sm:p-8");
        }

        /// <summary>
        /// Handles the login form submission.
        /// </summary>
        private async Task HandleLoginAsync()
        {
            if (OnLogin.HasDelegate)
            {
                await OnLogin.InvokeAsync(new XFLoginArgs
                {
                    Username = Username,
                    Password = Password,
                    RememberMe = RememberMe
                });
            }
        }

        /// <summary>
        /// Handles the register link click.
        /// </summary>
        private async Task HandleRegisterAsync()
        {
            if (OnRegister.HasDelegate)
            {
                await OnRegister.InvokeAsync();
            }
        }

        /// <summary>
        /// Handles the reset password link click.
        /// </summary>
        private async Task HandleResetPasswordAsync()
        {
            if (OnResetPassword.HasDelegate)
            {
                await OnResetPassword.InvokeAsync(Username);
            }
        }
    }
}
