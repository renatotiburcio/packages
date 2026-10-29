using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Routing;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Tabs
{
    public partial class XFTabs : XForgeComponentBase, IDisposable
    {
        private readonly List<XFTab> tabs = new();
        private string? currentActiveId;
        private string? previousActiveIdParameter;
        private XFTab? focusedTab;

        protected override string BaseClass => "xf-tabs";

        [Parameter] public string? ActiveId { get; set; }
        [Parameter] public EventCallback<string?> ActiveIdChanged { get; set; }
        [Parameter] public EventCallback<XFTabChangeEventArgs> TabChanged { get; set; }
        [Parameter] public EventCallback<XFTabCloseEventArgs> TabClose { get; set; }
        [Parameter] public XFTabsVariant Variant { get; set; } = XFTabsVariant.Underline;
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFTabsOrientation Orientation { get; set; } = XFTabsOrientation.Horizontal;
        [Parameter] public XFTabsActivationMode ActivationMode { get; set; } = XFTabsActivationMode.Automatic;
        [Parameter] public XFTabsRenderMode RenderMode { get; set; } = XFTabsRenderMode.Default;
        [Parameter] public XFTabsRouteMatch RouteMatch { get; set; } = XFTabsRouteMatch.Prefix;
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public bool ShowIndicator { get; set; } = true;
        [Parameter] public bool AllowClose { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Abas";
        [Parameter] public RenderFragment? StartContent { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public RenderFragment? Tabs { get; set; }
        [Parameter] public RenderFragment? Panels { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ActiveId, Variant, Size, Orientation, ActivationMode, RenderMode, RouteMatch, FullWidth, ShowIndicator, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [Inject] internal NavigationManager Navigation { get; set; } = default!;

        internal string? CurrentActiveId => currentActiveId;
        internal bool IsVertical => Orientation is XFTabsOrientation.Vertical or XFTabsOrientation.Right;
        internal bool IsBottom => Orientation == XFTabsOrientation.Bottom;
        internal bool IsRight => Orientation == XFTabsOrientation.Right;
        internal bool AllowCloseEnabled => AllowClose;

        protected override void OnInitialized()
        {
            currentActiveId = ActiveId;
            previousActiveIdParameter = ActiveId;
            Navigation.LocationChanged += OnLocationChanged;
        }

        protected override void OnParametersSet()
        {
            if (previousActiveIdParameter != ActiveId)
            {
                currentActiveId = ActiveId;
                previousActiveIdParameter = ActiveId;
            }

            if (string.IsNullOrWhiteSpace(currentActiveId))
            {
                TryMatchRoute();
            }
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            TryMatchRoute();
            InvokeAsync(StateHasChanged);
        }

        private void TryMatchRoute()
        {
            var uri = Navigation.Uri;
            var baseUri = Navigation.BaseUri;

            foreach (var tab in tabs.Where(t => !t.Disabled && !string.IsNullOrWhiteSpace(t.Href)))
            {
                var href = tab.Href!;
                var absoluteHref = href.StartsWith("/", StringComparison.Ordinal)
                    ? $"{baseUri.TrimEnd('/')}{href}"
                    : href;

                var matches = RouteMatch switch
                {
                    XFTabsRouteMatch.Exact => string.Equals(uri, absoluteHref, StringComparison.OrdinalIgnoreCase),
                    _ => uri.StartsWith(absoluteHref, StringComparison.OrdinalIgnoreCase)
                };

                if (matches && currentActiveId != tab.ResolvedTabId)
                {
                    currentActiveId = tab.ResolvedTabId;
                    focusedTab = tab;
                    if (ActiveIdChanged.HasDelegate)
                    {
                        _ = ActiveIdChanged.InvokeAsync(currentActiveId);
                    }
                    break;
                }
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {RootClass}");
        }

        internal void Register(XFTab tab)
        {
            if (!tabs.Contains(tab))
            {
                tabs.Add(tab);
            }

            if (!string.IsNullOrWhiteSpace(ActiveId)
                && string.Equals(tab.ResolvedTabId, ActiveId, StringComparison.Ordinal))
            {
                currentActiveId = ActiveId;
                focusedTab ??= tab;
            }

            if (string.IsNullOrWhiteSpace(currentActiveId) && !tab.Disabled)
            {
                if (!string.IsNullOrWhiteSpace(tab.Href))
                {
                    TryMatchRoute();
                }

                if (string.IsNullOrWhiteSpace(currentActiveId))
                {
                    currentActiveId = tab.ResolvedTabId;
                }
            }
        }

        internal void Unregister(XFTab tab)
        {
            tabs.Remove(tab);
        }

        internal bool IsActive(string tabId) => string.Equals(currentActiveId, tabId, StringComparison.Ordinal);

        internal int GetIndex(XFTab tab) => tabs.IndexOf(tab);

        internal string GetPanelId(string tabId) => $"{Id}-{tabId}-panel";

        internal string GetTabDomId(string tabId) => $"{Id}-{tabId}-tab";

        internal async Task SelectTabAsync(XFTab tab)
        {
            if (tab.Disabled)
            {
                return;
            }

            if (currentActiveId == tab.ResolvedTabId)
            {
                return;
            }

            currentActiveId = tab.ResolvedTabId;
            focusedTab = tab;
            previousActiveIdParameter = ActiveId;

            if (ActiveIdChanged.HasDelegate)
            {
                await ActiveIdChanged.InvokeAsync(currentActiveId);
            }

            if (TabChanged.HasDelegate)
            {
                await TabChanged.InvokeAsync(new XFTabChangeEventArgs(tab.ResolvedTabId, GetIndex(tab), tab));
            }

            await InvokeAsync(StateHasChanged);
        }

        internal async Task CloseTabAsync(XFTab tab)
        {
            if (TabClose.HasDelegate)
            {
                await TabClose.InvokeAsync(new XFTabCloseEventArgs(tab.ResolvedTabId, GetIndex(tab), tab));
            }
        }

        internal async Task FocusRelativeAsync(XFTab current, int direction)
        {
            var enabled = tabs.Where(tab => !tab.Disabled).ToArray();
            if (enabled.Length == 0)
            {
                return;
            }

            var currentIndex = Array.IndexOf(enabled, current);
            if (currentIndex < 0)
            {
                currentIndex = Array.FindIndex(enabled, tab => tab.ResolvedTabId == current.ResolvedTabId);
            }

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var next = enabled[(currentIndex + direction + enabled.Length) % enabled.Length];
            focusedTab = next;

            if (ActivationMode == XFTabsActivationMode.Automatic)
            {
                await SelectTabAsync(next);
            }

            await next.FocusAsync();
        }

        internal async Task FocusRelativeFromCurrentAsync(int direction)
        {
            var enabled = tabs.Where(tab => !tab.Disabled).ToArray();
            if (enabled.Length == 0)
            {
                return;
            }

            var anchor = focusedTab is not null && !focusedTab.Disabled
                ? focusedTab
                : enabled.FirstOrDefault(tab => IsActive(tab.ResolvedTabId)) ?? enabled[0];

            var currentIndex = Array.IndexOf(enabled, anchor);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var next = enabled[(currentIndex + direction + enabled.Length) % enabled.Length];
            focusedTab = next;

            if (ActivationMode == XFTabsActivationMode.Automatic)
            {
                await SelectTabAsync(next);
            }

            await next.FocusAsync();
        }

        internal async Task FocusFirstAsync()
        {
            var first = tabs.FirstOrDefault(tab => !tab.Disabled);
            if (first is not null)
            {
                await first.FocusAsync();
                focusedTab = first;
                if (ActivationMode == XFTabsActivationMode.Automatic)
                {
                    await SelectTabAsync(first);
                }
            }
        }

        internal async Task FocusLastAsync()
        {
            var last = tabs.LastOrDefault(tab => !tab.Disabled);
            if (last is not null)
            {
                await last.FocusAsync();
                focusedTab = last;
                if (ActivationMode == XFTabsActivationMode.Automatic)
                {
                    await SelectTabAsync(last);
                }
            }
        }

        private async Task HandleTabListKeyDownAsync(KeyboardEventArgs args)
        {
            var active = tabs.FirstOrDefault(tab => IsActive(tab.ResolvedTabId)) ?? tabs.FirstOrDefault(tab => !tab.Disabled);
            if (active is null)
            {
                return;
            }

            if ((!IsVertical && args.Key == "ArrowRight") || (IsVertical && args.Key == "ArrowDown"))
            {
                await FocusRelativeFromCurrentAsync(1);
            }
            else if ((!IsVertical && args.Key == "ArrowLeft") || (IsVertical && args.Key == "ArrowUp"))
            {
                await FocusRelativeFromCurrentAsync(-1);
            }
            else if (args.Key == "Home")
            {
                await FocusFirstAsync();
            }
            else if (args.Key == "End")
            {
                await FocusLastAsync();
            }
            else if ((args.Key == "Enter" || args.Key == " ") && ActivationMode == XFTabsActivationMode.Manual)
            {
                await SelectTabAsync(focusedTab ?? active);
            }
        }

        private string AriaOrientation => IsVertical ? "vertical" : "horizontal";

        private string RootClass
        {
            get
            {
                if (IsVertical && IsRight)
                {
                    return "grid gap-4 md:grid-cols-[1fr_minmax(12rem,16rem)]";
                }

                if (IsVertical)
                {
                    return "grid gap-4 md:grid-cols-[minmax(12rem,16rem)_1fr]";
                }

                return "grid gap-4";
            }
        }

        private string HeaderShellClass
        {
            get
            {
                var baseClass = IsVertical
                    ? "flex min-w-0 gap-2 flex-col"
                    : "flex min-w-0 gap-2 items-center";

                var positionOrder = IsBottom ? "order-2" : "order-1";

                var variantClass = Variant == XFTabsVariant.Enclosed
                    ? "rounded-lg border border-border-light bg-bg-surface p-1 shadow-[0_1px_2px_rgb(15_23_42/.04)]"
                    : null;

                var rightOrder = IsRight ? "md:col-start-2 md:row-start-1" : null;

                return Join(baseClass, positionOrder, variantClass, rightOrder);
            }
        }

        private string TabListClass => Join(
            "min-w-0",
            IsVertical ? "flex flex-col gap-1" : "flex items-center gap-1 overflow-x-auto",
            FullWidth && !IsVertical ? "w-full [&>[role=tab]]:flex-1" : null,
            FullWidth && IsVertical ? "[&>[role=tab]]:flex-1" : null,
            Variant == XFTabsVariant.Pills ? "rounded-lg bg-surface-100 p-1" : null,
            Variant == XFTabsVariant.Subtle ? "rounded-lg bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] p-1" : null,
            Variant == XFTabsVariant.Underline ? "border-b border-border-light overflow-x-hidden" : null);

        private string PanelShellClass
        {
            get
            {
                var baseClass = "min-w-0";
                var positionOrder = IsBottom ? "order-1" : "order-2";
                var verticalPosition = IsVertical && !IsRight ? "md:col-start-2 md:row-start-1" : null;

                return Join(baseClass, positionOrder, verticalPosition);
            }
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
