using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
namespace XForge.DesignSystem.Services.AI;
public interface IAIChatService
{
    IAsyncEnumerable<string> GetCompletionsAsync(string sessionId, string userMessage, CancellationToken cancellationToken = default);
    ConversationSession? GetSession(string sessionId);
    ConversationSession CreateSession(string? sessionId = null);
    void ClearSession(string sessionId);
    void RemoveSession(string sessionId);
    int SessionCount { get; }
}