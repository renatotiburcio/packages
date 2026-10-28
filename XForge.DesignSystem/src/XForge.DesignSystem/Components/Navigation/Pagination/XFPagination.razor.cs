using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Navigation.Pagination
{
    public partial class XFPagination : XForgeComponentBase
    {
        protected override string BaseClass => "xf-pagination";

        private static readonly IReadOnlyList<int> DefaultPageSizeOptions = new[] { 10, 25, 50, 100 };

        [Parameter] public int Page { get; set; } = 1;
        [Parameter] public EventCallback<int> PageChanged { get; set; }
        [Parameter] public int PageSize { get; set; } = 10;
        [Parameter] public EventCallback<int> PageSizeChanged { get; set; }
        [Parameter] public int TotalItems { get; set; }
        [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = Array.Empty<int>();
        [Parameter] public EventCallback<XFPaginationChangedEventArgs> Change { get; set; }


        /// <summary>Fires when the pagination reload button is clicked. Compliant with D-001 On{Action} convention.</summary>
        [Parameter] public EventCallback<XFPaginationChangedEventArgs> OnReload { get; set; }

        [Parameter] public XFContainerVariant Variant { get; set; } = XFContainerVariant.Surface;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFDensity Density { get; set; } = XFDensity.Comfortable;
        [Parameter] public string AriaLabel { get; set; } = default!;
        [Parameter] public bool AutoHide { get; set; }
        [Parameter] public bool AllowReload { get; set; }
        [Parameter] public bool ShowSummary { get; set; } = true;
        [Parameter] public bool ShowFirstLast { get; set; } = true;
        [Parameter] public bool ShowPrevNext { get; set; } = true;
        [Parameter] public bool ShowPageNumbers { get; set; } = true;
        [Parameter] public bool ShowPageSize { get; set; }

        [Inject] private IXFLocalizer? Localizer { get; set; }

        private string L(string key, string fallback) => Localizer?[key] ?? fallback;
        [Parameter] public string PageSizeLabel { get; set; } = default!;
        [Parameter] public string PageSizeAriaLabel { get; set; } = default!;
        [Parameter] public string EmptySummaryText { get; set; } = default!;
        [Parameter] public string SummaryFormat { get; set; } = default!;
        [Parameter] public string FirstPageLabel { get; set; } = default!;
        [Parameter] public string PreviousPageLabel { get; set; } = default!;
        [Parameter] public string NextPageLabel { get; set; } = default!;
        [Parameter] public string LastPageLabel { get; set; } = default!;
        [Parameter] public string ReloadLabel { get; set; } = default!;
        [Parameter] public string CurrentPageLabelFormat { get; set; } = default!;
        [Parameter] public string GoToPageLabelFormat { get; set; } = default!;
        [Parameter] public XFSemanticColor ActiveColor { get; set; } = XFSemanticColor.Primary;
        [Parameter] public int SiblingCount { get; set; } = 1;
        [Parameter] public int BoundaryCount { get; set; } = 1;
        [Parameter] public RenderFragment<XFPaginationChangedEventArgs>? SummaryTemplate { get; set; }
        [Parameter] public RenderFragment<int>? PageTemplate { get; set; }
        [Parameter] public RenderFragment? FirstContent { get; set; }
        [Parameter] public RenderFragment? PreviousContent { get; set; }
        [Parameter] public RenderFragment? NextContent { get; set; }
        [Parameter] public RenderFragment? LastContent { get; set; }
        [Parameter] public RenderFragment? ReloadContent { get; set; }

        private int CurrentPageSize => Math.Max(1, PageSize);

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(Math.Max(0, TotalItems) / (double)CurrentPageSize));

        private int CurrentPage => Math.Clamp(Page, 1, TotalPages);

        private bool CanGoBack => CurrentPage > 1;

        private bool CanGoForward => CurrentPage < TotalPages;

        private bool ShouldAutoHide => AutoHide && TotalPages <= 1 && !ShowPageSize && PageSizeOptions.Count == 0;

        private XFPaginationChangedEventArgs CurrentArgs => new(CurrentPage, CurrentPageSize, Math.Max(0, TotalItems), TotalPages);

        private IReadOnlyList<int> NormalizedPageSizeOptions
        {
            get
            {
                var source = PageSizeOptions.Count > 0 ? PageSizeOptions : (ShowPageSize ? DefaultPageSizeOptions : Array.Empty<int>());
                return source
                    .Append(CurrentPageSize)
                    .Where(value => value > 0)
                    .Distinct()
                    .OrderBy(value => value)
                    .ToArray();
            }
        }

        private IReadOnlyList<int?> PageItems => CreatePageItems();

        private string SummaryText
        {
            get
            {
                if (TotalItems <= 0)
                {
                    return EmptySummaryText;
                }

                var start = ((CurrentPage - 1) * CurrentPageSize) + 1;
                var end = Math.Min(CurrentPage * CurrentPageSize, TotalItems);
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, SummaryFormat, start, end, TotalItems);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("pagination.label", AriaLabel);
            if (string.IsNullOrWhiteSpace(PageSizeLabel)) PageSizeLabel = L("pagination.page_size", PageSizeLabel);
            if (string.IsNullOrWhiteSpace(PageSizeAriaLabel)) PageSizeAriaLabel = L("pagination.page_size", PageSizeAriaLabel);
            if (string.IsNullOrWhiteSpace(EmptySummaryText)) EmptySummaryText = L("pagination.empty", EmptySummaryText);
            if (string.IsNullOrWhiteSpace(SummaryFormat)) SummaryFormat = L("pagination.range_format", SummaryFormat);
            if (string.IsNullOrWhiteSpace(FirstPageLabel)) FirstPageLabel = L("pagination.first", FirstPageLabel);
            if (string.IsNullOrWhiteSpace(PreviousPageLabel)) PreviousPageLabel = L("pagination.prev", PreviousPageLabel);
            if (string.IsNullOrWhiteSpace(NextPageLabel)) NextPageLabel = L("pagination.next", NextPageLabel);
            if (string.IsNullOrWhiteSpace(LastPageLabel)) LastPageLabel = L("pagination.last", LastPageLabel);
            if (string.IsNullOrWhiteSpace(ReloadLabel)) ReloadLabel = L("pagination.reload", ReloadLabel);
            if (string.IsNullOrWhiteSpace(CurrentPageLabelFormat)) CurrentPageLabelFormat = L("pagination.current_format", CurrentPageLabelFormat);
            if (string.IsNullOrWhiteSpace(GoToPageLabelFormat)) GoToPageLabelFormat = L("pagination.goto_format", GoToPageLabelFormat);
        }

        protected override void OnParametersSet()
        {
            if (SiblingCount < 0)
            {
                SiblingCount = 0;
            }

            if (BoundaryCount < 0)
            {
                BoundaryCount = 0;
            }
        }

        protected override string GetComponentClass()
        {
            var variantClass = Variant switch
            {
                XFContainerVariant.Plain => "gap-3",
                XFContainerVariant.Soft => "rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-2",
                _ => "rounded-lg border border-border-light bg-bg-surface p-2 shadow-[0_1px_2px_rgb(15_23_42/.04)]"
            };

            return GetClass($"{BaseClass} flex flex-col gap-3 text-text-primary sm:flex-row sm:items-center sm:justify-between {variantClass}");
        }

        private IReadOnlyList<int?> CreatePageItems()
        {
            if (TotalPages <= 0)
            {
                return Array.Empty<int?>();
            }

            var boundary = Math.Max(0, BoundaryCount);
            var sibling = Math.Max(0, SiblingCount);
            var totalNumbers = (boundary * 2) + (sibling * 2) + 3;

            if (TotalPages <= totalNumbers)
            {
                return Enumerable.Range(1, TotalPages).Cast<int?>().ToArray();
            }

            var pages = new List<int?>();
            var startPages = Enumerable.Range(1, boundary);
            var endPages = Enumerable.Range(TotalPages - boundary + 1, boundary);
            var siblingStart = Math.Max(boundary + 2, CurrentPage - sibling);
            var siblingEnd = Math.Min(TotalPages - boundary - 1, CurrentPage + sibling);

            pages.AddRange(startPages.Cast<int?>());

            if (siblingStart > boundary + 2)
            {
                pages.Add(null);
            }
            else if (boundary + 1 < TotalPages - boundary)
            {
                pages.Add(boundary + 1);
            }

            for (var page = siblingStart; page <= siblingEnd; page++)
            {
                pages.Add(page);
            }

            if (siblingEnd < TotalPages - boundary - 1)
            {
                pages.Add(null);
            }
            else if (TotalPages - boundary > boundary)
            {
                pages.Add(TotalPages - boundary);
            }

            pages.AddRange(endPages.Cast<int?>());

            return pages
                .Where(item => item is null || item.Value >= 1 && item.Value <= TotalPages)
                .ToArray();
        }

        private async Task GoToPageAsync(int page)
        {
            if (Disabled)
            {
                return;
            }

            var targetPage = Math.Clamp(page, 1, TotalPages);
            if (targetPage == CurrentPage)
            {
                return;
            }

            await PageChanged.InvokeAsync(targetPage);
            await Change.InvokeAsync(new XFPaginationChangedEventArgs(targetPage, CurrentPageSize, Math.Max(0, TotalItems), TotalPages));
        }

        private async Task HandlePageSizeChangeAsync(ChangeEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            if (!int.TryParse(args.Value?.ToString(), out var nextPageSize) || nextPageSize <= 0)
            {
                return;
            }

            await PageSizeChanged.InvokeAsync(nextPageSize);

            var nextTotalPages = Math.Max(1, (int)Math.Ceiling(Math.Max(0, TotalItems) / (double)nextPageSize));
            var nextPage = Math.Clamp(CurrentPage, 1, nextTotalPages);

            await PageChanged.InvokeAsync(nextPage);
            await Change.InvokeAsync(new XFPaginationChangedEventArgs(nextPage, nextPageSize, Math.Max(0, TotalItems), nextTotalPages));
        }

        private async Task HandlePageSizeSelectAsync(int nextPageSize)
        {
            if (Disabled || nextPageSize <= 0)
            {
                return;
            }

            await PageSizeChanged.InvokeAsync(nextPageSize);

            var nextTotalPages = Math.Max(1, (int)Math.Ceiling(Math.Max(0, TotalItems) / (double)nextPageSize));
            var nextPage = Math.Clamp(CurrentPage, 1, nextTotalPages);

            await PageChanged.InvokeAsync(nextPage);
            await Change.InvokeAsync(new XFPaginationChangedEventArgs(nextPage, nextPageSize, Math.Max(0, TotalItems), nextTotalPages));
        }

        private Task HandleReloadAsync()
        {
            return Disabled ? Task.CompletedTask : OnReload.InvokeAsync(CurrentArgs);
        }

        private RenderFragment RenderPageButton(int page) => builder =>
        {
            var isCurrent = page == CurrentPage;
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "button");
            builder.AddAttribute(2, "class", PageButtonClass(isCurrent));
            builder.AddAttribute(3, "disabled", Disabled || isCurrent);
            builder.AddAttribute(4, "aria-label", isCurrent ? FormatPageLabel(CurrentPageLabelFormat, page) : FormatPageLabel(GoToPageLabelFormat, page));
            builder.AddAttribute(5, "aria-disabled", (Disabled || isCurrent).ToString().ToLowerInvariant());

            if (isCurrent)
            {
                builder.AddAttribute(6, "aria-current", "page");
            }

            builder.AddAttribute(7, "data-xf-pagination-page", page.ToString());
            builder.AddAttribute(8, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => GoToPageAsync(page)));

            if (PageTemplate is not null)
            {
                builder.AddContent(9, PageTemplate(page));
            }
            else
            {
                builder.AddContent(10, page);
            }

            builder.CloseElement();
        };

        private RenderFragment RenderNavButton(string name, string label, int page, bool disabled, RenderFragment? content, RenderFragment fallback) => builder =>
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "button");
            builder.AddAttribute(2, "class", NavButtonClass(disabled));
            builder.AddAttribute(3, "disabled", Disabled || disabled);
            builder.AddAttribute(4, "title", label);
            builder.AddAttribute(5, "aria-label", label);
            builder.AddAttribute(6, "aria-disabled", (Disabled || disabled).ToString().ToLowerInvariant());
            builder.AddAttribute(7, $"data-xf-pagination-{name}", string.Empty);
            builder.AddAttribute(8, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => GoToPageAsync(page)));
            builder.AddContent(9, content ?? fallback);
            builder.CloseElement();
        };

        private static string FormatPageLabel(string format, int page) =>
            string.Format(System.Globalization.CultureInfo.CurrentCulture, format, page);

        private string ControlsClass => "flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-end";

        private string ButtonsClass => Density == XFDensity.Compact ? "flex flex-wrap items-center gap-1" : "flex flex-wrap items-center gap-1.5";

        private string SummaryClass => Size switch
        {
            XFSize.Sm => "text-xs font-medium text-text-secondary",
            XFSize.Lg => "text-base font-medium text-text-secondary",
            _ => "text-sm font-medium text-text-secondary"
        };

        private string EllipsisClass => Size switch
        {
            XFSize.Sm => "inline-flex h-7 min-w-7 items-center justify-center px-1 text-xs text-text-muted",
            XFSize.Lg => "inline-flex h-10 min-w-10 items-center justify-center px-1 text-base text-text-muted",
            _ => "inline-flex h-9 min-w-9 items-center justify-center px-1 text-sm text-text-muted"
        };

        private string ActiveColorVar => ActiveColor switch
        {
            XFSemanticColor.Secondary => "var(--color-secondary)",
            XFSemanticColor.Success => "var(--color-success)",
            XFSemanticColor.Warning => "var(--color-warning)",
            XFSemanticColor.Danger => "var(--color-danger)",
            _ => "var(--color-primary)"
        };

        private string ActiveBgClass => ActiveColor switch
        {
            XFSemanticColor.Secondary => "bg-secondary text-secondary-contrast",
            XFSemanticColor.Success => "bg-success text-success-contrast",
            XFSemanticColor.Warning => "bg-warning text-warning-contrast",
            XFSemanticColor.Danger => "bg-danger text-danger-contrast",
            _ => "bg-primary text-primary-contrast"
        };

        private string ActiveTextClass => ActiveColor switch
        {
            XFSemanticColor.Secondary => "text-secondary",
            XFSemanticColor.Success => "text-success",
            XFSemanticColor.Warning => "text-warning",
            XFSemanticColor.Danger => "text-danger",
            _ => "text-primary"
        };

        private string PageButtonClass(bool isCurrent)
        {
            var sizeClass = Size switch
            {
                XFSize.Sm => "h-7 min-w-7 px-2 text-xs",
                XFSize.Lg => "h-10 min-w-10 px-3 text-base",
                _ => "h-9 min-w-9 px-2.5 text-sm"
            };

            var stateClass = isCurrent
                ? $"cursor-default border-[{ActiveColorVar}] {ActiveBgClass} shadow-[0_4px_10px_rgb(15_23_42/.12)]"
                : $"border-border-light bg-bg-surface text-text-secondary hover:border-[{ActiveColorVar}] hover:bg-[color-mix(in_oklab,{ActiveColorVar}_6%,transparent)] hover:{ActiveTextClass} focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,{ActiveColorVar}_22%,transparent)]";

            return $"inline-flex items-center justify-center rounded-md border font-semibold outline-none transition-all duration-150 disabled:pointer-events-none disabled:opacity-70 {sizeClass} {stateClass}";
        }

        private string NavButtonClass(bool edgeDisabled)
        {
            var sizeClass = Size switch
            {
                XFSize.Sm => "h-7 w-7",
                XFSize.Lg => "h-10 w-10",
                _ => "h-9 w-9"
            };

            var stateClass = edgeDisabled
                ? "border-border-light bg-surface-100 text-text-muted"
                : $"border-border-light bg-bg-surface text-text-secondary hover:border-[{ActiveColorVar}] hover:bg-[color-mix(in_oklab,{ActiveColorVar}_6%,transparent)] hover:{ActiveTextClass} focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,{ActiveColorVar}_22%,transparent)]";

            return $"inline-flex items-center justify-center rounded-md border outline-none transition-all duration-150 disabled:pointer-events-none disabled:opacity-70 {sizeClass} {stateClass}";
        }

        private RenderFragment RenderChevronLeft => builder => RenderSvg(builder, "M15 18l-6-6 6-6");

        private RenderFragment RenderChevronRight => builder => RenderSvg(builder, "M9 18l6-6-6-6");

        private RenderFragment RenderDoubleChevronLeft => builder =>
        {
            RenderSvg(builder, "M11 17l-5-5 5-5M18 17l-5-5 5-5");
        };

        private RenderFragment RenderDoubleChevronRight => builder =>
        {
            RenderSvg(builder, "M13 17l5-5-5-5M6 17l5-5-5-5");
        };

        private RenderFragment RenderReload => builder =>
        {
            RenderSvg(builder, "M21 12a9 9 0 11-2.64-6.36M21 3v6h-6");
        };

        private static void RenderSvg(RenderTreeBuilder builder, string path)
        {
            builder.OpenElement(0, "svg");
            builder.AddAttribute(1, "class", "h-4 w-4");
            builder.AddAttribute(2, "viewBox", "0 0 24 24");
            builder.AddAttribute(3, "fill", "none");
            builder.AddAttribute(4, "stroke", "currentColor");
            builder.AddAttribute(5, "stroke-width", "2");
            builder.AddAttribute(6, "stroke-linecap", "round");
            builder.AddAttribute(7, "stroke-linejoin", "round");
            builder.OpenElement(8, "path");
            builder.AddAttribute(9, "d", path);
            builder.CloseElement();
            builder.CloseElement();
        }
    }
}