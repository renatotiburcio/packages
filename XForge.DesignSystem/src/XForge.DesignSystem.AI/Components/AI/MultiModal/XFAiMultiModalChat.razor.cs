using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.MultiModal;

/// <summary>
/// Enterprise multi-modal AI chat supporting text, images, audio, and file attachments.
/// Integrates with IAiProvider and IAiVisionProvider for comprehensive AI interactions.
/// </summary>
public partial class XFAiMultiModalChat : XForgeComponentBase, IDisposable
{
    private readonly List<XFAiMultiModalMessage> _messages = new();
    private readonly List<XFAiAttachment> _pendingAttachments = new();
    private string _inputText = "";
    private bool _isLoading;
    private ElementReference _messagesContainer;

    [Inject] public IAiProvider? AiProvider { get; set; }
    [Inject] public IAiVisionProvider? VisionProvider { get; set; }

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Placeholder text for the input.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>ARIA label for the chat.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Chat multi-modal AI";

    /// <summary>Maximum number of messages to display.</summary>
    [Parameter] public int MaxMessages { get; set; } = 100;

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Callback when a message is sent.</summary>
    [Parameter] public EventCallback<XFAiMultiModalMessage> OnMessageSent { get; set; }

    /// <summary>Callback when a response is received.</summary>
    [Parameter] public EventCallback<XFAiMultiModalMessage> OnResponseReceived { get; set; }

    /// <summary>Callback when an error occurs.</summary>
    [Parameter] public EventCallback<string> OnError { get; set; }

    /// <summary>All messages in the chat.</summary>
    public IReadOnlyList<XFAiMultiModalMessage> Messages => _messages;

    /// <summary>
    /// Adds an attachment to the pending list.
    /// </summary>
    public void AddAttachment(XFAiAttachment attachment)
    {
        _pendingAttachments.Add(attachment);
        StateHasChanged();
    }

    /// <summary>
    /// Clears all messages.
    /// </summary>
    public void ClearMessages()
    {
        _messages.Clear();
        StateHasChanged();
    }

    private void RemovePendingAttachment(int index)
    {
        if (index >= 0 && index < _pendingAttachments.Count)
        {
            _pendingAttachments.RemoveAt(index);
        }
    }

    private void TriggerAttachment()
    {
        // In production, this would trigger file input via JS interop
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await SendMessageAsync();
        }
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(_inputText) && _pendingAttachments.Count == 0)
            return;

        var userMessage = new XFAiMultiModalMessage
        {
            Role = AiRole.User,
            Content = _inputText,
            Attachments = _pendingAttachments.Count > 0 ? _pendingAttachments.ToList() : null
        };

        _messages.Add(userMessage);
        _inputText = "";
        var attachmentsToSend = _pendingAttachments.ToList();
        _pendingAttachments.Clear();

        await OnMessageSent.InvokeAsync(userMessage);
        StateHasChanged();

        // Get AI response
        _isLoading = true;
        StateHasChanged();

        try
        {
            string responseText;

            // Check if there are image attachments — use vision provider
            var imageAttachments = attachmentsToSend.Where(a => a.Type == XFAiAttachmentType.Image).ToList();
            if (imageAttachments.Count > 0 && VisionProvider != null)
            {
                var visionRequest = new XForge.DesignSystem.Services.AI.AiVisionRequest
                {
                    Prompt = userMessage.Content,
                    Images = imageAttachments.Select(a => a.Data).ToList(),
                    MimeTypes = imageAttachments.Select(a => a.MimeType ?? "image/png").ToList()
                };
                var visionResponse = await VisionProvider.AnalyzeAsync(visionRequest);
                responseText = visionResponse.Text;
            }
            else if (AiProvider != null)
            {
                // Use text provider
                var chatRequest = new AiChatRequest
                {
                    Messages = _messages.Select(m => new AiMessage
                    {
                        Role = m.Role,
                        Content = m.Content
                    }).ToList(),
                    MaxTokens = 1024
                };
                var chatResponse = await AiProvider.CompleteAsync(chatRequest);
                responseText = chatResponse.Message.Content;
            }
            else
            {
                responseText = "Provedor AI não configurado. Configure IAiProvider ou IAiVisionProvider.";
            }

            var aiMessage = new XFAiMultiModalMessage
            {
                Role = AiRole.Assistant,
                Content = responseText
            };

            _messages.Add(aiMessage);
            await OnResponseReceived.InvokeAsync(aiMessage);

            // Trim messages if exceeding max
            while (_messages.Count > MaxMessages)
            {
                _messages.RemoveAt(0);
            }
        }
        catch (Exception ex)
        {
            var errorMsg = $"Erro ao obter resposta: {ex.Message}";
            _messages.Add(new XFAiMultiModalMessage
            {
                Role = AiRole.Assistant,
                Content = $"Desculpe, ocorreu um erro. {errorMsg}"
            });
            await OnError.InvokeAsync(errorMsg);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        // No unmanaged resources to dispose
    }
}
