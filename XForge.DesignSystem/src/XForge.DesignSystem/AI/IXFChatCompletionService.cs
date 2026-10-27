using System.Collections.Generic;
using System.Threading;

namespace XForge.DesignSystem.AI;

/// <summary>
/// Interface for getting chat completions from an AI model with conversation memory.
/// </summary>
public interface IXFChatCompletionService
{
    /// <summary>
    /// Streams chat completion responses from the AI model asynchronously with conversation memory.
    /// </summary>
    IAsyncEnumerable<string> GetCompletionsAsync(
        string userInput,
        string? sessionId = null,
        CancellationToken cancellationToken = default,
        string? model = null,
        string? systemPrompt = null,
        double? temperature = null,
        int? maxTokens = null,
        string? endpoint = null,
        string? proxy = null,
        string? apiKey = null,
        string? apiKeyHeader = null);

    /// <summary>
    /// Gets or creates a conversation session.
    /// </summary>
    XFConversationSession GetOrCreateSession(string? sessionId = null);

    /// <summary>
    /// Clears the conversation history for a specific session.
    /// </summary>
    void ClearSession(string sessionId);

    /// <summary>
    /// Gets all active conversation sessions.
    /// </summary>
    IEnumerable<XFConversationSession> GetActiveSessions();

    /// <summary>
    /// Removes old conversation sessions based on age.
    /// </summary>
    void CleanupOldSessions(int maxAgeHours = 24);
}
