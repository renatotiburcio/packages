namespace XForge.DesignSystem.Components.DataDisplay.Markdown
{
internal enum XFMarkdownBlockKind
    {
        Paragraph,
        Heading,
        UnorderedList,
        OrderedList,
        Blockquote,
        Code
    }

    internal sealed record XFMarkdownBlock(
        XFMarkdownBlockKind Kind,
        string Text,
        int Level = 0,
        IReadOnlyList<string>? Items = null,
        string? Language = null);
}
