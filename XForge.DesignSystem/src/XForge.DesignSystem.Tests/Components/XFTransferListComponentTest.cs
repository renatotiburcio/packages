#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using XForge.DesignSystem.Components.Forms.TransferList;
using XForge.DesignSystem.Core.Primitives;
#endregion

namespace XForge.DesignSystem.Tests.Components;

public class XFTransferListComponentTest : XForgeTestContext
{
    public XFTransferListComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static IReadOnlyList<XFTransferListItem> GetSampleItems()
    {
        return new List<XFTransferListItem>
        {
            new XFTransferListItem { Value = "1", Text = "Item 1" },
            new XFTransferListItem { Value = "2", Text = "Item 2" },
            new XFTransferListItem { Value = "3", Text = "Item 3" }
        };
    }

    [Fact]
    public void Render_WithMinimalParameters()
    {
        var cut = Render<XFTransferList>();

        Assert.Contains("xf-transfer-list", cut.Markup);
    }

    [Fact]
    public void Render_WithItems()
    {
        var cut = Render<XFTransferList>(parameters =>
            parameters.Add(p => p.Items, GetSampleItems()));

        Assert.Contains("xf-transfer-list", cut.Markup);
        Assert.Contains("Item 1", cut.Markup);
        Assert.Contains("Item 2", cut.Markup);
        Assert.Contains("Item 3", cut.Markup);
    }

    [Fact]
    public void Render_WithTargetValues()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.TargetValues, new List<string> { "1" });
        });

        Assert.Contains("xf-transfer-list", cut.Markup);
        Assert.Contains("Item 1", cut.Markup);
    }

    [Fact]
    public void Render_WithSourceLabel()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.SourceLabel, "Available");
        });

        Assert.Contains("Available", cut.Markup);
    }

    [Fact]
    public void Render_WithTargetLabel()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.TargetLabel, "Selected");
        });

        Assert.Contains("Selected", cut.Markup);
    }

    [Fact]
    public void Render_WithSearchable()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.Searchable, true);
        });

        Assert.Contains("xf-transfer-list", cut.Markup);
    }

    [Fact]
    public void Render_WithSearchPlaceholder()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.SearchPlaceholder, "Search items");
        });

        Assert.Contains("Search items", cut.Markup);
    }

    [Fact]
    public void Render_WhenDisabled_HasOpacity60()
    {
        var cut = Render<XFTransferList>(parameters =>
        {
            parameters.Add(p => p.Items, GetSampleItems());
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("opacity-60", cut.Markup);
    }
}
