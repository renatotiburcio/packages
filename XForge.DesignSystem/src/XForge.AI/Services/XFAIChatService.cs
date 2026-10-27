using Microsoft.Extensions.AI;

namespace XForge.AI.Services;

/// <summary>T129: XFAIChatService -- Microsoft.Extensions.AI wrapper. Syncfusion. P1. 16h. Dep T060.</summary>
public interface IXFAIChatService
{
    Task<string> SendAsync(string prompt, CancellationToken ct = default);
    IAsyncEnumerable<string> StreamAsync(string prompt, CancellationToken ct = default);
}
public class XFAIChatService : IXFAIChatService
{
    private readonly IChatClient _client;
    public XFAIChatService(IChatClient client) { _client = client; }
    public async Task<string> SendAsync(string prompt, CancellationToken ct = default)
    {
        var r = await _client.GetResponseAsync(new[] { new ChatMessage(ChatRole.User, prompt) }, cancellationToken: ct);
        return r.Text ?? "";
    }
    public async IAsyncEnumerable<string> StreamAsync(string prompt, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var u in _client.GetStreamingResponseAsync(new[] { new ChatMessage(ChatRole.User, prompt) }, cancellationToken: ct))
        {
            yield return u.Text;
        }
    }
}