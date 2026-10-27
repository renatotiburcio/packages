using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Navigation.Tabs
{
    public partial class XFTabPanel : XForgeComponentBase
    {
        private bool hasLoaded;

        protected override string BaseClass => "xf-tab-panel";

        [CascadingParameter] internal XFTabs? Parent { get; set; }

        [Parameter] public string TabId { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? LoadingContent { get; set; }

        private bool IsActive => Parent?.IsActive(TabId) == true;
        private string PanelId => Parent?.GetPanelId(TabId) ?? $"{TabId}-panel";
        private string TabDomId => Parent?.GetTabDomId(TabId) ?? $"{TabId}-tab";
        private string AriaHidden => (!IsActive).ToString().ToLowerInvariant();

        private bool IsLazyLoad => Parent?.RenderMode == XFTabsRenderMode.LazyLoad;
        private bool ShouldShowLoading => IsLazyLoad && !hasLoaded && IsActive;

        protected override void OnParametersSet()
        {
            if (IsActive)
            {
                hasLoaded = true;
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} rounded-lg text-sm leading-6 text-text-secondary outline-none transition-all duration-200 data-[state=active]:translate-y-0 data-[state=active]:opacity-100");
        }

        private bool ShouldRenderPanel => Parent?.RenderMode != XFTabsRenderMode.LazyReload || IsActive;

        private bool ShouldRenderContent => Parent?.RenderMode switch
        {
            XFTabsRenderMode.LazyLoad => IsActive || hasLoaded,
            XFTabsRenderMode.LazyReload => IsActive,
            _ => true
        };
    }
}
