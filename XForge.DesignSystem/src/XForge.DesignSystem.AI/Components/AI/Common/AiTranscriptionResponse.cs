using System.Collections.Generic;

namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiTranscriptionResponse
{
    public string Text { get; init; } = string.Empty;
    public string? DetectedLanguage { get; init; }
    public float? DurationSeconds { get; init; }
    public IReadOnlyList<AiTranscriptionSegment>? Segments { get; init; }
}
