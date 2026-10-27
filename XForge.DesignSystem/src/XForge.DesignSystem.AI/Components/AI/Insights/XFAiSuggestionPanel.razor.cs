using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.AI.Insights
{
    public partial class XFAiSuggestionPanel : XForgeComponentBase
    {
        protected override string BaseClass => "xf-ai-suggestion-panel";

        // ── Parameters ──────────────────────────────────────────────────────

        [Parameter]
        public IReadOnlyList<XFAiSuggestion> Suggestions { get; set; } = Array.Empty<XFAiSuggestion>();

        [Parameter]
        public EventCallback<IReadOnlyList<XFAiSuggestion>> SuggestionsChanged { get; set; }

        [Parameter]
        public string Title { get; set; } = "Sugestoes inteligentes";

        [Parameter]
        public bool ShowTitle { get; set; } = true;

        [Parameter]
        public bool ShowIcons { get; set; } = true;

        [Parameter]
        public bool ShowDismiss { get; set; } = true;

        [Parameter]
        public bool ShowConfidence { get; set; }

        [Parameter]
        public int MaxSuggestions { get; set; } = 5;

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public string EmptyText { get; set; } = "Nenhuma sugestao no momento.";

        [Parameter]
        public string LoadingText { get; set; } = "Analisando...";

        [Parameter]
        public RenderFragment<XFAiSuggestion>? SuggestionTemplate { get; set; }

        [Parameter]
        public RenderFragment? HeaderContent { get; set; }

        [Parameter]
        public RenderFragment? EmptyContent { get; set; }

        [Parameter]
        public EventCallback<XFAiSuggestion> OnSuggestionClick { get; set; }

        [Parameter]
        public EventCallback<XFAiSuggestion> OnDismiss { get; set; }

        // ── Computed ────────────────────────────────────────────────────────

        private IReadOnlyList<XFAiSuggestion> VisibleSuggestions =>
            Suggestions.Take(MaxSuggestions).ToList();

        private bool HasSuggestions => VisibleSuggestions.Count > 0;

        // ── Styling ─────────────────────────────────────────────────────────

        protected override string GetComponentClass()
        {
            return GetClass(
                $"{BaseClass} overflow-hidden rounded-xl border border-border-light bg-bg-surface shadow-[0_1px_3px_rgb(15_23_42/.06),0_8px_24px_rgb(15_23_42/.04)]");
        }

        private string HeaderClass =>
            "flex items-center justify-between gap-2 border-b border-border-light px-4 py-3";

        private string TitleClass =>
            "text-sm font-semibold text-text-primary";

        private string ListClass =>
            "divide-y divide-border-light";

        private string SuggestionBaseClass =>
            "group relative flex items-start gap-3 px-4 py-3 outline-none transition-colors duration-150 hover:bg-surface-50 focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-border-focus";

        private string SuggestionDisabledClass =>
            Disabled ? "pointer-events-none opacity-50" : "cursor-pointer";

        private string IconContainerClass(XFAiSuggestionType type) =>
            $"mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-lg {TypeBgClass(type)}";

        private string IconClass(XFAiSuggestionType type) =>
            $"h-4 w-4 {TypeTextClass(type)}";

        private string SuggestionTitleClass =>
            "text-sm font-medium text-text-primary leading-5";

        private string SuggestionDescriptionClass =>
            "mt-0.5 text-xs text-text-secondary leading-4";

        private string ConfidenceBarContainerClass =>
            "mt-1.5 flex items-center gap-2";

        private string ConfidenceBarTrackClass =>
            "h-1 w-16 overflow-hidden rounded-full bg-surface-200";

        private string ConfidenceBarFillClass(XFAiSuggestionType type) =>
            $"h-full rounded-full transition-all duration-300 {TypeConfidenceClass(type)}";

        private string ConfidenceLabelClass =>
            "text-[10px] font-medium text-text-muted tabular-nums";

        private string DismissButtonClass =>
            "absolute right-3 top-3 inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md text-text-muted outline-none transition-all duration-150 opacity-0 group-hover:opacity-100 group-focus-within:opacity-100 hover:bg-surface-200 hover:text-text-secondary focus-visible:ring-2 focus-visible:ring-border-focus active:scale-95";

        private string EmptyClass =>
            "flex flex-col items-center justify-center gap-2 px-4 py-8 text-center";

        private string EmptyIconClass =>
            "h-8 w-8 text-text-muted";

        private string EmptyTextClass =>
            "text-sm text-text-secondary";

        private string LoadingSkeletonClass =>
            "animate-pulse";

        private string SkeletonItemClass =>
            "flex items-start gap-3 px-4 py-3";

        private string SkeletonCircleClass =>
            "h-8 w-8 shrink-0 rounded-lg bg-surface-200";

        private string SkeletonLineClass(int width) =>
            $"h-3 rounded bg-surface-200";

        private string TypeBorderClass(XFAiSuggestionType type) => type switch
        {
            XFAiSuggestionType.Action => "border-l-[3px] border-l-blue-500",
            XFAiSuggestionType.Insight => "border-l-[3px] border-l-purple-500",
            XFAiSuggestionType.Warning => "border-l-[3px] border-l-amber-500",
            XFAiSuggestionType.Recommendation => "border-l-[3px] border-l-emerald-500",
            _ => "border-l-[3px] border-l-slate-400"
        };

        private string TypeBgClass(XFAiSuggestionType type) => type switch
        {
            XFAiSuggestionType.Action => "bg-blue-50",
            XFAiSuggestionType.Insight => "bg-purple-50",
            XFAiSuggestionType.Warning => "bg-amber-50",
            XFAiSuggestionType.Recommendation => "bg-emerald-50",
            _ => "bg-slate-100"
        };

        private string TypeTextClass(XFAiSuggestionType type) => type switch
        {
            XFAiSuggestionType.Action => "text-blue-600",
            XFAiSuggestionType.Insight => "text-purple-600",
            XFAiSuggestionType.Warning => "text-amber-600",
            XFAiSuggestionType.Recommendation => "text-emerald-600",
            _ => "text-slate-500"
        };

        private string TypeConfidenceClass(XFAiSuggestionType type) => type switch
        {
            XFAiSuggestionType.Action => "bg-blue-500",
            XFAiSuggestionType.Insight => "bg-purple-500",
            XFAiSuggestionType.Warning => "bg-amber-500",
            XFAiSuggestionType.Recommendation => "bg-emerald-500",
            _ => "bg-slate-400"
        };

        private string DefaultIconForType(XFAiSuggestionType type) => type switch
        {
            XFAiSuggestionType.Action => "bolt",
            XFAiSuggestionType.Insight => "lightbulb",
            XFAiSuggestionType.Warning => "triangle-exclamation",
            XFAiSuggestionType.Recommendation => "circle-check",
            _ => "circle-info"
        };

        private string ConfidencePercent(double? confidence)
        {
            if (confidence is null) return string.Empty;
            var clamped = Math.Clamp(confidence.Value, 0, 1);
            return $"{clamped:P0}";
        }

        private double ConfidenceWidth(double? confidence)
        {
            if (confidence is null) return 0;
            return Math.Clamp(confidence.Value, 0, 1) * 100;
        }

        // ── Events ──────────────────────────────────────────────────────────

        private async Task HandleSuggestionClickAsync(XFAiSuggestion suggestion)
        {
            if (Disabled) return;

            if (OnSuggestionClick.HasDelegate)
            {
                await OnSuggestionClick.InvokeAsync(suggestion);
            }
        }

        private async Task HandleDismissAsync(XFAiSuggestion suggestion, MouseEventArgs _)
        {
            if (Disabled) return;

            var updated = Suggestions.Where(s => s.Id != suggestion.Id).ToList();

            if (SuggestionsChanged.HasDelegate)
            {
                await SuggestionsChanged.InvokeAsync(updated);
            }

            if (OnDismiss.HasDelegate)
            {
                await OnDismiss.InvokeAsync(suggestion);
            }
        }

        private async Task HandleSuggestionKeyDownAsync(KeyboardEventArgs args, XFAiSuggestion suggestion)
        {
            if (Disabled) return;

            switch (args.Key)
            {
                case "Enter":
                case " ":
                    await HandleSuggestionClickAsync(suggestion);
                    break;

                case "Delete":
                case "Backspace":
                    if (ShowDismiss)
                    {
                        await HandleDismissAsync(suggestion, default!);
                    }
                    break;
            }
        }
    }
}
