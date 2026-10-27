namespace XForge.DesignSystem.Components.AI.Avatars;

/// <summary>
/// Presence/online status indicators for AI avatar.
/// </summary>
public enum XFAiAvatarStatus
{
    Online, Busy, Away, Offline, Hidden
}

/// <summary>
/// Event arguments when an AI avatar is clicked.
/// </summary>
public sealed record XFAiAvatarClickEventArgs
{
    public required string AgentName { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
