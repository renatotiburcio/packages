namespace XForge.DesignSystem.Components.AI.Chat;

/// <summary>
/// Represents the current operational state of the XFAiChat component.
/// </summary>
public enum XFAiChatState
{
    /// <summary>Component is idle — no active request or streaming.</summary>
    Idle,

    /// <summary>A request has been sent and the component is waiting for a response.</summary>
    Loading,

    /// <summary>A streaming response is being received and accumulated.</summary>
    Streaming,

    /// <summary>The last request resulted in an error.</summary>
    Error,

    /// <summary>The last response completed successfully.</summary>
    Completed,

    /// <summary>The user cancelled an in-progress streaming response.</summary>
    Cancelled
}
