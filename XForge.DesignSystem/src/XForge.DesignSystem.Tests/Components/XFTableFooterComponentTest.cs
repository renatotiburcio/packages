using Bunit;
using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Table;

namespace XForge.DesignSystem.Tests.Components;

public class XFTableFooterComponentTest : BunitContext
{
    [Fact]
    public void Render_Default_ShowsFooterElement()
    {
        var cut = Render<XFTableFooter>(parameters =>
        {
            parameters.AddChildContent("Footer content");
        });

        Assert.Contains("<tfoot", cut.Markup);
        Assert.Contains("Footer content", cut.Markup);
        Assert.Contains("bg-surface-50", cut.Markup);
        Assert.Contains("font-semibold", cut.Markup);
    }

    [Fact]
    public void Render_Visible_False_HidesFooter()
    {
        var cut = Render<XFTableFooter>(parameters =>
        {
            parameters.Add(p => p.Visible, false);
            parameters.AddChildContent("Hidden footer");
        });

        Assert.DoesNotContain("Hidden footer", cut.Markup);
        Assert.DoesNotContain("<tfoot", cut.Markup);
    }

    [Fact]
    public void Render_Visible_True_ShowsFooter()
    {
        var cut = Render<XFTableFooter>(parameters =>
        {
            parameters.Add(p => p.Visible, true);
            parameters.AddChildContent("Visible footer");
        });

        Assert.Contains("Visible footer", cut.Markup);
        Assert.Contains("<tfoot", cut.Markup);
    }

    [Fact]
    public void Render_WithAdditionalAttributes_PassesThrough()
    {
        var cut = Render<XFTableFooter>(parameters =>
        {
            parameters.Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-test", "footer-value" }
            });
            parameters.AddChildContent("Content");
        });

        Assert.Contains("data-test=\"footer-value\"", cut.Markup);
    }

    [Fact]
    public void Render_WithTableRows_RendersCorrectly()
    {
        var cut = Render<XFTableFooter>(parameters =>
        {
            parameters.AddChildContent((Microsoft.AspNetCore.Components.RenderFragment)(builder =>
            {
                builder.OpenElement(0, "tr");
                builder.OpenElement(1, "td");
                builder.AddContent(2, "Total");
                builder.CloseElement();
                builder.OpenElement(3, "td");
                builder.AddContent(4, "$1,000");
                builder.CloseElement();
                builder.CloseElement();
            }));
        });

        Assert.Contains("<tfoot", cut.Markup);
        Assert.Contains("Total", cut.Markup);
        Assert.Contains("$1,000", cut.Markup);
    }
}
