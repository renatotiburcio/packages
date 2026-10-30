using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using XForge.DesignSystem.Components.Icon;
using XForge.DesignSystem.Components.Shared;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.Shell
{
    public partial class XFRailItem : XForgeComponentBase, IDisposable
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        [Parameter] public RailNavItem? Item { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public bool Active { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }
        [Parameter] public bool ShowTooltip { get; set; } = true;
        [Parameter] public XFTooltipPosition TooltipPosition { get; set; } = XFTooltipPosition.Right;
        [Parameter] public XFTooltipTheme TooltipTheme { get; set; } = XFTooltipTheme.Dark;
        [Parameter] public string? Icon { get; set; }
        [Parameter] public string UnlabeledItemAriaLabel { get; set; } = "Item de navegacao";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Item, ChildContent, Label, Active, OnClick, ShowTooltip, TooltipPosition, TooltipTheme, Icon, UnlabeledItemAriaLabel,
                _isActive, NavigationState?.CurrentBrand);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        [Inject] private NavigationStateService? NavigationState { get; set; }

        private bool _isActive;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (NavigationState != null)
            {
                NavigationState.ActiveItemChanged += OnActiveItemChanged;
            NavigationState.BrandChanged += OnBrandChanged;
                UpdateActiveState();
            }
        }

        private void OnActiveItemChanged(object? sender, RailNavItem? activeItem)
        {
            UpdateActiveState();
            InvokeAsync(StateHasChanged);
        }

    private void OnBrandChanged(object? sender, string brand)
    {
        UpdateActiveState();
        InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        UpdateActiveState();
    }

        private void UpdateActiveState()
        {
            if (Item != null && NavigationState != null)
            {
            _isActive = Item.Type == RailNavItemType.Route &&
                !string.IsNullOrEmpty(NavigationState.CurrentBrand) &&
                Item.Brand.Equals(NavigationState.CurrentBrand, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                _isActive = Active;
            }
        }

        private string GetRailItemClass()
        {
            var isRoute = Item?.Type == RailNavItemType.Route;
            var isActive = isRoute && _isActive;

            var baseClass = Item?.Type switch
            {
                RailNavItemType.Separator => "flex items-center justify-center w-10 h-6 shrink-0 rounded border-none bg-transparent cursor-default",
                RailNavItemType.Action => "flex items-center justify-center w-10 h-10 shrink-0 rounded-lg border-none cursor-pointer text-primary-contrast transition-all duration-200 hover:bg-primary-contrast/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-contrast focus-visible:ring-offset-2 focus-visible:ring-offset-primary" + (isActive ? " bg-primary-contrast/20 font-medium" : " bg-transparent"),
                _ => "flex items-center justify-center w-10 h-10 shrink-0 rounded-lg border-none cursor-pointer text-primary-contrast transition-all duration-200 hover:bg-primary-contrast/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-contrast focus-visible:ring-offset-2 focus-visible:ring-offset-primary" + (isActive ? " bg-primary-contrast/20 font-medium" : " bg-transparent")
            };

            if (Disabled)
            {
                baseClass += " opacity-45 cursor-not-allowed pointer-events-none";
            }

            return baseClass;
        }

        private string GetTooltipClass()
        {
            return "flex h-10 w-10 shrink-0 items-center justify-center";
        }

        private bool ShouldRenderIcon => !string.IsNullOrWhiteSpace(IconValue);

        private string IconValue => Icon ?? Item?.Icon ?? string.Empty;

        private string EffectiveLabel
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Label))
                    return Label;

                if (!string.IsNullOrWhiteSpace(Item?.Label))
                    return Item.Label;

                return UnlabeledItemAriaLabel;
            }
        }

        private string? EffectiveSeparatorLabel => !string.IsNullOrWhiteSpace(Label)
            ? Label
            : !string.IsNullOrWhiteSpace(Item?.Label) ? Item.Label : null;

        private bool IsSvgIcon => IconValue.Contains('<', StringComparison.Ordinal);

        private RenderFragment RenderRailIcon() => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "inline-block h-5 w-5");
            builder.AddMarkupContent(2, IsSvgIcon ? IconValue : XFSvgIcons.Get(IconValue));
            builder.CloseElement();
        };

        private async Task HandleClickAsync()
        {
            if (Disabled)
                return;

            if (Item != null && Item.Type == RailNavItemType.Route && NavigationState != null)
            {
                NavigationState.NavigateToItem(Item);
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync();
            }
        }

        public void Dispose()
        {
            if (NavigationState != null)
            {
                NavigationState.ActiveItemChanged -= OnActiveItemChanged;
            NavigationState.BrandChanged -= OnBrandChanged;
            }
        }
    }
}

