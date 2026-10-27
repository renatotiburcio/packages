namespace XForge.DesignSystem.Core.Primitives;

/// <summary>
/// Shared Tailwind CSS class mappings for form input components.
/// Eliminates 15+ duplicate switch expressions across TextInput, Select, NumberInput,
/// DateInput, TimeInput, InputMask, Autocomplete, Combobox, MultiSelect, DatePicker,
/// FileInput, Upload, ColorInput, TimeSpan, Slider, RangeSlider, Textarea.
/// </summary>
public static class XFFormCss
{
    // Size
    public static string SizeClass(XFSize size) => size switch
    {
        XFSize.Sm => "h-9 py-1.5",
        XFSize.Lg => "h-12 py-2.5",
        _ => "h-10 py-2"
    };

    public static string SizeTextClass(XFSize size) => size switch
    {
        XFSize.Sm => "text-sm",
        XFSize.Lg => "text-base",
        _ => "text-sm"
    };

    // Variant
    public static string VariantClass(XFFormVariant variant) => variant switch
    {
        XFFormVariant.Filled => "border-transparent bg-surface-100 ring-1 ring-surface-200/70 shadow-[inset_0_1px_0_rgb(255_255_255/.55)]",
        XFFormVariant.Soft => "border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)] bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)] ring-1 ring-[color-mix(in_oklab,var(--color-primary)_14%,transparent)]",
        XFFormVariant.Ghost => "border-transparent bg-transparent ring-1 ring-transparent hover:bg-[color-mix(in_oklab,var(--color-primary)_5%,transparent)]",
        _ => "border-surface-200 bg-bg-surface ring-1 ring-surface-200/80 shadow-[0_1px_2px_rgb(15_23_42/.035)]"
    };

    // Tone
    public static string ValidationToneClass() =>
        "border-[color-mix(in_oklab,var(--color-danger)_45%,transparent)] ring-2 ring-[color-mix(in_oklab,var(--color-danger)_15%,transparent)] focus-within:border-[var(--color-danger)] focus-within:ring-[color-mix(in_oklab,var(--color-danger)_25%,transparent)]";

    public static string ToneClass(XFSemanticColor tone) => tone switch
    {
        XFSemanticColor.Primary => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]",
        XFSemanticColor.Secondary => "focus-within:border-[var(--color-secondary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-secondary)_16%,transparent)]",
        XFSemanticColor.Success => "focus-within:border-[var(--color-success)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-success)_16%,transparent)]",
        XFSemanticColor.Warning => "focus-within:border-[var(--color-warning)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-warning)_20%,transparent)]",
        XFSemanticColor.Danger => "focus-within:border-[var(--color-danger)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-danger)_16%,transparent)]",
        _ => "focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]"
    };

    // State
    public static string StateClass(bool disabled) => disabled
        ? "cursor-not-allowed opacity-60"
        : "hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_10%,transparent)]";

    // Common field shell base classes
    public const string FieldShellBase =
        "group relative flex items-center gap-2 rounded-lg border transition-all duration-200 ease-out";

    public const string SelectFieldShellBase =
        "group relative flex cursor-pointer items-center rounded-lg border transition-all duration-200 ease-out";

    // Common label / helper / error classes
    public const string LabelClass =
        "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";

    public const string HelperClass = "text-xs leading-5 text-text-secondary";

    public const string ErrorClass = "text-xs font-medium leading-5 text-danger";

    // Common clear button classes
    public const string ClearButtonClass =
        "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95";

    // Common adornment classes
    public const string AdornmentClass =
        "flex shrink-0 items-center text-text-muted transition-colors duration-150 group-focus-within:text-primary";

    // Common select native element classes
    public const string SelectNativeBase =
        "xf-select-native min-w-0 flex-1 cursor-pointer appearance-none self-stretch border-0 bg-transparent text-text-primary outline-none disabled:cursor-not-allowed disabled:text-text-muted";

    // Common option classes for select/combobox
    public const string OptionClass =
        "bg-bg-surface text-text-primary checked:bg-[var(--color-primary)] checked:text-white disabled:text-text-muted";
}
