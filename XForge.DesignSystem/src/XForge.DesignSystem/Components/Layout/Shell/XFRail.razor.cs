using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFRail : XForgeComponentBase, IDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public IReadOnlyList<RailNavItem>? Items { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Navegacao principal";
        [Parameter] public int ActiveIndex { get; set; } = -1;
        [Parameter] public bool AutoTrackActiveItem { get; set; } = true;
        [Parameter] public bool ShowActiveIndicator { get; set; } = true;
        [Parameter] public bool RenderItemsWhenEmpty { get; set; } = true;
        [Parameter] public string? HeaderClass { get; set; }
        [Parameter] public string? ItemsClass { get; set; }
        [Parameter] public string? FooterClass { get; set; }
        [Parameter] public string IndicatorColor { get; set; } = "var(--color-primary-contrast)";
        [Parameter] public int IndicatorHeight { get; set; } = 28;

        [Inject] private NavigationStateService? NavigationState { get; set; }
    
        private int _computedActiveIndex = -1;

        /// <summary>
        /// The gap between items in pixels. Must match Tailwind's gap-4 (16px).
        /// </summary>
        public const int ItemGap = 16;

        /// <summary>
        /// Standard item height for Route and Action types.
        /// </summary>
        public const int ItemHeight = 40;

        /// <summary>
        /// Combined height of item + gap (40px item + 16px gap = 56px).
        /// Used for indicator positioning: activeIndex * 56.
        /// </summary>
        public const int ItemStep = 56;

        protected override async Task OnInitializedAsync()
        {
            UpdateActiveState();

            if (Items != null && Items.Count > 0 && NavigationState != null)
            {
                await NavigationState.InitializeAsync(Items);
                UpdateActiveState();
            }

            if (NavigationState != null)
            {
                NavigationState.ActiveIndexChanged += OnActiveIndexChanged;
            }
        }

        protected override void OnParametersSet()
        {
            UpdateActiveState();
        }

        private void OnActiveIndexChanged(object? sender, int index)
        {
            _computedActiveIndex = index;
            InvokeAsync(StateHasChanged);
        }

        private void UpdateActiveState()
        {
            _computedActiveIndex = NavigationState?.GetActiveIndex() ?? ActiveIndex;
        }

        public int GetActiveIndexForRender()
        {
            return AutoTrackActiveItem ? _computedActiveIndex : ActiveIndex;
        }

        /// <summary>
        /// Calculate the indicator top position accounting for separator heights.
        /// Separators are 24px (vs 40px for route/action items), so we can't use a simple multiple.
        /// Includes centering adjustment so the indicator is perfectly centered on the active item.
        /// </summary>
        public int GetIndicatorOffset()
        {
            var activeIdx = GetActiveIndexForRender();
            if (activeIdx < 0 || Items == null)
                return -1;

            if (activeIdx >= Items.Count || Items[activeIdx].IsDefault)
                return -1;

            // Sum heights of all items before the active item
            int offset = 0;
            for (int i = 0; i < activeIdx; i++)
            {
                if (Items[i].IsDefault)
                    continue;

                offset += Items[i].Height + ItemGap;
            }

            // Center the indicator on the active item
            var activeItem = Items[activeIdx];
            int indicatorHeight = NavigationStateService.IndicatorHeight;
            int centeringOffset = (activeItem.Height - indicatorHeight) / 2;
            offset += centeringOffset;

            return offset;
        }

        private bool ShouldRenderItemsFromModel => RenderItemsWhenEmpty && ChildContent == null && Items is { Count: > 0 };

        private IEnumerable<RailNavItem> MainItems => Items?.Where(x => !x.IsDefault) ?? Enumerable.Empty<RailNavItem>();

        private IEnumerable<RailNavItem> FooterItems => Items?.Where(x => x.IsDefault) ?? Enumerable.Empty<RailNavItem>();

private string GetRailClass()
        {
            return GetClass("flex w-16 shrink-0 flex-col items-center h-full z-50 overflow-visible py-4 bg-primary transition-theme");
        }

        private string GetHeaderClass()
        {
            return JoinClasses("flex items-center justify-center w-10 h-10 rounded-lg shrink-0 mb-8 bg-primary-contrast/20 text-primary-contrast", HeaderClass);
        }

        private string GetItemsClass()
        {
            return JoinClasses("relative flex-1 flex flex-col items-center gap-4 w-full overflow-visible overflow-y-auto overflow-x-hidden rail-scrollbar rail-items", ItemsClass);
        }

        private string GetFooterClass()
        {
            return JoinClasses("flex flex-col items-center gap-4 w-full pt-4", FooterClass);
        }

        private static string JoinClasses(params string?[] classes)
        {
            return string.Join(" ", classes.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        public void Dispose()
        {
            if (NavigationState != null)
            {
                NavigationState.ActiveIndexChanged -= OnActiveIndexChanged;
            }
        }
    }
}
