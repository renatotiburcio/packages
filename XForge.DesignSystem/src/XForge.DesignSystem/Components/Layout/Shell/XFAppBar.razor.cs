using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFAppBar : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public XFAppBarPosition Position { get; set; } = XFAppBarPosition.Static;
        [Parameter] public bool Bordered { get; set; } = true;
        [Parameter] public string? AriaLabel { get; set; } = "Barra do aplicativo";
        [Parameter] public string InnerClass { get; set; } = default!;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, Position, Bordered, AriaLabel, InnerClass, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("appbar.label", AriaLabel);
            base.OnInitialized();
        }

        private string GetPositionClass() => Position switch
        {
            XFAppBarPosition.Fixed => "sticky top-0 shadow-[0_4px_12px_rgba(0,0,0,0.05)]",
            _ => ""
        };

        private string GetAppBarClass()
        {
            return GetClass($"flex w-full h-16 bg-bg-surface z-30 shrink-0 transition-theme {GetPositionClass()} {(Bordered ? "border-b border-border-light" : "")}");
        }
    }
}
