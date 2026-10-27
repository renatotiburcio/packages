using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Emoji;

public sealed record XFEmojiInfo(
    string Name,
    XFEmojiGroup Group,
    XFEmojiStyle Style,
    XFEmojiSkintone Skintone,
    XFEmojiSize Size,
    IReadOnlyList<string> Keywords,
    byte[] Data);