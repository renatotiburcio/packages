using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.AI;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Feedback.Chat;

/// <summary>
/// AI-powered chat component with SSE streaming, conversation memory, and markdown rendering.
/// </summary>
public partial class XFAIChat : XForgeComponentBase, IAsyncDisposable
{
    [Inject] private IXFChatCompletionService AIChatService { get; set; } = default!;

    private readonly List<XFAIChatMessage> InternalMessages = new();
    private CancellationTokenSource? _cts;
    private bool IsStreaming;
    private string? Draft;
    private string? currentSessionId;
    private string? SelectedModel;

    #region Parameters

    [Parameter] public string? Title { get; set; } = "AI Assistant";
    [Parameter] public string? Placeholder { get; set; } = "Type your message...";
    [Parameter] public string EmptyText { get; set; } = "No messages yet. Start a conversation!";
    [Parameter] public string StreamingText { get; set; } = "Thinking...";
    [Parameter] public string AssistantName { get; set; } = "Assistant";
    [Parameter] public string AssistantAvatarText { get; set; } = "AI";
    [Parameter] public bool ShowAvatar { get; set; } = true;
    [Parameter] public bool ShowTimestamps { get; set; } = true;
    [Parameter] public string TimestampFormat { get; set; } = "HH:mm";
    [Parameter] public bool ShowClearButton { get; set; } = true;
    [Parameter] public bool ShowModelSelector { get; set; }
    [Parameter] public string[]? AvailableModels { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>ARIA label for the chat region.</summary>
    [Parameter] public string AriaLabel { get; set; } = "AI Chat";

    [Parameter] public string? SessionId { get; set; }
    [Parameter] public EventCallback<string> SessionIdChanged { get; set; }
    [Parameter] public string? Model { get; set; }
    [Parameter] public string? SystemPrompt { get; set; }
    [Parameter] public double? Temperature { get; set; }
    [Parameter] public int? MaxTokens { get; set; }
    [Parameter] public string? Endpoint { get; set; }
    [Parameter] public string? Proxy { get; set; }
    [Parameter] public string? ApiKey { get; set; }
    [Parameter] public string? ApiKeyHeader { get; set; }

    [Parameter] public EventCallback<XFAIChatMessage> MessageAdded { get; set; }
    [Parameter] public EventCallback<string> MessageSent { get; set; }
    [Parameter] public EventCallback<string> ResponseReceived { get; set; }
    [Parameter] public EventCallback ChatCleared { get; set; }
    [Parameter] public EventCallback<string> StreamingToken { get; set; }

    #endregion

    #region Public Methods

    public async Task SendMessageAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content) || IsStreaming || Disabled) return;

        var userMessage = AddMessage(content, isUser: true);
        await MessageAdded.InvokeAsync(userMessage);
        await MessageSent.InvokeAsync(content);

        Draft = string.Empty;
        await InvokeAsync(StateHasChanged);
        await GetAIResponseAsync(content);
    }

    public async Task ClearChatAsync()
    {
        InternalMessages.Clear();
        if (!string.IsNullOrEmpty(currentSessionId))
            AIChatService.ClearSession(currentSessionId);

        await ChatCleared.InvokeAsync();
        await InvokeAsync(StateHasChanged);
    }

    public IReadOnlyList<XFAIChatMessage> GetMessages() => InternalMessages.AsReadOnly();
    public string? GetSessionId() => currentSessionId;

    #endregion

    #region Lifecycle

    protected override void OnInitialized()
    {
        currentSessionId = SessionId ?? Guid.NewGuid().ToString();
        SelectedModel = Model;
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrEmpty(SessionId) && SessionId != currentSessionId)
        {
            currentSessionId = SessionId;
            await SessionIdChanged.InvokeAsync(currentSessionId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            _cts.Dispose();
        }
    }

    #endregion

    #region Private Methods

    private XFAIChatMessage AddMessage(string content, bool isUser)
    {
        var message = new XFAIChatMessage
        {
            Content = content,
            Role = isUser ? "user" : "assistant",
            Timestamp = DateTime.Now
        };
        InternalMessages.Add(message);
        return message;
    }

    private async Task GetAIResponseAsync(string userInput)
    {
        IsStreaming = true;
        var previousCts = _cts;
        if (previousCts != null) await previousCts.CancelAsync();
        _cts = new CancellationTokenSource();

        var assistantMessage = AddMessage("", isUser: false);
        assistantMessage.IsStreaming = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            var response = "";
            await foreach (var token in AIChatService.GetCompletionsAsync(
                userInput, currentSessionId, _cts.Token,
                SelectedModel, SystemPrompt, Temperature, MaxTokens,
                Endpoint, Proxy, ApiKey, ApiKeyHeader))
            {
                response += token;
                assistantMessage.Content = response;
                await StreamingToken.InvokeAsync(token);
                await InvokeAsync(StateHasChanged);
            }

            assistantMessage.IsStreaming = false;
            await ResponseReceived.InvokeAsync(response);
            await MessageAdded.InvokeAsync(assistantMessage);
        }
        catch (OperationCanceledException)
        {
            assistantMessage.IsStreaming = false;
        }
        catch (Exception ex)
        {
            assistantMessage.Content = $"Error: {ex.Message}";
            assistantMessage.IsStreaming = false;
        }
        finally
        {
            IsStreaming = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnSendMessageAsync()
    {
        if (!string.IsNullOrWhiteSpace(Draft))
            await SendMessageAsync(Draft.Trim());
    }

    private async Task OnClearChatAsync()
    {
        await ClearChatAsync();
    }

    private async Task OnInputAsync(ChangeEventArgs args)
    {
        Draft = args.Value?.ToString();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Enter" && !args.ShiftKey)
            await OnSendMessageAsync();
    }

    #endregion
}
