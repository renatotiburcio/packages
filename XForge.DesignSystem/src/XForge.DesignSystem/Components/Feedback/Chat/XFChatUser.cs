namespace XForge.DesignSystem.Components.Feedback.Chat
{
    public sealed class XFChatUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsOnline { get; set; } = true;

        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return "?";
                }

                var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1
                    ? parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant()
                    : $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();
            }
        }
    }
}
