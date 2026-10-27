namespace XForge.DesignSystem.Components.Forms.Login
{
    /// <summary>
    /// Arguments passed when the <see cref="XFLogin"/> component triggers the <c>OnLogin</c> event.
    /// </summary>
    public class XFLoginArgs
    {
        /// <summary>
        /// The username entered by the user.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The password entered by the user.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Whether the user opted to be remembered.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
