using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates that a field value matches a regular expression pattern.
    /// </summary>
    public partial class XFRegexValidator : XFValidatorBase
    {
        /// <summary>The regex pattern to match against.</summary>
        [Parameter] public string Pattern { get; set; } = ".*";

        protected override bool Validate(object? value)
        {
            if (value is null) return true; // null passes regex; use XFRequiredValidator for non-null
            var text = value.ToString() ?? string.Empty;
            return Regex.IsMatch(text, Pattern);
        }
    }
}
