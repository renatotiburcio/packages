namespace XForge.DesignSystem.Components.Forms.PasswordStrength
{
    public enum XFPasswordStrengthLevel
    {
        None,
        Weak,
        Fair,
        Good,
        Strong
    }

    public sealed record XFPasswordStrengthChangedEventArgs(
        string? Value,
        XFPasswordStrengthLevel Level,
        int Score,
        int PassedRules,
        int TotalRules,
        bool IsValid);
}
