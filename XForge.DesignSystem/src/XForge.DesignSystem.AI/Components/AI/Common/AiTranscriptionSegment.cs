using System.Collections.Generic;

namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiTranscriptionSegment
{
    public string Text { get; init; } = string.Empty;
    public float? Start { get; init; }
    public float? End { get; init; }
    public IReadOnlyList<AiTranscriptionWord>? Words { get; init; }
}
