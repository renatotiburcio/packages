using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Layout.ProfileMenu
{
    public partial class XFProfileMenu : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-profile-menu";

        private bool _isOpen;
        private bool _hasFocus;

        #region Parameters

        /// <summary>User's display name.</summary>
        [Parameter]
        public string? UserName { get; set; }

        /// <summary>User's display name (overrides UserName).</summary>
        [Parameter]
        public string? DisplayName { get; set; }

        /// <summary>User's email.</summary>
        [Parameter]
        public string? Email { get; set; }

        /// <summary>User's avatar image URL.</summary>
        [Parameter]
        public string? ImageUrl { get; set; }

        /// <summary>Initials to show when no image is available.</summary>
        [Parameter]
        public string? Initials { get; set; }

        /// <summary>Subtitle text below the name.</summary>
        [Parameter]
        public string? Subtitle { get; set; }

        /// <summary>Menu items.</summary>
        [Parameter]
        public IReadOnlyList<XFProfileMenuItem> Items { get; set; } = Array.Empty<XFProfileMenuItem>();

        /// <summary>Badge count for notifications.</summary>
        [Parameter]
        public int BadgeCount { get; set; }

        /// <summary>Status indicator color.</summary>
        [Parameter]
        public XFProfileMenuStatus Status { get; set; } = XFProfileMenuStatus.None;

        /// <summary>Whether to show the user info section in the trigger.</summary>
        [Parameter]
        public bool ShowUserInfo { get; set; } = true;

        /// <summary>Whether to show the chevron icon.</summary>
        [Parameter]
        public bool ShowChevron { get; set; } = true;

        /// <summary>Whether to show the status indicator dot.</summary>
        [Parameter]
        public bool ShowStatusIndicator { get; set; } = true;

        /// <summary>Whether to show the dropdown header with user info.</summary>
        [Parameter]
        public bool ShowDropdownHeader { get; set; } = true;

        /// <summary>Whether the menu is open.</summary>
        [Parameter]
        public bool IsOpen { get; set; }

        /// <summary>Open state changed callback.</summary>
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        /// <summary>Item clicked callback.</summary>
        [Parameter]
        public EventCallback<XFProfileMenuItem> OnItemClick { get; set; }

        /// <summary>Logout clicked callback.</summary>
        [Parameter]
        public EventCallback OnLogout { get; set; }

        /// <summary>Custom child content (replaces default items).</summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        #endregion

        #region Labels

        private string AriaLabel => L("profilemenu.aria_label", "User menu");
        private string MenuLabel => L("profilemenu.menu_label", "User actions");

        #endregion

        #region CSS

        private string TriggerClass => string.Join(" ",
            "flex items-center gap-2 rounded-lg px-2 py-1.5 transition-colors",
            "hover:bg-surface-100 focus-visible:ring-2 focus-visible:ring-[var(--color-primary)]",
            "outline-none cursor-pointer");

        private string AvatarClass => string.Join(" ",
            "relative h-9 w-9 rounded-full flex items-center justify-center shrink-0",
            AvatarBgClass);

        private string AvatarBgClass => Status switch
        {
            XFProfileMenuStatus.Online => "bg-green-500",
            XFProfileMenuStatus.Away => "bg-amber-500",
            XFProfileMenuStatus.Busy => "bg-red-500",
            _ => "bg-[var(--color-primary)]"
        };

        private string StatusIndicatorClass => string.Join(" ",
            "absolute bottom-0 right-0 h-3 w-3 rounded-full border-2 border-bg-surface",
            Status switch
            {
                XFProfileMenuStatus.Online => "bg-green-500",
                XFProfileMenuStatus.Away => "bg-amber-500",
                XFProfileMenuStatus.Busy => "bg-red-500",
                XFProfileMenuStatus.Offline => "bg-gray-400",
                _ => ""
            });

        private string BadgeClass => "inline-flex items-center justify-center h-5 min-w-[1.25rem] rounded-full bg-danger px-1 text-[10px] font-bold text-white";

        private string DropdownClass => string.Join(" ",
            "absolute right-0 top-full mt-2 z-50 min-w-[220px] max-w-[320px]",
            "rounded-xl border border-border-light bg-bg-surface shadow-lg",
            "py-1 outline-none");

        private string ItemClass(XFProfileMenuItem item) => string.Join(" ",
            "flex w-full items-center gap-2 px-4 py-2 text-sm transition-colors",
            item.Disabled ? "opacity-50 cursor-not-allowed" : "cursor-pointer hover:bg-surface-100",
            item.Danger ? "text-danger" : "text-text-primary");

        private string ItemBadgeClass => "inline-flex items-center justify-center h-5 min-w-[1.25rem] rounded-full bg-surface-200 px-1 text-[10px] font-bold text-text-secondary";

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            _isOpen = IsOpen;
        }

        #endregion

        #region Toggle

        private async Task ToggleAsync()
        {
            _isOpen = !_isOpen;
            await IsOpenChanged.InvokeAsync(_isOpen);
            StateHasChanged();
        }

        private async Task CloseAsync()
        {
            if (!_isOpen) return;
            _isOpen = false;
            await IsOpenChanged.InvokeAsync(false);
            StateHasChanged();
        }

        #endregion

        #region Item Click

        private async Task OnItemClickAsync(XFProfileMenuItem item)
        {
            if (item.Disabled) return;

            await OnItemClick.InvokeAsync(item);
            await CloseAsync();
        }

        #endregion

        #region Focus

        private async Task OnFocusOutAsync(FocusEventArgs e)
        {
            // Delay to allow dropdown clicks to register
            await Task.Delay(150);
            if (!_hasFocus)
            {
                await CloseAsync();
            }
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Escape":
                    await CloseAsync();
                    break;
                case "ArrowDown":
                    if (_isOpen)
                        FocusNextItem(1);
                    break;
                case "ArrowUp":
                    if (_isOpen)
                        FocusNextItem(-1);
                    break;
                case "Home":
                    if (_isOpen)
                        FocusFirstItem();
                    break;
                case "End":
                    if (_isOpen)
                        FocusLastItem();
                    break;
            }
        }

        private void FocusNextItem(int direction)
        {
            // JS interop would be needed for actual focus management
            // For now, this is a placeholder for the keyboard navigation pattern
        }

        private void FocusFirstItem()
        {
            // Placeholder
        }

        private void FocusLastItem()
        {
            // Placeholder
        }

        #endregion
    }

    #region Supporting types

    public class XFProfileMenuItem
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
        public string? Icon { get; set; }
        public string? Href { get; set; }
        public string? Shortcut { get; set; }
        public int Badge { get; set; }
        public bool Disabled { get; set; }
        public bool Danger { get; set; }
        public bool IsDivider { get; set; }
        public EventCallback<XFProfileMenuItem> OnClick { get; set; }
    }

    public enum XFProfileMenuStatus
    {
        None,
        Online,
        Away,
        Busy,
        Offline
    }

    #endregion
}
