using Microsoft.AspNetCore.Components;
using System.Globalization;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Avatar
{
    public partial class XFAvatarGroup : XForgeComponentBase
    {
        protected override string BaseClass => "xf-avatar-group";

        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Stacked { get; set; } = true;
        [Parameter] public XFAvatarSize Size { get; set; } = XFAvatarSize.Md;
        [Parameter] public int OverflowCount { get; set; }
        [Parameter] public string AriaLabel { get; set; } = "Grupo de usuarios";
        [Parameter] public string? OverflowLabel { get; set; }
        [Parameter] public string OverflowSingleLabelFormat { get; set; } = "{0} usuario adicional";
        [Parameter] public string OverflowMultipleLabelFormat { get; set; } = "{0} usuarios adicionais";
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Visible, Stacked, Size, OverflowCount, AriaLabel, OverflowLabel, OverflowSingleLabelFormat, OverflowMultipleLabelFormat);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override string GetComponentClass()
        {
            var spacing = Stacked ? "-space-x-1" : "gap-2";
            return GetClass($"{BaseClass} flex items-center {spacing} overflow-visible [&_.xf-avatar]:bg-bg-surface [&_.xf-avatar]:ring-2 [&_.xf-avatar]:ring-bg-surface");
        }

        private string OverflowClass => Size switch
        {
            XFAvatarSize.Xs => "z-10 inline-flex h-6 min-w-6 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-1 text-[10px] font-semibold text-text-secondary shadow-sm ring-1 ring-border-light",
            XFAvatarSize.Sm => "z-10 inline-flex h-8 min-w-8 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-1.5 text-xs font-semibold text-text-secondary shadow-sm ring-1 ring-border-light",
            XFAvatarSize.Lg => "z-10 inline-flex h-12 min-w-12 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-2 text-sm font-semibold text-text-secondary shadow-sm ring-1 ring-border-light",
            XFAvatarSize.Xl => "z-10 inline-flex h-16 min-w-16 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-2 text-base font-semibold text-text-secondary shadow-sm ring-1 ring-border-light",
            XFAvatarSize.Xxl => "z-10 inline-flex h-20 min-w-20 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-3 text-lg font-semibold text-text-secondary shadow-sm ring-1 ring-border-light",
            _ => "z-10 inline-flex h-10 min-w-10 items-center justify-center rounded-full border-2 border-bg-surface bg-bg-surface px-2 text-sm font-semibold text-text-secondary shadow-sm ring-1 ring-border-light"
        };

        private string ResolvedOverflowLabel
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(OverflowLabel))
                {
                    return OverflowLabel;
                }

                var format = OverflowCount == 1 ? OverflowSingleLabelFormat : OverflowMultipleLabelFormat;
                return string.Format(CultureInfo.CurrentCulture, format, OverflowCount);
            }
        }
    }
}
