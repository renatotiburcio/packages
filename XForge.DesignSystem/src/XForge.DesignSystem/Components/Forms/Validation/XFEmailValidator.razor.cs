using System.ComponentModel.DataAnnotations;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates that a field value is a valid email address format.
    /// </summary>
    public partial class XFEmailValidator : XFValidatorBase
    {
        private static readonly EmailAddressAttribute EmailAttribute = new();

        protected override bool Validate(object? value)
        {
            if (value is null) return true; // null passes; use XFRequiredValidator for non-null
            var text = value.ToString() ?? string.Empty;
            return EmailAttribute.IsValid(text);
        }
    }
}
