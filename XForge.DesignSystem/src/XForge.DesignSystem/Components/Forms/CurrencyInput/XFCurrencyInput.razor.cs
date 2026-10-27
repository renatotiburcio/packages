using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Linq.Expressions;
using XForge.DesignSystem.Components.Forms.NumberInput;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Forms.CurrencyInput
{
    public partial class XFCurrencyInput : XFInputComponentBase
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public decimal? Value { get; set; }
        [Parameter] public EventCallback<decimal?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<decimal?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<decimal?> Change { get; set; }
        [Parameter] public decimal? Min { get; set; }
        [Parameter] public decimal? Max { get; set; }
        [Parameter] public decimal? Step { get; set; }
        [Parameter] public int Scale { get; set; } = 2;
        [Parameter] public int? Precision { get; set; } = 14;
        [Parameter] public string? CultureName { get; set; }
        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        [Parameter] public string? CurrencySymbol { get; set; }
        [Parameter] public string? CurrencyCode { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public XFNumberInputTextAlign TextAlign { get; set; } = XFNumberInputTextAlign.Right;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Clamp { get; set; } = true;
        [Parameter] public bool ShowSteppers { get; set; } = true;
        [Parameter] public XFStepperLayout StepperLayout { get; set; } = XFStepperLayout.Stacked;
        [Parameter] public bool AutoDecimalSeparator { get; set; }
        [Parameter] public string IncrementLabel { get; set; } = "Increase value";
        [Parameter] public string DecrementLabel { get; set; } = "Decrease value";
        [Parameter] public string? AriaValueTextFormat { get; set; }

        private string? ResolvedCurrencySymbol
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CurrencySymbol))
                {
                    return CurrencySymbol;
                }

                if (!string.IsNullOrWhiteSpace(CultureName))
                {
                    return CultureInfo.GetCultureInfo(CultureName).NumberFormat.CurrencySymbol;
                }

                return Culture.NumberFormat.CurrencySymbol;
            }
        }

        private string? ResolvedSuffixText => string.IsNullOrWhiteSpace(CurrencyCode) ? null : CurrencyCode;
    }
}
