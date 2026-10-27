#region Using directives
using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Chip;
#endregion

namespace XForge.DesignSystem.Tests.Components;

/// <summary>
/// Tests for event naming (D-001 On{Action} convention).
/// Verifies that OnClick/OnDismiss fire correctly and keyboard events work.
/// </summary>
public class XFEventAliasTest : Bunit.BunitContext
{
    #region XFChip — OnClick

    [Fact]
    public void Chip_OnClick_FiresCallback()
    {
        var clicked = false;
        var cut = Render<XFChip>(p =>
        {
            p.Add(c => c.Clickable, true);
            p.Add(c => c.OnClick, _ => clicked = true);
            p.AddChildContent("Tag");
        });

        cut.Find("span[role='button']").Click();
        Assert.True(clicked);
    }

    #endregion

    #region XFChip — OnDismiss

    [Fact]
    public void Chip_OnDismiss_FiresCallback()
    {
        var dismissed = false;
        var cut = Render<XFChip>(p =>
        {
            p.Add(c => c.Dismissible, true);
            p.Add(c => c.OnDismiss, () => dismissed = true);
            p.AddChildContent("Tag");
        });

        cut.Find("button[aria-label='Remover']").Click();
        Assert.True(dismissed);
    }

    #endregion

    #region Keyboard — OnClick via Enter/Space

    [Fact]
    public void Chip_OnClick_FiresOnEnterKey()
    {
        var clicked = false;
        var cut = Render<XFChip>(p =>
        {
            p.Add(c => c.Clickable, true);
            p.Add(c => c.OnClick, _ => clicked = true);
            p.AddChildContent("Tag");
        });

        cut.Find("span[role='button']").KeyDown(key: "Enter");
        Assert.True(clicked);
    }

    [Fact]
    public void Chip_OnClick_FiresOnSpaceKey()
    {
        var clicked = false;
        var cut = Render<XFChip>(p =>
        {
            p.Add(c => c.Clickable, true);
            p.Add(c => c.OnClick, _ => clicked = true);
            p.AddChildContent("Tag");
        });

        cut.Find("span[role='button']").KeyDown(key: " ");
        Assert.True(clicked);
    }

    #endregion
}
