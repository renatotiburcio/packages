using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Layout.ProfileMenu;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Layout.ProfileMenu;

public class XFProfileMenuTest : BunitContext
{
    public XFProfileMenuTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_ShowsAvatar()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.UserName, "John Doe"));
        Assert.Contains("xf-profile-menu", cut.Markup);
        Assert.Contains("J", cut.Markup);
    }

    [Fact]
    public void Render_WithImage_ShowsImage()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.ImageUrl, "avatar.jpg"));
        Assert.Contains("avatar.jpg", cut.Markup);
    }

    [Fact]
    public void Render_WithInitials_ShowsInitials()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Initials, "JD"));
        Assert.Contains("JD", cut.Markup);
    }

    [Fact]
    public void Render_WithDisplayName_ShowsName()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.DisplayName, "John Doe"));
        Assert.Contains("John Doe", cut.Markup);
    }

    [Fact]
    public void Render_WithSubtitle_ShowsSubtitle()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Subtitle, "Admin"));
        Assert.Contains("Admin", cut.Markup);
    }

    [Fact]
    public void Render_WithEmail_ShowsEmail()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Email, "john@example.com");
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("john@example.com", cut.Markup);
    }

    [Fact]
    public void Render_WithBadgeCount_ShowsBadge()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.BadgeCount, 5));
        Assert.Contains("5", cut.Markup);
    }

    [Fact]
    public void Render_WithOnlineStatus_ShowsGreenDot()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Status, XFProfileMenuStatus.Online));
        Assert.Contains("bg-green-500", cut.Markup);
    }

    [Fact]
    public void Render_WithAwayStatus_ShowsAmberDot()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Status, XFProfileMenuStatus.Away));
        Assert.Contains("bg-amber-500", cut.Markup);
    }

    [Fact]
    public void Render_WithBusyStatus_ShowsRedDot()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Status, XFProfileMenuStatus.Busy));
        Assert.Contains("bg-red-500", cut.Markup);
    }

    [Fact]
    public void Render_WithoutStatusIndicator_HidesDot()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.ShowStatusIndicator, false));
        Assert.DoesNotContain("bg-green-500", cut.Markup);
    }

    [Fact]
    public void Render_WithItems_ShowsMenuItems()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "profile", Label = "My Profile", Icon = "<path d='M12 12c2 0 3.5-1.5 3.5-3.5S14 5 12 5 8.5 6.5 8.5 8.5 10 12 12 12z' />" },
            new XFProfileMenuItem { Id = "settings", Label = "Settings", Icon = "<path d='M12 15.5A3.5 3.5 0 0 1 8.5 12 3.5 3.5 0 0 1 12 8.5a3.5 3.5 0 0 1 3.5 3.5 3.5 3.5 0 0 1-3.5 3.5z' />" }
        };

        var cut = Render<XFProfileMenu>(p => p.Add(x => x.Items, items));
        Assert.Contains("xf-profile-menu", cut.Markup);
    }

    [Fact]
    public void Render_Open_ShowsDropdown()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "profile", Label = "My Profile" }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("My Profile", cut.Markup);
    }

    [Fact]
    public void Render_WithDivider_ShowsDivider()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "profile", Label = "My Profile" },
            new XFProfileMenuItem { Id = "divider1", IsDivider = true },
            new XFProfileMenuItem { Id = "logout", Label = "Logout", Danger = true }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("My Profile", cut.Markup);
        Assert.Contains("Logout", cut.Markup);
    }

    [Fact]
    public void Render_WithDangerItem_ShowsDangerStyle()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "logout", Label = "Logout", Danger = true }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("text-danger", cut.Markup);
    }

    [Fact]
    public void Render_WithDisabledItem_ShowsDisabledStyle()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "profile", Label = "My Profile", Disabled = true }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("opacity-50", cut.Markup);
    }

    [Fact]
    public void Render_WithItemBadge_ShowsBadge()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "notifications", Label = "Notifications", Badge = 3 }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("3", cut.Markup);
    }

    [Fact]
    public void Render_WithItemShortcut_ShowsShortcut()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "settings", Label = "Settings", Shortcut = "Ctrl+," }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("Ctrl+,", cut.Markup);
    }

    [Fact]
    public void Render_WithoutShowUserInfo_HidesUserInfo()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.UserName, "John Doe");
            p.Add(x => x.ShowUserInfo, false);
        });
        Assert.DoesNotContain("John Doe", cut.Markup);
    }

    [Fact]
    public void Render_WithoutShowChevron_HidesChevron()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.UserName, "John Doe");
            p.Add(x => x.ShowChevron, false);
        });
        Assert.DoesNotContain("chevron-down", cut.Markup);
    }

    [Fact]
    public void Render_WithoutDropdownHeader_HidesHeader()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.UserName, "John Doe");
            p.Add(x => x.ShowDropdownHeader, false);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("xf-profile-menu", cut.Markup);
    }

    [Fact]
    public void Render_ShowsMenuRole()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.UserName, "John");
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("role=\"menu\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsMenuitemRole()
    {
        var items = new[]
        {
            new XFProfileMenuItem { Id = "profile", Label = "My Profile" }
        };

        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.Items, items);
            p.Add(x => x.IsOpen, true);
        });
        Assert.Contains("role=\"menuitem\"", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAriaHaspopup()
    {
        var cut = Render<XFProfileMenu>(p => p.Add(x => x.UserName, "John"));
        Assert.Contains("aria-haspopup=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomChildContent_ShowsContent()
    {
        var cut = Render<XFProfileMenu>(p =>
        {
            p.Add(x => x.UserName, "John");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent("<div>Custom content</div>");
        });
        Assert.Contains("Custom content", cut.Markup);
    }
}
