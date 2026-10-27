using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.ColorInput
{
    public partial class XFColorInput : XFInputComponentBase
    {
        private const string FallbackColor = "#000000";
        private static readonly Regex HexColorRegex = new("^#?[0-9a-fA-F]{6}$", RegexOptions.Compiled);

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? PrefixText { get; set; }
        [Parameter] public string? SuffixText { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool Immediate { get; set; }
        [Parameter] public bool Clearable { get; set; }
        [Parameter] public bool ShowValue { get; set; } = true;
        [Parameter] public IReadOnlyList<string> Palette { get; set; } = Array.Empty<string>();
        [Parameter] public string ClearLabel { get; set; } = "Clear color";
        [Parameter] public string PaletteLabel { get; set; } = "Color samples";
        [Parameter] public string SwatchLabelFormat { get; set; } = "Select {0}";
        [Parameter] public XFSize SwatchSize { get; set; } = XFSize.Md;
        [Parameter] public XFShape SwatchShape { get; set; } = XFShape.Rounded;
        [Parameter] public string? SwatchButtonClass { get; set; }

        [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

        private string? CurrentValue { get; set; }
        private string? LastParameterValue { get; set; }
        private FieldIdentifier? FieldIdentifierValue { get; set; }

        private string HelperId => $"{Id}-helper";
        private string ErrorId => $"{Id}-error";
        private string PaletteId => $"{Id}-palette";
        private string AriaInvalid => HasValidationError ? "true" : "false";
        private string NativeValue => CurrentValue ?? FallbackColor;
        private bool CanClear => Clearable && CurrentValue is not null && !ReadOnly && !Disabled;
        private bool HasPalette => Palette.Count > 0;

        private string? FirstValidationMessage => HasValidationError
            ? CascadedEditContext!.GetValidationMessages(FieldIdentifierValue!.Value).FirstOrDefault()
            : null;

        private bool HasValidationError =>
            CascadedEditContext is not null
            && ValueExpression is not null
            && FieldIdentifierValue.HasValue
            && CascadedEditContext.GetValidationMessages(FieldIdentifierValue.Value).Any();

        private string? DescribedBy
        {
            get
            {
                var ids = new List<string>();

                if (HasValidationError)
                {
                    ids.Add(ErrorId);
                }
                else if (!string.IsNullOrWhiteSpace(HelperText))
                {
                    ids.Add(HelperId);
                }

                if (HasPalette)
                {
                    ids.Add(PaletteId);
                }

                return ids.Count == 0 ? null : string.Join(" ", ids);
            }
        }

        private string RootClass =>
            Join("xf-color-input flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);

        private string LabelClass =>
            "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

        private string FieldShellClass =>
            Join(
                "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out",
                SizeClass,
                VariantClass,
                ToneClass,
                StateClass);

        private string ColorInputClass =>
            Join(
                "shrink-0 cursor-pointer appearance-none overflow-hidden rounded-md border border-surface-200 p-0 outline-none transition-all duration-150 disabled:cursor-not-allowed",
                "scheme-light dark:scheme-dark [&::-moz-color-swatch]:border-0 [&::-moz-color-swatch]:rounded-md [&::-webkit-color-swatch]:border-0 [&::-webkit-color-swatch]:rounded-md [&::-webkit-color-swatch-wrapper]:p-0",
                ColorInputSizeClass);

        private string ValueClass =>
            Join(
                "min-w-0 flex-1 border-0 bg-transparent p-0 font-mono text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted",
                SizeTextClass);

        private string AdornmentClass =>
            "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

        private string ClearButtonClass =>
            "inline-flex h-6 w-6 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-primary/10 hover:ring-primary/20 hover:text-primary focus-visible:ring-2 focus-visible:ring-primary/25 active:scale-95";

        private string SwatchListClass =>
            "flex flex-wrap gap-1.5";

        private string SwatchBaseClass =>
            "border border-surface-200 shadow-[inset_0_0_0_1px_rgb(255_255_255/.35)] outline-none transition-all duration-150 hover:-translate-y-0.5 focus-visible:ring-2 focus-visible:ring-primary/35 disabled:cursor-not-allowed disabled:opacity-60";

        private string SwatchSizeClass => SwatchSize switch
        {
            XFSize.Sm => "h-6 w-6",
            XFSize.Lg => "h-10 w-10",
            _ => "h-8 w-8"
        };

        private string SwatchShapeClass => SwatchShape switch
        {
            XFShape.Square => "rounded-none",
            XFShape.Circle => "rounded-full",
            XFShape.Pill => "rounded-full",
            _ => "rounded-lg"
        };

        private string GetSwatchButtonClass(string color)
        {
            var isSelected = NormalizeColor(color) == CurrentValue;
            var ringClass = isSelected
                ? "ring-2 ring-primary ring-offset-1 ring-offset-bg-surface"
                : "ring-1 ring-transparent hover:ring-primary/25";
            var custom = string.IsNullOrWhiteSpace(SwatchButtonClass) ? null : SwatchButtonClass;
            return Join(SwatchBaseClass, SwatchSizeClass, SwatchShapeClass, ringClass, custom);
        }

        private string HelperClass => "text-xs leading-5 text-text-secondary";

        private string ErrorClass => "text-xs font-medium leading-5 text-danger";

        private string SizeClass => Size switch
        {
            XFSize.Sm => "min-h-9 py-1.5 pl-2 pr-3",
            XFSize.Lg => "min-h-12 py-2.5 pl-2.5 pr-4",
            _ => "min-h-10 py-2 pl-2 pr-3.5"
        };

        private string ColorInputSizeClass => Size switch
        {
            XFSize.Sm => "h-7 w-9",
            XFSize.Lg => "h-9 w-12",
            _ => "h-8 w-11"
        };

        private string SizeTextClass => Size switch
        {
            XFSize.Sm => "text-sm",
            XFSize.Lg => "text-base",
            _ => "text-sm"
        };

        private string VariantClass => Variant switch
        {
            XFFormVariant.Filled => "border-transparent bg-surface-100 ring-1 ring-surface-200/70 shadow-[inset_0_1px_0_rgb(255_255_255/.55)]",
            XFFormVariant.Soft => "border-primary/35 bg-primary/5 ring-1 ring-primary/14",
            XFFormVariant.Ghost => "border-transparent bg-transparent ring-1 ring-transparent hover:bg-primary/5",
            _ => "border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]"
        };

        private string ToneClass
        {
            get
            {
                if (HasValidationError)
                {
                    return "border-danger/45 ring-2 ring-danger/15 focus-within:border-danger focus-within:ring-danger/25";
                }

                return Tone switch
                {
                    XFSemanticColor.Primary => "focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/18",
                    XFSemanticColor.Secondary => "focus-within:border-secondary focus-within:ring-2 focus-within:ring-secondary/16",
                    XFSemanticColor.Success => "focus-within:border-success focus-within:ring-2 focus-within:ring-success/16",
                    XFSemanticColor.Warning => "focus-within:border-warning focus-within:ring-2 focus-within:ring-warning/20",
                    XFSemanticColor.Danger => "focus-within:border-danger focus-within:ring-2 focus-within:ring-danger/16",
                    _ => "focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/18"
                };
            }
        }

        private string StateClass => Disabled
            ? "cursor-not-allowed opacity-60"
            : "hover:border-primary/45 hover:ring-primary/10";

        private string NativeInputStyle => $"background-color: {NativeValue};";

        protected override void OnParametersSet()
        {
            var normalized = NormalizeColor(Value);

            if (normalized != LastParameterValue)
            {
                CurrentValue = normalized;
                LastParameterValue = normalized;
            }

            if (ValueExpression is not null)
            {
                FieldIdentifierValue = FieldIdentifier.Create(ValueExpression);
            }
        }

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            if (Immediate)
            {
                await SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
            }
        }

        private Task HandleChangeAsync(ChangeEventArgs args)
        {
            return SetValueFromStringAsync(args.Value?.ToString(), notifyChange: true);
        }

        private Task SelectSwatchAsync(string color)
        {
            return SetValueFromStringAsync(color, notifyChange: true);
        }

        private Task ClearAsync()
        {
            return SetValueAsync(null, notifyChange: true);
        }

        private Task SetValueFromStringAsync(string? value, bool notifyChange)
        {
            return SetValueAsync(NormalizeColor(value), notifyChange);
        }

        private async Task SetValueAsync(string? value, bool notifyChange)
        {
            CurrentValue = value;
            LastParameterValue = value;

            if (notifyChange && ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(value);
            }

            if (notifyChange && CascadedEditContext is not null && FieldIdentifierValue.HasValue)
            {
                CascadedEditContext.NotifyFieldChanged(FieldIdentifierValue.Value);
            }
        }

        private static string? NormalizeColor(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();

            if (!HexColorRegex.IsMatch(trimmed))
            {
                return null;
            }

            var hex = trimmed.StartsWith("#", StringComparison.Ordinal) ? trimmed : $"#{trimmed}";
            return hex.ToUpperInvariant();
        }

        private static string GetSwatchStyle(string color)
        {
            var normalized = NormalizeColor(color) ?? FallbackColor;
            return string.Create(CultureInfo.InvariantCulture, $"background-color: {normalized};");
        }

        private string GetSwatchLabel(string color)
        {
            return string.Format(CultureInfo.CurrentCulture, SwatchLabelFormat, NormalizeColor(color) ?? color);
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
