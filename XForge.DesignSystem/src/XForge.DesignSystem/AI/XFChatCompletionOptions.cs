namespace XForge.DesignSystem.AI;

/// <summary>
/// Configuration options for the <see cref="XFChatCompletionService"/>.
/// </summary>
public class XFChatCompletionOptions
{
    /// <summary>
    /// Gets or sets the endpoint URL for the AI service (e.g., OpenAI API URL).
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proxy URL for the AI service, if any. If set, this will override the Endpoint.
    /// </summary>
    public string? Proxy { get; set; }

    /// <summary>
    /// Gets or sets the API key for authentication with the AI service.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the header name for the API key (e.g., 'Authorization' or 'api-key').
    /// </summary>
    public string ApiKeyHeader { get; set; } = "Authorization";

    /// <summary>
    /// Gets or sets the model name to use for executing chat completions (e.g., 'gpt-4o').
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the system prompt for the AI assistant.
    /// </summary>
    public string SystemPrompt { get; set; } = "You are a helpful AI assistant.";

    /// <summary>
    /// Gets or sets the temperature for the AI model (0.0 to 2.0).
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate in the response.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages to keep in conversation memory.
    /// </summary>
    public int MaxMessages { get; set; } = 50;

    /// <summary>
    /// Gets or sets the maximum age in hours for conversation sessions before cleanup.
    /// </summary>
    public int SessionMaxAgeHours { get; set; } = 24;
}
