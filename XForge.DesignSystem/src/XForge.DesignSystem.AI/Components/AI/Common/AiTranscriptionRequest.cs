using System.IO;

namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiTranscriptionRequest
{
    public string? Audio { get; init; }
    public Stream? AudioStream { get; init; }
    public byte[]? AudioBytes { get; init; }
    public string? Language { get; init; }
    public string? ContentType { get; init; }
    public bool IncludeTimestamps { get; init; }
    public string? Format { get; init; }
}
