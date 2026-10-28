using Bunit;
using Xunit;
using XForge.DesignSystem.Components.Navigation.Menu;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Navigation;

public sealed class XFMenuFunctionalTest : XForgeTestContext
{
    public XFMenuFunctionalTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    // Enums
    [Fact] public void MenuVariant_Has7Values() => Assert.Equal(7, Enum.GetValues<XFMenuVariant>().Length);
    [Fact] public void MenuDisplayMode_Has3Values() => Assert.Equal(3, Enum.GetValues<XFMenuDisplayMode>().Length);
    [Fact] public void MenuItemKind_Has3Values() => Assert.Equal(3, Enum.GetValues<XFMenuItemKind>().Length);

    // XFMenuItem
    [Fact] public void MenuItem_DefaultId() => Assert.NotEmpty(new XFMenuItem().Id);
    [Fact] public void MenuItem_DefaultTextNull() => Assert.Null(new XFMenuItem().Text);
    [Fact] public void MenuItem_DefaultKindItem() => Assert.Equal(XFMenuItemKind.Item, new XFMenuItem().Kind);
    [Fact] public void MenuItem_DefaultDisabled() => Assert.False(new XFMenuItem().Disabled);
    [Fact] public void MenuItem_DefaultSelected() => Assert.False(new XFMenuItem().Selected);
    [Fact] public void MenuItem_DefaultExpanded() => Assert.False(new XFMenuItem().Expanded);
    [Fact] public void MenuItem_DefaultChildrenEmpty() => Assert.Empty(new XFMenuItem().Children);
    [Fact] public void MenuItem_SetText() => Assert.Equal("Home", new XFMenuItem { Text = "Home" }.Text);
    [Fact] public void MenuItem_SetHref() => Assert.Equal("/home", new XFMenuItem { Href = "/home" }.Href);
    [Fact] public void MenuItem_SetBadge() => Assert.Equal("3", new XFMenuItem { Badge = "3" }.Badge);
    [Fact] public void MenuItem_SetDisabled() => Assert.True(new XFMenuItem { Disabled = true }.Disabled);
    [Fact] public void MenuItem_SetSelected() => Assert.True(new XFMenuItem { Selected = true }.Selected);
    [Fact] public void MenuItem_SetExpanded() => Assert.True(new XFMenuItem { Expanded = true }.Expanded);
    [Fact] public void MenuItem_SectionKind() => Assert.Equal(XFMenuItemKind.Section, new XFMenuItem { Kind = XFMenuItemKind.Section }.Kind);
    [Fact] public void MenuItem_SeparatorKind() => Assert.Equal(XFMenuItemKind.Separator, new XFMenuItem { Kind = XFMenuItemKind.Separator }.Kind);
    [Fact] public void MenuItem_WithChildren() { var item = new XFMenuItem { Text = "P", Children = new[] { new XFMenuItem { Text = "C" } } }; Assert.Single(item.Children); }
    [Fact] public void MenuItem_WithDescription() => Assert.Equal("Desc", new XFMenuItem { Description = "Desc" }.Description);
    [Fact] public void MenuItem_IdsAreUnique() => Assert.NotEqual(new XFMenuItem().Id, new XFMenuItem().Id);

    // Render
    [Fact] public void Renders_Default() => Render<XFMenu>();
    [Fact] public void Renders_WithItems()
    {
        var items = new[] { new XFMenuItem { Text = "Home", Href = "/" }, new XFMenuItem { Text = "About", Href = "/about" } };
        Render<XFMenu>(p => p.Add(x => x.Items, items));
    }
    [Fact] public void Renders_VariantSurface() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.Surface));
    [Fact] public void Renders_VariantPlain() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.Plain));
    [Fact] public void Renders_VariantSoft() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.Soft));
    [Fact] public void Renders_VariantTopNav() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.TopNav));
    [Fact] public void Renders_VariantDropdown() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.Dropdown));
    [Fact] public void Renders_VariantSidebar() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.Sidebar));
    [Fact] public void Renders_VariantDarkSidebar() => Render<XFMenu>(p => p.Add(x => x.Variant, XFMenuVariant.DarkSidebar));
    [Fact] public void Renders_DisplayModeIconAndText() => Render<XFMenu>(p => p.Add(x => x.DisplayMode, XFMenuDisplayMode.IconAndText));
    [Fact] public void Renders_DisplayModeIconOnly() => Render<XFMenu>(p => p.Add(x => x.DisplayMode, XFMenuDisplayMode.IconOnly));
    [Fact] public void Renders_DisplayModeTextOnly() => Render<XFMenu>(p => p.Add(x => x.DisplayMode, XFMenuDisplayMode.TextOnly));
    [Fact] public void Renders_Disabled() => Render<XFMenu>(p => p.Add(x => x.Disabled, true));
    [Fact] public void Renders_WithSectionItems()
    {
        var items = new XFMenuItem[]
        {
            new() { Kind = XFMenuItemKind.Section, Text = "Nav" },
            new() { Text = "Home", Href = "/" },
            new() { Kind = XFMenuItemKind.Separator },
            new() { Text = "Exit", Href = "/exit" }
        };
        Render<XFMenu>(p => p.Add(x => x.Items, items));
    }
    [Fact] public void Renders_WithNestedMenu()
    {
        var items = new XFMenuItem[]
        {
            new() { Text = "Admin", Expanded = true, Children = new[] { new XFMenuItem { Text = "Users" }, new XFMenuItem { Text = "Roles" } } }
        };
        Render<XFMenu>(p => p.Add(x => x.Items, items));
    }
}
