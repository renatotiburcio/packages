using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Chat;

/// <summary>
/// Enterprise AI chat copilot component with streaming, multi-model support,
/// tool calling, citations, and accessibility.
/// </summary>
public partial class XFAiChat : XForgeComponentBase, IDisposable
{
    // -- Parameters ------------------------------------------------------

    /// <summary>Conversation history (two-way bindable via MessagesChanged).</summary>
    [Parameter]
    public IReadOnlyList<AiMessage> Messages { get; set; } = Array.Empty<AiMessage>();

    /// <summary>Two-way binding callback for Messages.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<AiMessage>> MessagesChanged { get; set; }

    /// <summary>Input placeholder text.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Digite sua mensagem para a IA...";

    /// <summary>AI provider for completions. When null, only OnSend is invoked.</summary>
    [Parameter]
    public IAiProvider? Provider { get; set; }

    /// <summary>Whether streaming is enabled (default true).</summary>
    [Parameter]
    public bool Streaming { get; set; } = true;

    /// <summary>Whether file attachments are allowed.</summary>
    [Parameter]
    public bool AllowAttachments { get; set; }

    /// <summary>Whether voice input is allowed.</summary>
    [Parameter]
    public bool AllowVoice { get; set; }

    /// <summary>Temperature (creativity) parameter for the AI model.</summary>
    [Parameter]
    public double? Temperature { get; set; }

    /// <summary>Maximum completion tokens.</summary>
    [Parameter]
    public int? MaxTokens { get; set; }

    /// <summary>System prompt prepended to conversations.</summary>
    [Parameter]
    public string? SystemPrompt { get; set; }

    /// <summary>Model identifier (e.g., "gpt-4o", "llama3").</summary>
    [Parameter]
    public string? Model { get; set; }

    /// <summary>Whether to show the header bar.</summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>Header title.</summary>
    [Parameter]
    public string Title { get; set; } = "Assistente IA";

    /// <summary>Header subtitle.</summary>
    [Parameter]
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>Whether to show message timestamps.</summary>
    [Parameter]
    public bool ShowTimestamps { get; set; } = true;

    /// <summary>Whether to show copy button on assistant messages.</summary>
    [Parameter]
    public bool ShowCopyButton { get; set; } = true;

    /// <summary>Whether to show cumulative token usage in footer.</summary>
    [Parameter]
    public bool ShowTokenUsage { get; set; }

    /// <summary>Content rendered when there are no messages.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>Custom header content (replaces default header).</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Custom message template.</summary>
    [Parameter]
    public RenderFragment<AiMessage>? MessageTemplate { get; set; }

    /// <summary>Custom footer content.</summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    /// <summary>Send button aria-label.</summary>
    [Parameter]
    public string SendLabel { get; set; } = "Enviar mensagem";

    /// <summary>Cancel button aria-label.</summary>
    [Parameter]
    public string CancelLabel { get; set; } = "Cancelar";

    /// <summary>Copy button aria-label.</summary>
    [Parameter]
    public string CopyLabel { get; set; } = "Copiar mensagem";

    // -- Events ----------------------------------------------------------

    /// <summary>Fired when user sends a message. If Provider is set, the component
    /// handles the request automatically; otherwise the consumer must handle it.</summary>
    [Parameter]
    public EventCallback<AiChatRequest> OnSend { get; set; }

    /// <summary>Fired when AI responds (complete response).</summary>
    [Parameter]
    public EventCallback<AiChatResponse> OnReceive { get; set; }

    /// <summary>Fired on error during AI request.</summary>
    [Parameter]
    public EventCallback<string> OnError { get; set; }

    /// <summary>Fired when user cancels streaming.</summary>
    [Parameter]
    public EventCallback OnCancel { get; set; }

    // -- Computed State --------------------------------------------------

    /// <summary>Current operational state of the component.</summary>
    public XFAiChatState State { get; private set; } = XFAiChatState.Idle;

    /// <summary>True when Loading or Streaming.</summary>
    public bool IsBusy => State is XFAiChatState.Loading or XFAiChatState.Streaming;

    /// <summary>Cumulative token usage across all interactions.</summary>
    public int TotalTokens { get; private set; }

    /// <summary>Last error message, if any.</summary>
    public string? ErrorText { get; private set; }

    // -- Internal State --------------------------------------------------

    private string? _draft;
    private bool _preventEnterDefault;
    private CancellationTokenSource? _cts;
    private readonly List<AiMessage> _messages = new();
    private string? _copiedMessageId;

    private bool CanSend => !Disabled && !IsBusy && !string.IsNullOrWhiteSpace(_draft);
    private bool CanCancel => IsBusy;
    private string AriaBusyValue => IsBusy ? "true" : "false";

    // -- Lifecycle -------------------------------------------------------

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Sync external Messages into internal list
        if (!ReferenceEquals(_messages, Messages) && !_messages.SequenceEqual(Messages))
        {
            _messages.Clear();
            _messages.AddRange(Messages);
        }
    }

    // -- CSS Classes -----------------------------------------------------

    private string RootClass => Join(
        "xf-ai-chat flex min-h-[32rem] flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]",
        Class);

    private string HeaderClass =>
        "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

    private string HeaderTitleClass =>
        "inline-flex items-center gap-2 text-sm font-semibold text-text-primary";

    private string HeaderSubtitleClass =>
        "text-xs text-text-secondary";

    private string MessagesContainerClass =>
        "xf-ai-chat-messages flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto p-4 scroll-smooth";

    private string EmptyContainerClass =>
        "grid min-h-40 flex-1 place-items-center px-6 py-8";

    private string EmptyInnerClass =>
        "flex flex-col items-center gap-3 text-center";

    private string EmptyIconClass =>
        "inline-flex h-12 w-12 items-center justify-center rounded-full bg-surface-100 text-text-muted";

    private string EmptyTextClass =>
        "text-sm text-text-secondary max-w-xs";

    private string MessageRowClass(AiMessage message) => message.Role switch
    {
        AiRole.User => "flex gap-3 justify-end",
        AiRole.System => "flex justify-center",
        _ => "flex gap-3 justify-start"
    };

    private string BubbleClass(AiMessage message) => message.Role switch
    {
        AiRole.User =>
            "max-w-[min(34rem,80%)] rounded-2xl rounded-br-md bg-primary px-4 py-3 text-sm leading-6 text-primary-contrast shadow-[0_1px_2px_rgb(15_23_42/.12)]",
        AiRole.System =>
            "max-w-[min(34rem,80%)] rounded-2xl bg-surface-100 px-4 py-2 text-xs leading-5 text-text-muted ring-1 ring-border-light",
        _ =>
            "max-w-[min(34rem,80%)] rounded-2xl rounded-bl-md bg-bg-surface px-4 py-3 text-sm leading-6 text-text-primary ring-1 ring-border-light shadow-[0_1px_2px_rgb(15_23_42/.04)]"
    };

    private string AvatarClass =>
        "mt-1 inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-surface-100 text-xs font-semibold text-text-secondary ring-1 ring-border-light";

    private string MetaClass(AiMessage message) => message.Role switch
    {
        AiRole.User => "mb-1 flex items-center gap-2 text-xs text-text-muted justify-end",
        AiRole.System => "mb-1 flex items-center justify-center gap-2 text-xs text-text-muted",
        _ => "mb-1 flex items-center gap-2 text-xs text-text-muted justify-start"
    };

    private string ComposerClass =>
        "flex items-end gap-2 border-t border-border-light bg-surface-50 p-3";

    private string TextareaClass =>
        "min-h-10 flex-1 resize-none rounded-lg border border-surface-200 bg-bg-surface px-3 py-2 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60";

    private string SendButtonClass =>
        "inline-flex h-10 min-w-10 items-center justify-center rounded-lg bg-primary px-3 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";

    private string CancelButtonClass =>
        "inline-flex h-10 min-w-10 items-center justify-center rounded-lg border border-border-light bg-bg-surface px-3 text-sm font-semibold text-text-primary outline-none transition-all duration-150 hover:bg-surface-100 focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2";

    private string CopyButtonClass =>
        "inline-flex h-7 w-7 items-center justify-center rounded-md text-text-muted outline-none transition-all duration-150 hover:bg-surface-100 hover:text-text-primary focus-visible:ring-2 focus-visible:ring-border-focus";

    private string FooterClass =>
        "flex items-center justify-between gap-3 border-t border-border-light px-4 py-2 text-xs text-text-muted";

    private string ErrorBannerClass =>
        "flex items-center gap-2 border-t border-danger-200 bg-danger-50 px-4 py-2 text-xs text-danger";

    // -- User Interactions -----------------------------------------------

    private void HandleInput(ChangeEventArgs args)
    {
        _draft = args.Value?.ToString();
        StateHasChanged();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        // Escape cancels streaming
        if (args.Key == "Escape" && CanCancel)
        {
            await CancelAsync();
            return;
        }

        // Enter sends, Shift+Enter inserts newline
        _preventEnterDefault = args.Key == "Enter" && !args.ShiftKey;

        if (_preventEnterDefault && CanSend)
        {
            await SendMessageAsync();
        }
    }

    // -- Public Methods --------------------------------------------------

    /// <summary>
    /// Programmatically sends a message. Creates an AiMessage with Role=User,
    /// appends it, and invokes the AI provider if configured.
    /// </summary>
    public async Task SendMessageAsync()
    {
        if (!CanSend)
            return;

        var content = _draft!.Trim();
        _draft = string.Empty;
        ErrorText = null;

        // 1. Create user message
        var userMessage = new AiMessage
        {
            Role = AiRole.User,
            Content = content,
            Timestamp = DateTimeOffset.UtcNow
        };

        _messages.Add(userMessage);
        await NotifyMessagesChangedAsync();

        // 2. Build request
        var request = new AiChatRequest
        {
            Messages = _messages.ToList(),
            SystemPrompt = SystemPrompt,
            Temperature = Temperature,
            MaxTokens = MaxTokens,
            Model = Model
        };

        // 3. Fire OnSend
        if (OnSend.HasDelegate)
        {
            await OnSend.InvokeAsync(request);
        }

        // 4. If Provider is set, handle the request
        if (Provider is not null)
        {
            await ProcessRequestAsync(request);
        }

        StateHasChanged();
    }

    /// <summary>
    /// Clears the conversation history.
    /// </summary>
    public async Task ClearAsync()
    {
        _messages.Clear();
        ErrorText = null;
        State = XFAiChatState.Idle;
        TotalTokens = 0;
        await NotifyMessagesChangedAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Cancels the current streaming operation.
    /// </summary>
    public async Task CancelAsync()
    {
        if (_cts is not null && !_cts.IsCancellationRequested)
        {
            await _cts.CancelAsync();
        }

        State = XFAiChatState.Cancelled;

        if (OnCancel.HasDelegate)
        {
            await OnCancel.InvokeAsync();
        }

        StateHasChanged();
    }

    // -- Internal Processing ---------------------------------------------

    private async Task ProcessRequestAsync(AiChatRequest request)
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        if (Streaming && Provider!.SupportsStreaming)
        {
            await ProcessStreamingAsync(request, ct);
        }
        else
        {
            await ProcessCompleteAsync(request, ct);
        }
    }

    private async Task ProcessStreamingAsync(AiChatRequest request, CancellationToken ct)
    {
        State = XFAiChatState.Streaming;
        var accumulated = new System.Text.StringBuilder();

        try
        {
            await foreach (var chunk in Provider!.StreamAsync(request, ct))
            {
                if (chunk.Delta is not null)
                {
                    accumulated.Append(chunk.Delta);
                }

                if (chunk.FinishReason is not null)
                {
                    break;
                }

                StateHasChanged();
            }

            // Create assistant message from accumulated content
            if (accumulated.Length > 0)
            {
                var assistantMessage = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = accumulated.ToString(),
                    Timestamp = DateTimeOffset.UtcNow
                };

                _messages.Add(assistantMessage);
                await NotifyMessagesChangedAsync();
            }

            State = XFAiChatState.Completed;
        }
        catch (OperationCanceledException)
        {
            State = XFAiChatState.Cancelled;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }

        _cts?.Dispose();
        _cts = null;
        StateHasChanged();
    }

    private async Task ProcessCompleteAsync(AiChatRequest request, CancellationToken ct)
    {
        State = XFAiChatState.Loading;

        try
        {
            var response = await Provider!.CompleteAsync(request, ct);

            // Create assistant message
            var assistantMessage = new AiMessage
            {
                Role = AiRole.Assistant,
                Content = response.Message.Content,
                Citations = response.Message.Citations,
                ToolCalls = response.Message.ToolCalls,
                Reasoning = response.Message.Reasoning,
                Timestamp = DateTimeOffset.UtcNow
            };

            _messages.Add(assistantMessage);
            await NotifyMessagesChangedAsync();

            // Track token usage
            if (response.Usage is not null)
            {
                TotalTokens += response.Usage.TotalTokens;
            }

            State = XFAiChatState.Completed;

            if (OnReceive.HasDelegate)
            {
                await OnReceive.InvokeAsync(response);
            }
        }
        catch (OperationCanceledException)
        {
            State = XFAiChatState.Cancelled;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }

        _cts?.Dispose();
        _cts = null;
        StateHasChanged();
    }

    private async Task HandleErrorAsync(Exception ex)
    {
        State = XFAiChatState.Error;
        ErrorText = ex.Message;

        if (OnError.HasDelegate)
        {
            await OnError.InvokeAsync(ex.Message);
        }
    }

    private async Task NotifyMessagesChangedAsync()
    {
        if (MessagesChanged.HasDelegate)
        {
            await MessagesChanged.InvokeAsync(_messages.ToList());
        }
    }

    // -- Copy Support ----------------------------------------------------

    private void CopyMessage(AiMessage message)
    {
        _copiedMessageId = message.GetHashCode().ToString();
        // Actual clipboard copy requires JS interop — the button renders
        // a data-copy-content attribute for external JS to handle.
        // For now, we toggle the visual feedback state.
        StateHasChanged();

        // Reset after 2 seconds
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            _copiedMessageId = null;
            await InvokeAsync(StateHasChanged);
        });
    }

    private bool IsCopied(AiMessage message)
    {
        return _copiedMessageId == message.GetHashCode().ToString();
    }

    // -- Dispose ---------------------------------------------------------

    /// <inheritdoc />
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    // -- Helpers ---------------------------------------------------------

    private static string Join(params string?[] values)
    {
        return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    private static string GetRoleName(AiRole role) => role switch
    {
        AiRole.User => "Você",
        AiRole.Assistant => "IA",
        AiRole.System => "Sistema",
        AiRole.Tool => "Ferramenta",
        _ => "Desconhecido"
    };

    private static string GetRoleAvatar(AiRole role) => role switch
    {
        AiRole.Assistant => "IA",
        AiRole.System => "S",
        AiRole.Tool => "T",
        _ => "U"
    };
}
