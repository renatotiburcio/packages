using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace XForge.DesignSystem.Services.AI;
public class AIChatService : IAIChatService
{
    private readonly HttpClient _http;
    private readonly AIChatServiceOptions _opts;
    private readonly Dictionary<string, ConversationSession> _sessions = new();
    private readonly object _lock = new();
    public int SessionCount { get { lock (_lock) return _sessions.Count; } }
    public AIChatService(HttpClient http, IOptions<AIChatServiceOptions> opts) { _http = http; _opts = opts.Value; }
    public ConversationSession CreateSession(string? sessionId = null) { sessionId ??= Guid.NewGuid().ToString("N").Substring(0, 16); var s = new ConversationSession(sessionId); lock (_lock) _sessions[sessionId] = s; return s; }
    public ConversationSession? GetSession(string sessionId) { lock (_lock) return _sessions.TryGetValue(sessionId, out var s) ? s : null; }
    public void ClearSession(string sessionId) { GetSession(sessionId)?.Clear(); }
    public void RemoveSession(string sessionId) { lock (_lock) _sessions.Remove(sessionId); }
    public async IAsyncEnumerable<string> GetCompletionsAsync(string sessionId, string userMessage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var session = GetSession(sessionId) ?? CreateSession(sessionId);
        session.AddMessage(new AIChatMessage("user", userMessage, DateTime.UtcNow));
        await Task.Yield();
        var chunks = new[] { "Hello", " from", " AI", " stream" };
        foreach (var c in chunks) { await Task.Delay(10, cancellationToken); yield return c; }
        session.AddMessage(new AIChatMessage("assistant", string.Concat(chunks), DateTime.UtcNow));
    }
}