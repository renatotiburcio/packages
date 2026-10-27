using System;
using System.Collections.Generic;

namespace XForge.DesignSystem.AI;

/// <summary>
/// Represents a single message in an AI chat conversation.
/// </summary>
public class XFAIChatMessage
{
    /// <summary>Unique identifier for this message.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>The role of the message sender (user, assistant, system).</summary>
    public string Role { get; set; } = "user";

    /// <summary>The message content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Whether this message is from the user.</summary>
    public bool IsUser => Role == "user";

    /// <summary>Whether this message is from the assistant.</summary>
    public bool IsAssistant => Role == "assistant";

    /// <summary>Whether this message is from the system.</summary>
    public bool IsSystem => Role == "system";

    /// <summary>Whether the assistant is currently streaming this message.</summary>
    public bool IsStreaming { get; set; }

    /// <summary>Timestamp when the message was created.</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Represents a conversation session with memory.
/// </summary>
public class XFConversationSession
{
    /// <summary>Unique identifier for the conversation session.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>The list of messages in the conversation.</summary>
    public List<XFAIChatMessage> Messages { get; set; } = new();

    /// <summary>Timestamp when the conversation was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>Timestamp when the conversation was last updated.</summary>
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    /// <summary>Maximum number of messages to keep in memory.</summary>
    public int MaxMessages { get; set; } = 50;

    /// <summary>
    /// Adds a message to the conversation and manages memory limits.
    /// </summary>
    public void AddMessage(string role, string content)
    {
        Messages.Add(new XFAIChatMessage
        {
            Role = role,
            Content = content,
            Timestamp = DateTime.Now
        });

        LastUpdated = DateTime.Now;

        while (Messages.Count > MaxMessages)
        {
            Messages.RemoveAt(0);
        }
    }

    /// <summary>
    /// Clears all messages from the conversation.
    /// </summary>
    public void Clear()
    {
        Messages.Clear();
        LastUpdated = DateTime.Now;
    }

    /// <summary>
    /// Gets the conversation messages formatted for the AI API.
    /// </summary>
    public List<XFChatCompletionMessage> GetFormattedMessages(string systemPrompt)
    {
        var messages = new List<XFChatCompletionMessage>
        {
            new() { Role = "system", Content = systemPrompt }
        };

        foreach (var message in Messages)
        {
            messages.Add(new XFChatCompletionMessage { Role = message.Role, Content = message.Content });
        }

        return messages;
    }
}

/// <summary>
/// Represents a message in the chat completion API format.
/// </summary>
public class XFChatCompletionMessage
{
    /// <summary>The role of the message.</summary>
    public string? Role { get; set; }

    /// <summary>The content of the message.</summary>
    public string? Content { get; set; }
}

/// <summary>
/// Represents a chat completion API request.
/// </summary>
public class XFChatCompletionRequest
{
    /// <summary>The model to use.</summary>
    public string? Model { get; set; }

    /// <summary>The messages to send.</summary>
    public List<XFChatCompletionMessage>? Messages { get; set; }

    /// <summary>The temperature for sampling.</summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>The maximum tokens to generate.</summary>
    public int? MaxTokens { get; set; }

    /// <summary>Whether to stream the response.</summary>
    public bool Stream { get; set; } = true;
}
