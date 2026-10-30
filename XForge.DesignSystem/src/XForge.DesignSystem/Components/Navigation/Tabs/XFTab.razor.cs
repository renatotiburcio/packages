using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Navigation.Tabs
{
    public partial class XFTab : XForgeComponentBase, IDisposable
    {
        private ElementReference buttonElement;

        protected override string BaseClass => "xf-tab";

        [CascadingParameter] internal XFTabs? Parent { get; set; }

        [Parameter] public string? TabId { get; set; }
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? Href { get; set; }
        [Parameter] public XFTabsRouteMatch? Match { get; set; }
        [Parameter] public object? Value { get; set; }
        [Parameter] public bool Closable { get; set; }
        [Parameter] public RenderFragment? StartIcon { get; set; }
        [Parameter] public RenderFragment? EndContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? LoadingContent { get; set; }
        [Parameter] public string CloseTabLabel { get; set; } = "Fechar aba";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', TabId, Name, Label, AriaLabel, Href, Match, Value, Closable, StartIcon, EndContent, ChildContent, LoadingContent, CloseTabLabel);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        internal string ResolvedTabId => !string.IsNullOrWhiteSpace(TabId) ? TabId! : (!string.IsNullOrWhiteSpace(Name) ? Name! : Id);
        private string TabDomId => Parent?.GetTabDomId(ResolvedTabId) ?? $"{ResolvedTabId}-tab";
        private string PanelId => Parent?.GetPanelId(ResolvedTabId) ?? $"{ResolvedTabId}-panel";
        private bool IsActive => Parent?.IsActive(ResolvedTabId) == true;
        private bool IsClosable => Closable || (Parent?.AllowCloseEnabled == true);
        private string? EffectiveAriaLabel => !string.IsNullOrWhiteSpace(AriaLabel)
            ? AriaLabel
            : ChildContent is not null && !string.IsNullOrWhiteSpace(Label) ? Label : null;

        protected override void OnInitialized()
        {
            Parent?.Register(this);
        }

        public void Dispose()
        {
            Parent?.Unregister(this);
        }

        internal async Task FocusAsync()
        {
            await buttonElement.FocusAsync();
        }

        private async Task SelectAsync()
        {
            if (Parent is not null)
            {
                if (!string.IsNullOrWhiteSpace(Href) && Parent is { Navigation: not null })
                {
                    Parent.Navigation.NavigateTo(Href!);
                }

                await Parent.SelectTabAsync(this);
            }
        }

        private async Task CloseAsync(MouseEventArgs args)
        {
            if (Parent is not null)
            {
                await Parent.CloseTabAsync(this);
            }
        }

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (Parent is null)
            {
                return;
            }

            if ((!Parent.IsVertical && args.Key == "ArrowRight") || (Parent.IsVertical && args.Key == "ArrowDown"))
            {
                await Parent.FocusRelativeAsync(this, 1);
            }
            else if ((!Parent.IsVertical && args.Key == "ArrowLeft") || (Parent.IsVertical && args.Key == "ArrowUp"))
            {
                await Parent.FocusRelativeAsync(this, -1);
            }
            else if (args.Key == "Home")
            {
                await Parent.FocusFirstAsync();
            }
            else if (args.Key == "End")
            {
                await Parent.FocusLastAsync();
            }
            else if ((args.Key == "Enter" || args.Key == " ") && Parent.ActivationMode == XFTabsActivationMode.Manual)
            {
                await Parent.SelectTabAsync(this);
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} {TabClass}");
        }

        private string TabClass => Join(
            "relative inline-flex min-h-9 items-center justify-center gap-2 whitespace-nowrap rounded-lg px-3 text-sm font-semibold outline-none transition-all duration-200 ease-[cubic-bezier(0.4,0,0.2,1)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:text-text-muted disabled:opacity-60",
            Parent?.Size == XFSize.Sm ? "min-h-8 px-2.5 text-xs" : null,
            Parent?.Size == XFSize.Lg ? "min-h-11 px-4 text-base" : null,
            Parent?.Variant switch
            {
                XFTabsVariant.Pills => IsActive
                    ? "bg-bg-surface text-primary shadow-[0_1px_2px_rgb(15_23_42/.08)]"
                    : "text-text-secondary hover:bg-bg-surface/70 hover:text-text-primary",
                XFTabsVariant.Enclosed => IsActive
                    ? "bg-[color-mix(in_oklab,var(--color-primary)_8%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_16%,transparent)]"
                    : "text-text-secondary hover:bg-surface-100 hover:text-text-primary",
                XFTabsVariant.Subtle => IsActive
                    ? "bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary"
                    : "text-text-secondary hover:bg-[color-mix(in_oklab,var(--color-primary)_6%,transparent)] hover:text-primary",
                _ => IsActive
                    ? "rounded-b-none text-primary after:absolute after:inset-x-2 after:-bottom-px after:h-0.5 after:rounded-full after:bg-primary"
                    : "rounded-b-none text-text-secondary hover:bg-[color-mix(in_oklab,var(--color-primary)_4%,transparent)] hover:text-text-primary"
            },
            Parent?.ShowIndicator == false ? "after:hidden" : null);

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
