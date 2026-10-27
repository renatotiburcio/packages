using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates a field value using a custom delegate.
    /// </summary>
    public partial class XFCustomValidator : XFValidatorBase
    {
        /// <summary>A function that returns true when the value is valid.</summary>
        [Parameter] public Func<object?, bool> Validator { get; set; } = _ => true;

        protected override bool Validate(object? value)
        {
            return Validator.Invoke(value);
        }
    }
}
