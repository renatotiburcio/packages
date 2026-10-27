using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Navigation.Breadcrumb
{
    public sealed class XFBreadcrumbItemModel
    {
        public string? Text { get; set; }
        public string? Href { get; set; }
        public bool Current { get; set; }
        public RenderFragment? Icon { get; set; }
        public RenderFragment<XFBreadcrumbItemModel>? Template { get; set; }
    }
}
