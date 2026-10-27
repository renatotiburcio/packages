using System;

namespace XForge.DesignSystem.Components.Emoji;

/// <summary>
/// Renders an emoji as base64-encoded data URI for badges, tooltips, status indicators.
/// Reuses <see cref="XFEmojiCompress"/> to keep the payload small.
/// </summary>
public static class XFEmojiDataUri
{
    public static string ToDataUri(XFEmojiInfo emoji, int size = 32)
    {
        var svg = XFEmojiCompress.Unzip(emoji.Data);
        if (string.IsNullOrEmpty(svg)) return string.Empty;
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
        return $"data:image/svg+xml;base64,{base64}";
    }
}