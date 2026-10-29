using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.List
{
    public partial class XFList : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-list";

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? EmptyContent { get; set; }
        [Parameter] public string EmptyText { get; set; } = default!;
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Dividers { get; set; } = true;
        [Parameter] public XFListSelectionMode SelectionMode { get; set; } = XFListSelectionMode.None;
        [Parameter] public string? SelectedValue { get; set; }
        [Parameter] public EventCallback<string?> SelectedValueChanged { get; set; }
        [Parameter] public IReadOnlyList<string> SelectedValues { get; set; } = [];
        [Parameter] public EventCallback<IReadOnlyList<string>> SelectedValuesChanged { get; set; }
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;
        [Parameter] public XFListVariant Variant { get; set; } = XFListVariant.Bordered;

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', ChildContent, EmptyContent, EmptyText, AriaLabel, Visible, Dividers, SelectionMode, SelectedValue, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string? Role => SelectionMode == XFListSelectionMode.None ? "list" : "listbox";

        private string? AriaMultiselectable => SelectionMode == XFListSelectionMode.Multiple ? "true" : null;

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(EmptyText)) EmptyText = L("list.empty", EmptyText);
            base.OnInitialized();
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {VariantClass}");
        }

        private string VariantClass => Variant switch
        {
            XFListVariant.Plain => "grid overflow-hidden",
            XFListVariant.Soft => "grid overflow-hidden rounded-lg bg-surface-50 ring-1 ring-border-light",
            _ => "grid overflow-hidden rounded-lg border border-border-light bg-bg-surface shadow-[0_1px_2px_rgb(15_23_42/.04)]"
        };

        internal string ItemSizeClass => Size switch
        {
            XFSize.Sm => "min-h-10 px-3 py-2 text-sm",
            XFSize.Lg => "min-h-16 px-5 py-4 text-base",
            _ => "min-h-12 px-4 py-3 text-sm"
        };

        internal string ItemDividerClass => Dividers ? "border-b border-border-light last:border-b-0" : "";

        internal bool IsSelected(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || SelectionMode == XFListSelectionMode.None)
            {
                return false;
            }

            return SelectionMode == XFListSelectionMode.Single
                ? SelectedValue == value
                : SelectedValues.Contains(value);
        }

        internal async Task SelectAsync(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || SelectionMode == XFListSelectionMode.None)
            {
                return;
            }

            if (SelectionMode == XFListSelectionMode.Single)
            {
                SelectedValue = value;
                await SelectedValueChanged.InvokeAsync(value);
                return;
            }

            var next = SelectedValues.ToList();
            if (!next.Remove(value))
            {
                next.Add(value);
            }

            SelectedValues = next;
            await SelectedValuesChanged.InvokeAsync(next);
        }

        private string EmptyClass => "px-4 py-8 text-center text-sm text-text-muted";
    }
}
