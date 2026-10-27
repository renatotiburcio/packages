namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiTranscriptionWord
{
    public string Text { get; init; } = string.Empty;
    public float? Start { get; init; }
    public float? End { get; init; }
    public float Confidence { get; init; }
}
