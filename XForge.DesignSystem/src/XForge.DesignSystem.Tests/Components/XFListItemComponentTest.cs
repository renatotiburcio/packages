#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.List;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFListItemComponentTest : XForgeTestContext
{
    public XFListItemComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFListItem>();
        Assert.Contains("xf-list-item", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle()
    {
        var cut = Render<XFListItem>(parameters =>
        {
            parameters.Add(p => p.Title, "Inbox");
        });
        Assert.Contains("Inbox", cut.Markup);
    }

    [Fact]
    public void Render_WithDescription()
    {
        var cut = Render<XFListItem>(parameters =>
        {
            parameters.Add(p => p.Title, "Task");
            parameters.Add(p => p.Description, "Pending approval");
        });
        Assert.Contains("Pending approval", cut.Markup);
    }

    [Fact]
    public void Render_SelectedState()
    {
        var cut = Render<XFListItem>(parameters =>
        {
            parameters.Add(p => p.Selected, true);
        });
        Assert.Contains("xf-list-item", cut.Markup);
        Assert.Contains("aria-selected=\"true\"", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent()
    {
        var cut = Render<XFListItem>(parameters =>
        {
            parameters.AddChildContent("Custom content");
        });
        Assert.Contains("Custom content", cut.Markup);
    }
}
