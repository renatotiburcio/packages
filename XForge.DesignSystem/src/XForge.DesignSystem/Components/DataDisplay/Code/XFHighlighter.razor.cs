using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.DataDisplay.Code
{
    public partial class XFHighlighter : XForgeComponentBase
    {
        protected override string BaseClass => "xf-highlighter";

        [Parameter] public string? Text { get; set; }
        [Parameter] public string? HighlightedText { get; set; }
        [Parameter] public IReadOnlyList<string>? HighlightedTexts { get; set; }
        [Parameter] public bool CaseSensitive { get; set; }
        [Parameter] public bool WholeWord { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Primary;

        private IReadOnlyList<Fragment> Fragments { get; set; } = Array.Empty<Fragment>();

        protected override void OnParametersSet()
        {
            Fragments = BuildFragments();
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} contents");
        }

        private string MarkClass => $"rounded px-0.5 py-px font-semibold {MarkToneClass}";

        private string MarkToneClass => Tone switch
        {
            XFSemanticColor.Success => "bg-[color-mix(in_oklab,var(--color-success)_16%,transparent)] text-success",
            XFSemanticColor.Warning => "bg-[color-mix(in_oklab,var(--color-warning)_18%,transparent)] text-warning",
            XFSemanticColor.Danger => "bg-[color-mix(in_oklab,var(--color-danger)_16%,transparent)] text-danger",
            XFSemanticColor.Neutral => "bg-surface-200 text-text-primary",
            _ => "bg-[color-mix(in_oklab,var(--color-primary)_14%,transparent)] text-primary"
        };

        private IReadOnlyList<Fragment> BuildFragments()
        {
            var source = Text ?? string.Empty;
            if (string.IsNullOrEmpty(source))
            {
                return Array.Empty<Fragment>();
            }

            var terms = (HighlightedTexts ?? Array.Empty<string>())
                .Concat(new[] { HighlightedText })
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .Select(term => Regex.Escape(term!.Trim()))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (terms.Length == 0)
            {
                return new[] { new Fragment(source, false) };
            }

            var pattern = WholeWord
                ? $@"\b({string.Join("|", terms)})\b"
                : $"({string.Join("|", terms)})";

            var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            var regex = new Regex(pattern, options | RegexOptions.CultureInvariant);
            var parts = regex.Split(source).Where(part => part.Length > 0);

            return parts.Select(part => new Fragment(part, regex.IsMatch(part))).ToArray();
        }

        private sealed record Fragment(string Text, bool IsMatch);
    }
}
