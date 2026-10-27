using System;
using XForge.DesignSystem.Shared.Contracts;
namespace XForge.DesignSystem.Shared.Setup;

/// <summary>
/// Global static defaults for all XForge components (Havit HxSetup pattern).
/// Lower-precedence fallback: instance > settings > defaults > global.
/// </summary>
public static class XForgeSetup
{
    public static class Defaults
    {
        public static string Placeholder { get; set; } = string.Empty;
        public static InputSize InputSize { get; set; } = InputSize.Regular;
        public static LabelType LabelType { get; set; } = LabelType.Automatic;
    }
}

public enum InputSize { Small, Regular, Large }
public enum LabelType { Automatic, Floating, Stacked, Hidden }