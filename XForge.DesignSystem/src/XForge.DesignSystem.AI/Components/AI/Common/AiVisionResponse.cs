namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiVisionResponse
{
    public string Text { get; init; } = string.Empty;
    public string? StructuredData { get; init; }
    public float? Confidence { get; init; }
}
