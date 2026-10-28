using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Feedback.Chat
{
    public partial class XFMessageList : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public IReadOnlyList<XFChatMessage> Messages { get; set; } = Array.Empty<XFChatMessage>();
        [Parameter] public IReadOnlyList<XFChatUser> Users { get; set; } = Array.Empty<XFChatUser>();
        [Parameter] public string CurrentUserId { get; set; } = string.Empty;
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public string TypingText { get; set; } = default!;
        [Parameter] public string MessageLoadingLabel { get; set; } = default!;
        [Parameter] public string TimestampFormat { get; set; } = default!;
        [Parameter] public string DateSeparatorFormat { get; set; } = default!;
        [Parameter] public bool ShowDateSeparators { get; set; } = true;
        [Parameter] public bool ShowUserNames { get; set; } = true;
        [Parameter] public bool ShowTypingIndicator { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public RenderFragment<XFChatMessage>? MessageTemplate { get; set; }
        [Parameter] public RenderFragment<DateTime>? DateSeparatorTemplate { get; set; }
        [Parameter] public RenderFragment? TypingTemplate { get; set; }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("messagelist.empty", EmptyText);
            if (string.IsNullOrWhiteSpace(TypingText)) TypingText = L("messagelist.typing", TypingText);
            if (string.IsNullOrWhiteSpace(MessageLoadingLabel)) MessageLoadingLabel = L("messagelist.loading", MessageLoadingLabel);
            base.OnInitialized();
        }

        private string RootClass =>
            Join("xf-message-list flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto p-4", Class);

        private string EmptyClass =>
            "grid min-h-40 place-items-center rounded-lg border border-dashed border-border-light bg-surface-50 px-6 py-8 text-center text-sm text-text-secondary";

        private string DateSeparatorClass =>
            "flex items-center gap-3 text-xs font-medium text-text-muted before:h-px before:flex-1 before:bg-border-light after:h-px after:flex-1 after:bg-border-light";

        private string TypingClass =>
            "w-fit rounded-full bg-surface-100 px-3 py-1.5 text-xs font-medium text-text-secondary ring-1 ring-border-light";

        private XFChatUser? GetUser(string userId)
        {
            return Users.FirstOrDefault(user => string.Equals(user.Id, userId, StringComparison.Ordinal));
        }

        private bool IsCurrentUser(XFChatMessage message)
        {
            return string.Equals(message.UserId, CurrentUserId, StringComparison.Ordinal)
                || message.Role == AiRole.User;
        }

        private string MessageRowClass(XFChatMessage message)
        {
            return Join("flex gap-3", IsCurrentUser(message) ? "justify-end" : "justify-start");
        }

        private string BubbleClass(XFChatMessage message)
        {
            if (IsCurrentUser(message))
            {
                return "max-w-[min(34rem,80%)] rounded-2xl rounded-br-md bg-primary px-4 py-3 text-sm leading-6 text-primary-contrast shadow-[0_1px_2px_rgb(15_23_42/.12)]";
            }

            return message.Role switch
            {
                AiRole.System => "max-w-[min(34rem,80%)] rounded-2xl bg-surface-100 px-4 py-3 text-sm leading-6 text-text-secondary ring-1 ring-border-light",
                _ => "max-w-[min(34rem,80%)] rounded-2xl rounded-bl-md bg-bg-surface px-4 py-3 text-sm leading-6 text-text-primary ring-1 ring-border-light shadow-[0_1px_2px_rgb(15_23_42/.04)]"
            };
        }

        private string AvatarClass =>
            "mt-5 inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-surface-100 text-xs font-semibold text-text-secondary ring-1 ring-border-light";

        private string MetaClass(XFChatMessage message) =>
            Join("mb-1 flex items-center gap-2 text-xs text-text-muted", IsCurrentUser(message) ? "justify-end" : "justify-start");

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }
    }
}
