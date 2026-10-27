using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace XForge.DesignSystem.AI;

/// <summary>
/// Service for interacting with AI chat models to get completions with conversation memory.
/// Supports OpenAI-compatible APIs with SSE streaming.
/// </summary>
public class XFChatCompletionService : IXFChatCompletionService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly XFChatCompletionOptions _options;
    private readonly Dictionary<string, XFConversationSession> _sessions = new();
    private readonly object _sessionsLock = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public XFChatCompletionService(IServiceProvider serviceProvider, IOptions<XFChatCompletionOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetCompletionsAsync(
        string userInput,
        string? sessionId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default,
        string? model = null,
        string? systemPrompt = null,
        double? temperature = null,
        int? maxTokens = null,
        string? endpoint = null,
        string? proxy = null,
        string? apiKey = null,
        string? apiKeyHeader = null)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            throw new ArgumentException("User input cannot be null or empty.", nameof(userInput));
        }

        var session = GetOrCreateSession(sessionId);
        session.AddMessage("user", userInput);

        var url = proxy ?? _options.Proxy ?? endpoint ?? _options.Endpoint;
        var effectiveApiKey = apiKey ?? _options.ApiKey;
        var effectiveApiKeyHeader = apiKeyHeader ?? _options.ApiKeyHeader;

        var messages = session.GetFormattedMessages(systemPrompt ?? _options.SystemPrompt);

        var payload = new XFChatCompletionRequest
        {
            Model = model ?? _options.Model,
            Messages = messages,
            Temperature = temperature ?? _options.Temperature,
            MaxTokens = maxTokens ?? _options.MaxTokens,
            Stream = true
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json")
        };

        if (!string.IsNullOrEmpty(effectiveApiKey))
        {
            if (string.Equals(effectiveApiKeyHeader, "Authorization", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", effectiveApiKey);
            }
            else
            {
                request.Headers.Add(effectiveApiKeyHeader, effectiveApiKey);
            }
        }

        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Chat stream failed ({response.StatusCode}): {errorBody}");
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        var assistantResponse = new StringBuilder();

        string? line;
        while ((line = await reader.ReadLineAsync()) is not null && !cancellationToken.IsCancellationRequested)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            var json = line["data:".Length..].Trim();

            if (json == "[DONE]")
            {
                break;
            }

            var content = ParseStreamingResponse(json);
            if (!string.IsNullOrEmpty(content))
            {
                assistantResponse.Append(content);
                yield return content;
            }
        }

        if (assistantResponse.Length > 0)
        {
            session.AddMessage("assistant", assistantResponse.ToString());
        }
    }

    /// <inheritdoc />
    public XFConversationSession GetOrCreateSession(string? sessionId = null)
    {
        lock (_sessionsLock)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                session = new XFConversationSession
                {
                    Id = sessionId,
                    MaxMessages = _options.MaxMessages
                };
                _sessions[sessionId] = session;
            }

            return session;
        }
    }

    /// <inheritdoc />
    public void ClearSession(string sessionId)
    {
        lock (_sessionsLock)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.Clear();
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<XFConversationSession> GetActiveSessions()
    {
        lock (_sessionsLock)
        {
            return _sessions.Values.ToList();
        }
    }

    /// <inheritdoc />
    public void CleanupOldSessions(int maxAgeHours = 24)
    {
        lock (_sessionsLock)
        {
            var cutoff = DateTime.Now.AddHours(-maxAgeHours);
            var toRemove = _sessions.Values
                .Where(s => s.LastUpdated < cutoff)
                .Select(s => s.Id)
                .ToList();

            foreach (var id in toRemove)
            {
                _sessions.Remove(id);
            }
        }
    }

    private static string ParseStreamingResponse(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                return string.Empty;

            var firstChoice = choices[0];

            if (!firstChoice.TryGetProperty("delta", out var delta))
                return string.Empty;

            if (delta.TryGetProperty("content", out var contentElement))
                return contentElement.GetString() ?? string.Empty;

            return string.Empty;
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }
}
