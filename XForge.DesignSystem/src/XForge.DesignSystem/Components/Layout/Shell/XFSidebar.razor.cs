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
        [Parameter] public string HeaderClass { get; set; } = default!;
        [Parameter] public string ContentClass { get; set; } = default!;
        [Parameter] public string FooterClass { get; set; } = default!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("sidebar.navigation", AriaLabel);
            base.OnInitialized();
        }

        /// <summary>
        /// When true, the Drawer variant uses <c>absolute</c> positioning instead of <c>fixed</c>,
        /// keeping it contained within the nearest positioned parent container.
        /// Only applies when <see cref="Variant"/> is <see cref="XFSidebarVariant.Drawer"/>.
        /// The parent element must have <c>position: relative</c> for absolute positioning to work correctly.
        /// </summary>
        [Parameter] public bool ContainerRelative { get; set; } = false;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Header, ChildContent, Footer, Expanded, Variant, AriaLabel, HeaderClass, ContentClass, FooterClass, ContainerRelative);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

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
