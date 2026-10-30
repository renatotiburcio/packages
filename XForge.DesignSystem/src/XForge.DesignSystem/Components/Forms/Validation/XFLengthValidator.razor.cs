using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Forms.Validation
{
    /// <summary>
    /// Validates that a string field value length is within a specified range.
    /// </summary>
    public partial class XFLengthValidator : XFValidatorBase
    {
        /// <summary>Minimum allowed length (inclusive). Null means no minimum.</summary>
        [Parameter] public int? Min { get; set; }

        /// <summary>Maximum allowed length (inclusive). Null means no maximum.</summary>
        [Parameter] public int? Max { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Min, Max);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override bool Validate(object? value)
        {
            if (value is null) return true; // null passes; use XFRequiredValidator for non-null
            var text = value.ToString() ?? string.Empty;
            if (Min is not null && text.Length < Min.Value) return false;
            if (Max is not null && text.Length > Max.Value) return false;
            return true;
        }
    }
}
