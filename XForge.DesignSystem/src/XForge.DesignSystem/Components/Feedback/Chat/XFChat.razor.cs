using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;
namespace XForge.DesignSystem.Components.Feedback.Chat
{
    public partial class XFChat : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Subtitle { get; set; }
        [Parameter] public IReadOnlyList<XFChatMessage> Messages { get; set; } = Array.Empty<XFChatMessage>();
        [Parameter] public EventCallback<IReadOnlyList<XFChatMessage>> MessagesChanged { get; set; }
        [Parameter] public IReadOnlyList<XFChatUser> Users { get; set; } = Array.Empty<XFChatUser>();
        [Parameter] public string CurrentUserId { get; set; } = string.Empty;
        [Parameter] public string Placeholder { get; set; } = "Digite sua mensagem...";
        [Parameter] public string SendLabel { get; set; } = "Enviar mensagem";
        [Parameter] public string EmptyText { get; set; } = "Nenhuma mensagem ainda.";
        [Parameter] public string ComposerLabel { get; set; } = "Digite sua mensagem";
        [Parameter] public string TypingText { get; set; } = "Digitando...";
        [Parameter] public string MessageLoadingLabel { get; set; } = "Carregando";
        [Parameter] public bool Loading { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool ShowTypingIndicator { get; set; }
        [Parameter] public bool SendOnEnter { get; set; } = true;
        [Parameter] public int Rows { get; set; } = 2;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public RenderFragment? HeaderContent { get; set; }
        [Parameter] public RenderFragment? ComposerStart { get; set; }
        [Parameter] public RenderFragment? ComposerEnd { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public RenderFragment<XFChatMessage>? MessageTemplate { get; set; }
        [Parameter] public EventCallback<XFChatSendEventArgs> MessageSent { get; set; }
        [Parameter] public EventCallback<string?> DraftChanged { get; set; }
        [Parameter] public bool ShowDateSeparators { get; set; } = true;
        [Parameter] public string DateFormat { get; set; } = "dd/MM/yyyy";
        [Parameter] public string TimestampFormat { get; set; } = "HH:mm";
        [Parameter] public bool ShowTimestamps { get; set; } = true;
        [Parameter] public bool ShowUserNames { get; set; } = true;
        [Parameter] public int? MaxMessages { get; set; }
        [Parameter] public string MaxMessagesText { get; set; } = "Mostrando as ultimas {0} mensagens";

        protected override void OnInitialized()
        {
            Placeholder = L("chat.placeholder", Placeholder);
            SendLabel = L("chat.send", SendLabel);
            EmptyText = L("chat.empty", EmptyText);
            ComposerLabel = L("chat.composer", ComposerLabel);
            TypingText = L("chat.typing", TypingText);
            MessageLoadingLabel = L("chat.loading", MessageLoadingLabel);
            MaxMessagesText = L("chat.max_messages", MaxMessagesText);
            base.OnInitialized();
        }

        private string? Draft { get; set; }
        private bool PreventEnterDefault { get; set; }
        private bool CanSend => !Disabled && !ReadOnly && !Loading && !string.IsNullOrWhiteSpace(Draft);
        private string AriaBusy => Loading ? "true" : "false";

        private string RootClass =>
            Join("xf-chat flex min-h-[32rem] flex-col overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]", Class);

        private string HeaderClass =>
            "flex items-center justify-between gap-3 border-b border-border-light px-4 py-3";

        private string TitleClass => "text-sm font-semibold text-text-primary";
        private string SubtitleClass => "text-xs text-text-secondary";

        private string ComposerClass =>
            "flex items-end gap-2 border-t border-border-light bg-surface-50 p-3";

        private string TextareaClass =>
            Join(
                "min-h-10 flex-1 resize-none rounded-lg border border-surface-200 bg-bg-surface px-3 py-2 text-sm leading-6 text-text-primary outline-none transition-all duration-150 placeholder:text-text-muted focus:border-[var(--color-primary)] focus:ring-2 focus:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)] disabled:cursor-not-allowed disabled:opacity-60",
                Size == XFSize.Lg ? "text-base" : "");

        private string SendButtonClass =>
            "inline-flex h-10 min-w-10 items-center justify-center rounded-lg bg-primary px-3 text-sm font-semibold text-primary-contrast outline-none transition-all duration-150 hover:bg-primary-hover focus-visible:ring-2 focus-visible:ring-border-focus focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";

        private async Task HandleInputAsync(ChangeEventArgs args)
        {
            Draft = args.Value?.ToString();

            if (DraftChanged.HasDelegate)
            {
                await DraftChanged.InvokeAsync(Draft);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            PreventEnterDefault = SendOnEnter && args.Key == "Enter" && !args.ShiftKey;

            if (PreventEnterDefault)
            {
                await SendAsync();
            }
        }

        private async Task SendAsync()
        {
            if (!CanSend)
            {
                return;
            }

            var content = Draft!.Trim();
            var sentAt = DateTime.Now;

            if (MessageSent.HasDelegate)
            {
                await MessageSent.InvokeAsync(new XFChatSendEventArgs(content, CurrentUserId, sentAt));
            }
            else if (MessagesChanged.HasDelegate)
            {
                var next = Messages.Concat(new[]
                {
                    new XFChatMessage
                    {
                        Content = content,
                        UserId = CurrentUserId,
                        Role = AiRole.User,
                        Timestamp = sentAt
                    }
                }).ToArray();

                await MessagesChanged.InvokeAsync(next);
            }

            Draft = string.Empty;

            if (DraftChanged.HasDelegate)
            {
                await DraftChanged.InvokeAsync(Draft);
            }
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
