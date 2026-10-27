namespace XForge.DesignSystem.Components.Forms.NumberInput
{
    public enum XFNumberInputFormatStyle
    {
        Number,
        Currency,
        Percent
    }

    public enum XFNumberInputTextAlign
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Controls the positioning of stepper (+/-) buttons around a numeric input.
    /// </summary>
    public enum XFStepperLayout
    {
        /// <summary>Both buttons stacked vertically on the right side (default). Matches all reference libraries.</summary>
        Stacked,
        /// <summary>Decrement button on the left, increment button on the right (sandwich). Optimized for mobile touch.</summary>
        Split,
        /// <summary>Split on mobile (&lt;md breakpoint), Stacked on desktop (≥md). Responsive default.</summary>
        Auto
    }
}
