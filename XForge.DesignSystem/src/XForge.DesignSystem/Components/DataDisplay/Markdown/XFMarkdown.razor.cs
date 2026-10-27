using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Markdown
{
    public partial class XFMarkdown : XForgeComponentBase, IAsyncDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        private IJSObjectReference? clipboardModule;
        private string? copiedCodeKey;

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public string? Text { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public bool AllowLinks { get; set; } = true;
        [Parameter] public bool OpenLinksInNewTab { get; set; }
        [Parameter] public bool ShowCodeLanguage { get; set; } = true;
        [Parameter] public bool ShowCodeCopy { get; set; } = true;
        [Parameter] public string CopyCodeLabel { get; set; } = "Copiar";
        [Parameter] public string CopiedCodeLabel { get; set; } = "Copiado";
        [Parameter] public string EmptyText { get; set; } = "Nenhum conteudo.";

        protected override void OnInitialized()
        {
            CopyCodeLabel = L("markdown.copy", CopyCodeLabel);
            CopiedCodeLabel = L("markdown.copied", CopiedCodeLabel);
            EmptyText = L("markdown.empty", EmptyText);
            base.OnInitialized();
        }

        private IReadOnlyList<XFMarkdownBlock> Blocks => ParseBlocks(Text);

        private string RootClass =>
            Join("xf-markdown max-w-none text-text-primary", SizeClass, Class);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "text-sm leading-6",
            XFSize.Lg => "text-base leading-7",
            _ => "text-sm leading-6"
        };

        private string EmptyClass => "rounded-lg border border-dashed border-border-light bg-surface-50 px-4 py-6 text-center text-sm text-text-secondary";

        private async Task CopyCodeAsync(XFMarkdownBlock block)
        {
            clipboardModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/XForge.DesignSystem/js/xf-clipboard.js");
            await clipboardModule.InvokeAsync<bool>("copyText", block.Text ?? string.Empty);
            copiedCodeKey = GetCodeKey(block);
        }

        private string GetCodeButtonText(XFMarkdownBlock block)
        {
            return string.Equals(copiedCodeKey, GetCodeKey(block), StringComparison.Ordinal)
                ? CopiedCodeLabel
                : CopyCodeLabel;
        }

        private static string GetCodeKey(XFMarkdownBlock block)
        {
            return $"{block.Language}\n{block.Text}";
        }

        private string HeadingClass(int level)
        {
            return level switch
            {
                1 => "mt-0 mb-4 text-2xl font-semibold leading-tight text-text-primary",
                2 => "mt-6 mb-3 text-xl font-semibold leading-tight text-text-primary",
                3 => "mt-5 mb-2 text-lg font-semibold leading-tight text-text-primary",
                _ => "mt-4 mb-2 text-base font-semibold leading-tight text-text-primary"
            };
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        public async ValueTask DisposeAsync()
        {
            if (clipboardModule is not null)
            {
                await clipboardModule.DisposeAsync();
            }
        }

        private static IReadOnlyList<XFMarkdownBlock> ParseBlocks(string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return Array.Empty<XFMarkdownBlock>();
            }

            var lines = markdown.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
            var blocks = new List<XFMarkdownBlock>();
            var paragraph = new List<string>();
            var inCode = false;
            var codeLanguage = string.Empty;
            var code = new List<string>();

            void FlushParagraph()
            {
                if (paragraph.Count == 0)
                {
                    return;
                }

                blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.Paragraph, string.Join(" ", paragraph).Trim()));
                paragraph.Clear();
            }

            for (var i = 0; i < lines.Length; i++)
            {
                var raw = lines[i];
                var line = raw.TrimEnd();

                if (line.StartsWith("```", StringComparison.Ordinal))
                {
                    if (inCode)
                    {
                        blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.Code, string.Join(Environment.NewLine, code), Language: codeLanguage));
                        code.Clear();
                        codeLanguage = string.Empty;
                        inCode = false;
                    }
                    else
                    {
                        FlushParagraph();
                        codeLanguage = line[3..].Trim();
                        inCode = true;
                    }

                    continue;
                }

                if (inCode)
                {
                    code.Add(raw);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    FlushParagraph();
                    continue;
                }

                var trimmed = line.TrimStart();

                if (TryParseHeading(trimmed, out var level, out var heading))
                {
                    FlushParagraph();
                    blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.Heading, heading, level));
                    continue;
                }

                if (trimmed.StartsWith("> ", StringComparison.Ordinal))
                {
                    FlushParagraph();
                    blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.Blockquote, trimmed[2..].Trim()));
                    continue;
                }

                if (TryParseList(lines, ref i, ordered: false, out var unorderedItems))
                {
                    FlushParagraph();
                    blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.UnorderedList, string.Empty, Items: unorderedItems));
                    continue;
                }

                if (TryParseList(lines, ref i, ordered: true, out var orderedItems))
                {
                    FlushParagraph();
                    blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.OrderedList, string.Empty, Items: orderedItems));
                    continue;
                }

                paragraph.Add(trimmed);
            }

            if (inCode)
            {
                blocks.Add(new XFMarkdownBlock(XFMarkdownBlockKind.Code, string.Join(Environment.NewLine, code), Language: codeLanguage));
            }

            FlushParagraph();
            return blocks;
        }

        private static bool TryParseHeading(string line, out int level, out string text)
        {
            level = 0;
            text = string.Empty;

            while (level < line.Length && line[level] == '#')
            {
                level++;
            }

            if (level is < 1 or > 6 || level >= line.Length || line[level] != ' ')
            {
                return false;
            }

            text = line[(level + 1)..].Trim();
            return text.Length > 0;
        }

        private static bool TryParseList(string[] lines, ref int index, bool ordered, out IReadOnlyList<string> items)
        {
            var result = new List<string>();
            var current = index;

            while (current < lines.Length)
            {
                var line = lines[current].TrimStart();

                if (ordered)
                {
                    var dot = line.IndexOf('.', StringComparison.Ordinal);
                    if (dot <= 0 || !line[..dot].All(char.IsDigit) || dot + 1 >= line.Length || line[dot + 1] != ' ')
                    {
                        break;
                    }

                    result.Add(line[(dot + 2)..].Trim());
                }
                else if (line.StartsWith("- ", StringComparison.Ordinal) || line.StartsWith("* ", StringComparison.Ordinal))
                {
                    result.Add(line[2..].Trim());
                }
                else
                {
                    break;
                }

                current++;
            }

            items = result;

            if (result.Count == 0)
            {
                return false;
            }

            index = current - 1;
            return true;
        }

        private RenderFragment RenderInline(string text) => builder =>
        {
            var sequence = 0;
            RenderInlineRange(builder, ref sequence, text);
        };

        private void RenderInlineRange(RenderTreeBuilder builder, ref int sequence, string text)
        {
            var index = 0;

            while (index < text.Length)
            {
                var next = FindNextInlineToken(text, index);

                if (next < 0)
                {
                    builder.AddContent(sequence++, text[index..]);
                    return;
                }

                if (next > index)
                {
                    builder.AddContent(sequence++, text[index..next]);
                }

                if (TryRenderCode(builder, ref sequence, text, next, out var consumed)
                    || TryRenderStrong(builder, ref sequence, text, next, out consumed)
                    || TryRenderEmphasis(builder, ref sequence, text, next, out consumed)
                    || TryRenderLink(builder, ref sequence, text, next, out consumed))
                {
                    index = next + consumed;
                }
                else
                {
                    builder.AddContent(sequence++, text[next]);
                    index = next + 1;
                }
            }
        }

        private static int FindNextInlineToken(string text, int start)
        {
            var candidates = new[]
            {
                text.IndexOf('`', start),
                text.IndexOf("**", start, StringComparison.Ordinal),
                text.IndexOf('*', start),
                text.IndexOf('[', start)
            }.Where(value => value >= 0);

            return candidates.Any() ? candidates.Min() : -1;
        }

        private static bool TryRenderCode(RenderTreeBuilder builder, ref int sequence, string text, int start, out int consumed)
        {
            consumed = 0;

            if (text[start] != '`')
            {
                return false;
            }

            var end = text.IndexOf('`', start + 1);
            if (end < 0)
            {
                return false;
            }

            builder.OpenElement(sequence++, "code");
            builder.AddAttribute(sequence++, "class", "rounded bg-surface-100 px-1 py-0.5 font-mono text-[0.9em] text-primary ring-1 ring-border-light");
            builder.AddContent(sequence++, text[(start + 1)..end]);
            builder.CloseElement();
            consumed = end - start + 1;
            return true;
        }

        private void RenderNestedInline(RenderTreeBuilder builder, ref int sequence, string tag, string className, string content)
        {
            builder.OpenElement(sequence++, tag);
            builder.AddAttribute(sequence++, "class", className);
            RenderInlineRange(builder, ref sequence, content);
            builder.CloseElement();
        }

        private bool TryRenderStrong(RenderTreeBuilder builder, ref int sequence, string text, int start, out int consumed)
        {
            consumed = 0;

            if (!text[start..].StartsWith("**", StringComparison.Ordinal))
            {
                return false;
            }

            var end = text.IndexOf("**", start + 2, StringComparison.Ordinal);
            if (end < 0)
            {
                return false;
            }

            RenderNestedInline(builder, ref sequence, "strong", "font-semibold text-text-primary", text[(start + 2)..end]);
            consumed = end - start + 2;
            return true;
        }

        private bool TryRenderEmphasis(RenderTreeBuilder builder, ref int sequence, string text, int start, out int consumed)
        {
            consumed = 0;

            if (text[start] != '*' || text[start..].StartsWith("**", StringComparison.Ordinal))
            {
                return false;
            }

            var end = text.IndexOf('*', start + 1);
            if (end < 0)
            {
                return false;
            }

            RenderNestedInline(builder, ref sequence, "em", "italic", text[(start + 1)..end]);
            consumed = end - start + 1;
            return true;
        }

        private bool TryRenderLink(RenderTreeBuilder builder, ref int sequence, string text, int start, out int consumed)
        {
            consumed = 0;

            if (!AllowLinks || text[start] != '[')
            {
                return false;
            }

            var labelEnd = text.IndexOf("](", start, StringComparison.Ordinal);
            if (labelEnd < 0)
            {
                return false;
            }

            var urlEnd = text.IndexOf(')', labelEnd + 2);
            if (urlEnd < 0)
            {
                return false;
            }

            var label = text[(start + 1)..labelEnd];
            var url = text[(labelEnd + 2)..urlEnd].Trim();

            if (!IsSafeUrl(url))
            {
                builder.AddContent(sequence++, label);
                consumed = urlEnd - start + 1;
                return true;
            }

            builder.OpenElement(sequence++, "a");
            builder.AddAttribute(sequence++, "class", "font-medium text-primary underline underline-offset-4 hover:text-primary-hover");
            builder.AddAttribute(sequence++, "href", url);

            if (OpenLinksInNewTab)
            {
                builder.AddAttribute(sequence++, "target", "_blank");
                builder.AddAttribute(sequence++, "rel", "noopener noreferrer");
            }

            RenderInlineRange(builder, ref sequence, label);
            builder.CloseElement();
            consumed = urlEnd - start + 1;
            return true;
        }

        private static bool IsSafeUrl(string url)
        {
            if (url.StartsWith("#", StringComparison.Ordinal) || url.StartsWith("/", StringComparison.Ordinal))
            {
                return true;
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && uri.Scheme is "http" or "https" or "mailto";
        }
    }
}
