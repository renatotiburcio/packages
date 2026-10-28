using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Overlays.Drawer;

namespace XForge.DesignSystem.Tests.Components;

public class XFDrawerAriaTest : XForgeTestContext
{
    public XFDrawerAriaTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Role_IsDialog()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
        });

        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void AriaModal_WhenModal_IsTrue()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Modal, true);
        });

        Assert.Contains("aria-modal=\"true\"", cut.Markup);
    }

    [Fact]
    public void AriaModal_WhenNotModal_IsFalse()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Modal, false);
        });

        Assert.Contains("aria-modal=\"false\"", cut.Markup);
    }

    [Fact]
    public void AriaLabelledBy_PointsToTitle()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Title, "Filters");
        });

        var drawer = cut.Find("aside");
        var labelledBy = drawer.GetAttribute("aria-labelledby");
        Assert.NotNull(labelledBy);
        Assert.Contains(labelledBy, cut.Markup);
        Assert.Contains("Filters", cut.Markup);
    }

    [Fact]
    public void AriaDescribedBy_PointsToDescription()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.Title, "Filters");
            parameters.Add(p => p.Description, "Refine results");
        });

        var drawer = cut.Find("aside");
        var describedBy = drawer.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Contains(describedBy, cut.Markup);
        Assert.Contains("Refine results", cut.Markup);
    }

    [Fact]
    public void AriaReferences_WhenTitleAndDescriptionMissing_AreOmitted()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.ShowCloseButton, false);
        });

        var drawer = cut.Find("aside");
        Assert.Null(drawer.GetAttribute("aria-labelledby"));
        Assert.Null(drawer.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void AriaLabel_WhenNoTitle_NamesDrawer()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.AriaLabel, "Painel de filtros");
            parameters.Add(p => p.ShowCloseButton, false);
        });

        var drawer = cut.Find("aside");
        Assert.Equal("Painel de filtros", drawer.GetAttribute("aria-label"));
        Assert.Null(drawer.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void AriaLabelledBy_WhenCustomReference_SuppressesAriaLabel()
    {
        var cut = Render<XFDrawer>(parameters =>
        {
            parameters.Add(p => p.Open, true);
            parameters.Add(p => p.AriaLabel, "Painel");
            parameters.Add(p => p.AriaLabelledBy, "drawer-heading");
            parameters.Add(p => p.Header, builder =>
            {
                builder.OpenElement(0, "h2");
                builder.AddAttribute(1, "id", "drawer-heading");
                builder.AddContent(2, "Detalhes");
                builder.CloseElement();
            });
        });

        var drawer = cut.Find("aside");
        Assert.Null(drawer.GetAttribute("aria-label"));
        Assert.Equal("drawer-heading", drawer.GetAttribute("aria-labelledby"));
    }
}
