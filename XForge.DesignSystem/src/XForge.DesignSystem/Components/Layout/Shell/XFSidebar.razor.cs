using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFSidebar : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public RenderFragment? Header { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public bool Expanded { get; set; } = true;
        [Parameter] public XFSidebarVariant Variant { get; set; } = XFSidebarVariant.Persistent;
        [Parameter] public string? AriaLabel { get; set; } = "Navegacao secundaria";
        [Parameter] public string HeaderClass { get; set; } = "h-16 px-4 flex items-center gap-3 border-b border-border-light shrink-0 text-base font-bold text-text-primary transition-theme";
        [Parameter] public string ContentClass { get; set; } = "flex-1 overflow-y-auto overflow-x-hidden p-3 scrollbar-thin transition-theme";
        [Parameter] public string FooterClass { get; set; } = "p-3 border-t border-border-light shrink-0 transition-theme";

        protected override void OnInitialized()
        {
            AriaLabel = L("sidebar.navigation", AriaLabel);
            base.OnInitialized();
        }

        /// <summary>
        /// When true, the Drawer variant uses <c>absolute</c> positioning instead of <c>fixed</c>,
        /// keeping it contained within the nearest positioned parent container.
        /// Only applies when <see cref="Variant"/> is <see cref="XFSidebarVariant.Drawer"/>.
        /// The parent element must have <c>position: relative</c> for absolute positioning to work correctly.
        /// </summary>
        [Parameter] public bool ContainerRelative { get; set; } = false;

        private string GetVariantClass() => Variant switch
        {
            XFSidebarVariant.Drawer when ContainerRelative => "absolute left-0 top-0 bottom-0 h-full shadow-2xl",
            XFSidebarVariant.Drawer => "fixed left-0 top-0 bottom-0 h-screen shadow-2xl",
            XFSidebarVariant.Mini => "w-20 min-w-20",
            _ => ""
        };

        private string GetCollapsedClass() => !Expanded ? "!w-0 !min-w-0 border-r-0" : "";

        private string GetSidebarClass()
        {
            return GetClass($"flex flex-col shrink-0 h-full w-60 min-w-60 bg-bg-surface border-r border-border-light transition-[width,min-width,transform] duration-300 ease-out transition-theme overflow-hidden z-40 {GetVariantClass()} {GetCollapsedClass()}");
        }
    }
}
