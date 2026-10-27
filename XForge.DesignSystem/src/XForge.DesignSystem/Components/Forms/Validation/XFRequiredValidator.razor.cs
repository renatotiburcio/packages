using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates that a field value is not null, empty, or equal to a default value.
    /// </summary>
    public partial class XFRequiredValidator : XFValidatorBase
    {
        /// <summary>A value that should be treated as "empty" (e.g., 0 for int).</summary>
        [Parameter] public object? DefaultValue { get; set; }

        protected override bool Validate(object? value)
        {
            if (value is null) return false;
            if (value is string s && string.IsNullOrWhiteSpace(s)) return false;
            if (DefaultValue is not null && Equals(value, DefaultValue)) return false;
            return true;
        }
    }
}
