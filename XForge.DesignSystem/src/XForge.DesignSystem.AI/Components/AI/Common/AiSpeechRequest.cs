namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiSpeechRequest
{
    public required string Text { get; init; }
    public string? Voice { get; init; }
    public string? Language { get; init; }
    public float? Speed { get; init; }
}
