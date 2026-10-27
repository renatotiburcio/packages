using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates that a numeric field value is within a specified range.
    /// </summary>
    public partial class XFRangeValidator : XFValidatorBase
    {
        /// <summary>Minimum allowed value (inclusive). Null means no minimum.</summary>
        [Parameter] public double? Min { get; set; }

        /// <summary>Maximum allowed value (inclusive). Null means no maximum.</summary>
        [Parameter] public double? Max { get; set; }

        /// <summary>When true, null values pass validation.</summary>
        [Parameter] public bool AllowNull { get; set; } = true;

        protected override bool Validate(object? value)
        {
            if (value is null) return AllowNull;

            double numeric;
            try
            {
                numeric = Convert.ToDouble(value);
            }
            catch
            {
                return false;
            }

            if (Min is not null && numeric < Min.Value) return false;
            if (Max is not null && numeric > Max.Value) return false;
            return true;
        }
    }
}
