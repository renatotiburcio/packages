using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.VirtualList
{
    public partial class XFVirtualList<TItem> : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-virtual-list";

        [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public RenderFragment? LoadingContent { get; set; }
        [Parameter] public ICollection<TItem>? Items { get; set; }
        [Parameter] public ItemsProviderDelegate<TItem>? ItemsProvider { get; set; }
        [Parameter] public float ItemSize { get; set; } = 50;
        [Parameter] public int OverscanCount { get; set; } = 3;
        [Parameter] public string? Title { get; set; }
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public XFVirtualListVariant Variant { get; set; } = XFVirtualListVariant.Default;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ItemTemplate, EmptyContent, LoadingContent, Items.Count, ItemsProvider, ItemSize, OverscanCount, Title, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        protected override void OnInitialized()
        {
            // honra o valor do usuário quando informado; aplica o default localizado caso contrário
            if (string.IsNullOrWhiteSpace(EmptyText))
            {
                if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("virtuallist.empty", "Nenhum item para exibir.");
            }

            base.OnInitialized();
        }

        private string RootClass => Join(
            BaseClass,
            VariantClass,
            Class);

        private string VariantClass => Variant switch
        {
            XFVirtualListVariant.Bordered => "rounded-lg border border-border-light bg-bg-surface",
            XFVirtualListVariant.Compact => "",
            _ => "rounded-lg bg-bg-surface"
        };

        private string EmptyClass => "px-3 py-6 text-center text-sm text-text-muted";
        private string PlaceholderClass => "px-3 py-2";

        protected override string GetComponentClass()
        {
            return RootClass;
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
