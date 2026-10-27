namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiSpeechResponse
{
    public byte[]? AudioBytes { get; init; }
    public string? ContentType { get; init; }
}
