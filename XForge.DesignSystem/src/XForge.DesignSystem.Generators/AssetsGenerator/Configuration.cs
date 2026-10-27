namespace XForge.AssetsGenerator;

/// <summary>
/// Settings bound from CLI args via Microsoft.Extensions.Configuration.
/// </summary>
public sealed class GeneratorSettings
{
    public string? Assets { get; set; }
    public string? Target { get; set; }
    public string Namespace { get; set; } = "XForge.DesignSystem.Components.Icon";
    public string? Sizes { get; set; }
    public string? Names { get; set; }
    public string? Library { get; set; }
}

public sealed class Icon
{
    public string Name { get; set; } = "";
    public string Variant { get; set; } = "";
    public int Size { get; set; }
    public string SvgPath { get; set; } = "";
}

public sealed class Emoji
{
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public string Style { get; set; } = "";
    public string Skintone { get; set; } = "Default";
    public int Size { get; set; }
    public string SvgPath { get; set; } = "";
}