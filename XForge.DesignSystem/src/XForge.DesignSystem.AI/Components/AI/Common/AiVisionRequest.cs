using System.Collections.Generic;

namespace XForge.DesignSystem.Components.AI.Common;

public sealed record AiVisionRequest
{
    public required string Prompt { get; init; }
    public required IReadOnlyList<string> Images { get; init; }
    public IReadOnlyList<string>? MimeTypes { get; init; }
    public double? Temperature { get; init; }
    public int? MaxTokens { get; init; }
}
