using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.List
{
    public partial class XFListItem : XForgeComponentBase
    {
        protected override string BaseClass => "xf-list-item";

        [CascadingParameter] public XFList? ParentList { get; set; }

        [Parameter] public string? Value { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? MetadataText { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndIcon { get; set; }
        [Parameter] public RenderFragment? Metadata { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Selected { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Value, Title, Description, MetadataText, ChildContent, StartIcon, EndIcon, Metadata, Visible, Selected);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private bool IsSelectable => ParentList?.SelectionMode != XFListSelectionMode.None;

        private bool IsInteractive => IsSelectable || OnClick.HasDelegate;

        private bool IsSelected => Selected || ParentList?.IsSelected(Value) == true;

        private string? Role => IsSelectable ? "option" : "listitem";

        private string? AriaSelected => IsSelectable ? IsSelected.ToString().ToLowerInvariant() : null;

        private string? AriaDisabled => Disabled ? "true" : null;

        private int? TabIndexValue => IsInteractive && !Disabled ? 0 : null;

        protected override string GetComponentClass()
        {
            var size = ParentList?.ItemSizeClass ?? "min-h-12 px-4 py-3 text-sm";
            var divider = ParentList?.ItemDividerClass ?? "";
            var interactive = IsInteractive && !Disabled
                ? "cursor-pointer hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]"
                : "";
            var selected = IsSelected
                ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary ring-1 ring-inset ring-[color-mix(in_oklab,var(--color-primary)_22%,transparent)]"
                : "text-text-primary";
            var disabled = Disabled ? "cursor-not-allowed opacity-55" : "";

            return GetClass($"{BaseClass} flex items-center gap-3 transition-all duration-150 {size} {divider} {interactive} {selected} {disabled}");
        }

        private string IconClass => "grid h-8 w-8 shrink-0 place-items-center rounded-lg bg-current/8 text-current";

        private string TitleClass => "truncate font-semibold leading-5";

        private string DescriptionClass => "mt-0.5 line-clamp-2 text-sm leading-5 text-text-secondary";

        private string MetadataClass => "ml-3 shrink-0 text-xs font-medium text-text-muted";

        private async Task HandleClickAsync(MouseEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            if (ParentList is not null)
            {
                await ParentList.SelectAsync(Value);
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(args);
            }
        }
    }
}
