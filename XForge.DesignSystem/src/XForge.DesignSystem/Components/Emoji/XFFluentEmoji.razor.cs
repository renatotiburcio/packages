using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Emoji;

/// <summary>
/// Code-behind for <see cref="XFFluentEmoji{TEmoji}"/>. Resolves emoji from
/// <see cref="XFEmojiCatalog"/> and decompresses GZip payload on-demand.
/// </summary>
public partial class XFFluentEmoji<TEmoji> : ComponentBase where TEmoji : class
{
}