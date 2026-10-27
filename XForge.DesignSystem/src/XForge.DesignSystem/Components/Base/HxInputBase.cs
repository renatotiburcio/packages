using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using XForge.DesignSystem.Shared.Contracts;
using XForge.DesignSystem.Shared.Setup;
namespace XForge.DesignSystem.Components.Base;
public abstract class HxInputBase<TValue> : InputBase<TValue>, IInputWithPlaceholder, IInputWithSize, IInputWithLabelType
{
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public InputSize? InputSize { get; set; }
    [Parameter] public LabelType? LabelType { get; set; }
    [CascadingParameter] public XFEditContext? FormState { get; set; }
    protected string PlaceholderEffective => Placeholder ?? FormState?.Placeholder ?? XForgeSetup.Defaults.Placeholder;
    protected InputSize InputSizeEffective => InputSize ?? FormState?.InputSize ?? XForgeSetup.Defaults.InputSize;
    protected LabelType LabelTypeEffective => LabelType ?? FormState?.LabelType ?? XForgeSetup.Defaults.LabelType;
    [UnconditionalSuppressMessage("Trimming", "IL2091", Justification = "BindConverter.TryConvertTo is part of Microsoft.AspNetCore.Components and is AOT-compatible; TValue is a user-provided type.")]
    protected override bool TryParseValueFromString(string? value, out TValue result, out string validationErrorMessage)
    {
        if (BindConverter.TryConvertTo<TValue>(value, System.Globalization.CultureInfo.CurrentCulture, out var converted)) { result = converted!; validationErrorMessage = string.Empty; return true; }
        result = default!; validationErrorMessage = $"Cannot convert {value} to {typeof(TValue).Name}"; return false;
    }
}
public class XFEditContext { public string? Placeholder { get; init; } public InputSize? InputSize { get; init; } public LabelType? LabelType { get; init; } }