using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.DataDisplay.Chip
{
    public partial class XFChipList : XForgeComponentBase
    {
        protected override string BaseClass => "xf-chip-list";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment<string>? ItemTemplate { get; set; }
        [Parameter] public IEnumerable<string>? Items { get; set; }
        [Parameter] public ISet<string>? SelectedItems { get; set; }
        [Parameter] public EventCallback<string> ItemRemoved { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public bool Selectable { get; set; }
        [Parameter] public bool Wrap { get; set; } = true;
        [Parameter] public XFChipVariant ChipVariant { get; set; } = XFChipVariant.Solid;
        [Parameter] public XFSize ChipSize { get; set; } = XFSize.Md;
        [Parameter] public XFSemanticColor ChipTone { get; set; } = XFSemanticColor.Default;

        // assinatura de render: gráfico SVG e caro de repintar — pula render redundante
        // quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', ChildContent, ItemTemplate, Items is null ? 0 : Items.Count(), Title, AriaLabel, Selectable, Wrap, ChipVariant, ChipSize, ChipTone);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string RootClass => Join(
            BaseClass,
            "flex flex-col gap-1",
            Class);

        private string LayoutClass => Join(
            "flex items-center gap-1.5",
            Wrap ? "flex-wrap" : "overflow-x-auto");

        protected override string GetComponentClass()
        {
            return RootClass;
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
