using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Chat
{
    public sealed class XFChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public AiRole Role { get; set; } = AiRole.Other;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsStreaming { get; set; }
        public bool IsLoading { get; set; }
    }
}
