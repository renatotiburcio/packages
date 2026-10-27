using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Navigation.RouterTabs
{
    public partial class XFRouterTabs : XForgeComponentBase, IDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-router-tabs";

        private readonly List<XFRouterTab> _tabs = new();
        private string? _activeTabId;
        private string? _defaultTitle;

        #region Parameters

        /// <summary>Tab definitions.</summary>
        [Parameter]
        public IReadOnlyList<XFRouterTabOption> Tabs { get; set; } = Array.Empty<XFRouterTabOption>();

        /// <summary>Whether to show the "add tab" button.</summary>
        [Parameter]
        public bool AllowNewTab { get; set; }

        /// <summary>Callback when a new tab is requested.</summary>
        [Parameter]
        public EventCallback<XFRouterTabNewEventArgs> OnNewTab { get; set; }

        /// <summary>Callback when a tab is closed.</summary>
        [Parameter]
        public EventCallback<XFRouterTabCloseEventArgs> OnTabClose { get; set; }

        /// <summary>Callback when the active tab changes.</summary>
        [Parameter]
        public EventCallback<XFRouterTabChangeEventArgs> OnTabChange { get; set; }

        /// <summary>Callback when a tab is marked as modified.</summary>
        [Parameter]
        public EventCallback<XFRouterTabModifiedEventArgs> OnTabModified { get; set; }

        /// <summary>Maximum number of tabs allowed. 0 = unlimited.</summary>
        [Parameter]
        public int MaxTabs { get; set; }

        /// <summary>Default title for new tabs.</summary>
        [Parameter]
        public string DefaultTitle
        {
            get => _defaultTitle ?? L("router_tabs.new_tab", "New Tab");
            set => _defaultTitle = value;
        }

        #endregion

        #region Labels

        private string AriaLabel => L("router_tabs.aria_label", "Tab navigation");
        private string AddTabText => L("router_tabs.add_tab", "Add tab");
        private string CloseTabText => L("router_tabs.close_tab", "Close {0}");
        private string ModifiedText => L("router_tabs.modified", "Modified");

        #endregion

        #region CSS

        private string TabListClass => string.Join(" ",
            "flex items-center gap-1 border-b border-border-light bg-surface-50 px-2 py-1",
            "overflow-x-auto scrollbar-none");

        private string TabClass(bool isActive)
        {
            return string.Join(" ",
                "group flex items-center gap-1 rounded-t-lg px-3 py-2 text-sm font-medium transition-colors",
                "outline-none focus-visible:ring-2 focus-visible:ring-[var(--color-primary)]",
                isActive
                    ? "border-b-2 border-[var(--color-primary)] bg-bg-surface text-primary"
                    : "text-text-secondary hover:bg-surface-100 hover:text-text-primary border-b-2 border-transparent");
        }

        private string AddTabButtonClass => string.Join(" ",
            "ml-1 inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-lg",
            "text-text-muted hover:bg-surface-100 hover:text-text-primary transition-colors");

        private string PanelContainerClass => "xf-router-tabs-panels min-h-[200px]";

        #endregion

        #region Lifecycle

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Navigation.LocationChanged += OnLocationChanged;
        }

        protected override void OnParametersSet()
        {
            SyncTabsFromParameters();
            SyncActiveTabFromRoute();
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
        }

        #endregion

        #region Tab Management

        private void SyncTabsFromParameters()
        {
            // Add tabs from parameters that don't exist yet
            foreach (var option in Tabs)
            {
                var existing = _tabs.FirstOrDefault(t => t.Id == option.Id);
                if (existing == null)
                {
                    _tabs.Add(new XFRouterTab
                    {
                        Id = option.Id,
                        Title = option.Title,
                        Route = option.Route,
                        Icon = option.Icon,
                        Closable = option.Closable,
                        IsModified = false
                    });
                }
                else
                {
                    existing.Title = option.Title;
                    existing.Route = option.Route;
                    existing.Icon = option.Icon;
                }
            }

            // Remove tabs that are no longer in parameters (unless they have unsaved changes)
            var toRemove = _tabs
                .Where(t => !Tabs.Any(p => p.Id == t.Id) && !t.IsModified)
                .ToList();
            foreach (var tab in toRemove)
            {
                _tabs.Remove(tab);
            }
        }

        private void SyncActiveTabFromRoute()
        {
            if (_activeTabId != null) return;

            var currentUri = new Uri(Navigation.Uri).AbsolutePath;
            var matchingTab = _tabs.FirstOrDefault(t =>
                t.Route != null && currentUri.StartsWith(t.Route, StringComparison.OrdinalIgnoreCase));

            if (matchingTab != null)
            {
                _activeTabId = matchingTab.Id;
            }
            else if (_tabs.Count > 0)
            {
                _activeTabId = _tabs[0].Id;
            }
        }

        private async Task ActivateTabAsync(string tabId)
        {
            if (_activeTabId == tabId) return;

            var previousId = _activeTabId;
            _activeTabId = tabId;

            var tab = _tabs.FirstOrDefault(t => t.Id == tabId);
            if (tab?.Route != null)
            {
                Navigation.NavigateTo(tab.Route);
            }

            await OnTabChange.InvokeAsync(new XFRouterTabChangeEventArgs
            {
                PreviousTabId = previousId,
                NewTabId = tabId,
                Tab = tab
            });
        }

        private async Task AddNewTabAsync()
        {
            if (MaxTabs > 0 && _tabs.Count >= MaxTabs) return;

            var tabId = $"tab-{Guid.NewGuid():N}";
            var newTab = new XFRouterTab
            {
                Id = tabId,
                Title = DefaultTitle,
                Closable = true,
                IsModified = false
            };

            _tabs.Add(newTab);

            await ActivateTabAsync(tabId);

            await OnNewTab.InvokeAsync(new XFRouterTabNewEventArgs
            {
                TabId = tabId,
                Tab = newTab
            });
        }

        private async Task CloseTabAsync(string tabId)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == tabId);
            if (tab == null) return;

            var args = new XFRouterTabCloseEventArgs
            {
                TabId = tabId,
                Tab = tab,
                IsModified = tab.IsModified
            };

            await OnTabClose.InvokeAsync(args);

            if (args.Cancel) return;

            var index = _tabs.IndexOf(tab);
            _tabs.Remove(tab);

            if (_activeTabId == tabId)
            {
                var newActive = _tabs.Count > 0
                    ? _tabs[Math.Min(index, _tabs.Count - 1)]
                    : null;
                _activeTabId = newActive?.Id;

                if (newActive?.Route != null)
                    Navigation.NavigateTo(newActive.Route);
            }
        }

        /// <summary>Marks a tab as modified (showing unsaved indicator).</summary>
        public void MarkTabModified(string tabId, bool isModified)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == tabId);
            if (tab == null) return;

            tab.IsModified = isModified;
            StateHasChanged();
        }

        /// <summary>Updates a tab's title.</summary>
        public void UpdateTabTitle(string tabId, string newTitle)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == tabId);
            if (tab == null) return;

            tab.Title = newTitle;
            StateHasChanged();
        }

        /// <summary>Gets the currently active tab ID.</summary>
        public string? GetActiveTabId() => _activeTabId;

        /// <summary>Gets all open tabs.</summary>
        public IReadOnlyList<XFRouterTab> GetOpenTabs() => _tabs.AsReadOnly();

        #endregion

        #region Navigation

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            SyncActiveTabFromRoute();
            StateHasChanged();
        }

        #endregion
    }

    #region Supporting types

    public class XFRouterTabOption
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Route { get; set; }
        public MarkupString? Icon { get; set; }
        public bool Closable { get; set; } = true;
    }

    public class XFRouterTab
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Route { get; set; }
        public MarkupString? Icon { get; set; }
        public bool Closable { get; set; } = true;
        public bool IsModified { get; set; }
        public RenderFragment? Content { get; set; }
    }

    public class XFRouterTabNewEventArgs
    {
        public string TabId { get; set; } = "";
        public XFRouterTab? Tab { get; set; }
    }

    public class XFRouterTabCloseEventArgs
    {
        public string TabId { get; set; } = "";
        public XFRouterTab? Tab { get; set; }
        public bool IsModified { get; set; }
        public bool Cancel { get; set; }
    }

    public class XFRouterTabChangeEventArgs
    {
        public string? PreviousTabId { get; set; }
        public string? NewTabId { get; set; }
        public XFRouterTab? Tab { get; set; }
    }

    public class XFRouterTabModifiedEventArgs
    {
        public string TabId { get; set; } = "";
        public bool IsModified { get; set; }
    }

    #endregion
}
