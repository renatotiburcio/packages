using XForge.DesignSystem.Components.AI.Common;

namespace XForge.DesignSystem.Components.AI.MultiModal;

/// <summary>
/// Type of attachment in a multi-modal message.
/// </summary>
public enum XFAiAttachmentType
{
    /// <summary>Image attachment (base64).</summary>
    Image,
    /// <summary>Audio attachment (base64).</summary>
    Audio,
    /// <summary>File attachment (text content).</summary>
    File,
    /// <summary>URL reference.</summary>
    Url
}

/// <summary>
/// An attachment in a multi-modal message.
/// </summary>
public sealed record XFAiAttachment
{
    /// <summary>Type of attachment.</summary>
    public required XFAiAttachmentType Type { get; init; }

    /// <summary>Display name for the attachment.</summary>
    public required string Name { get; init; }

    /// <summary>Data as base64 string or text content.</summary>
    public required string Data { get; init; }

    /// <summary>MIME type (e.g., "image/png", "audio/mp3").</summary>
    public string? MimeType { get; init; }

    /// <summary>Size in bytes (if known).</summary>
    public long? SizeBytes { get; init; }
}

/// <summary>
/// A message in a multi-modal chat.
/// </summary>
public sealed record XFAiMultiModalMessage
{
    /// <summary>Message role.</summary>
    public required AiRole Role { get; init; }

    /// <summary>Text content.</summary>
    public required string Content { get; init; }

    /// <summary>Attachments (images, audio, files).</summary>
    public IReadOnlyList<XFAiAttachment>? Attachments { get; init; }

    /// <summary>Timestamp.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>Token usage for this message.</summary>
    public AiTokenUsage? Usage { get; init; }
}
