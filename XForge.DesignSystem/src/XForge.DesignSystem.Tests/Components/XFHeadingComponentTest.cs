using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Typography;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components;

public class XFHeadingComponentTest : XForgeTestContext
{
    public XFHeadingComponentTest()
    {
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_Default_RendersH1()
    {
        var cut = Render<XFHeading>(parameters =>
            parameters.Add(p => p.Text, "Title"));

        Assert.Contains("<h1", cut.Markup);
        Assert.Contains("Title", cut.Markup);
    }

    [Fact]
    public void Render_Level1_RendersH1()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 1);
            parameters.Add(p => p.Text, "Heading 1");
        });

        Assert.Contains("<h1", cut.Markup);
        Assert.Contains("Heading 1", cut.Markup);
    }

    [Fact]
    public void Render_Level2_RendersH2()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 2);
            parameters.Add(p => p.Text, "Heading 2");
        });

        Assert.Contains("<h2", cut.Markup);
        Assert.Contains("Heading 2", cut.Markup);
    }

    [Fact]
    public void Render_Level3_RendersH3()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 3);
            parameters.Add(p => p.Text, "Heading 3");
        });

        Assert.Contains("<h3", cut.Markup);
        Assert.Contains("Heading 3", cut.Markup);
    }

    [Fact]
    public void Render_Level4_RendersH4()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 4);
            parameters.Add(p => p.Text, "Heading 4");
        });

        Assert.Contains("<h4", cut.Markup);
    }

    [Fact]
    public void Render_Level5_RendersH5()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 5);
            parameters.Add(p => p.Text, "Heading 5");
        });

        Assert.Contains("<h5", cut.Markup);
    }

    [Fact]
    public void Render_Level6_RendersH6()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 6);
            parameters.Add(p => p.Text, "Heading 6");
        });

        Assert.Contains("<h6", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_RendersContent()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 2);
            parameters.Add(p => p.ChildContent, "Child heading");
        });

        Assert.Contains("Child heading", cut.Markup);
    }

    [Fact]
    public void Render_WithChildContent_PrecedesText()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 1);
            parameters.Add(p => p.Text, "Ignored");
            parameters.Add(p => p.ChildContent, "Visible");
        });

        Assert.Contains("Visible", cut.Markup);
    }

    [Fact]
    public void Render_WithAriaLabel_SetsAttribute()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Text, "Section");
            parameters.Add(p => p.AriaLabel, "Main section heading");
        });

        Assert.Contains("aria-label=\"Main section heading\"", cut.Markup);
    }

    [Fact]
    public void Render_LevelOutOfRange_ClampsToH1()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 0);
            parameters.Add(p => p.Text, "Clamped");
        });

        Assert.Contains("<h1", cut.Markup);
    }

    [Fact]
    public void Render_LevelAbove6_ClampsToH6()
    {
        var cut = Render<XFHeading>(parameters =>
        {
            parameters.Add(p => p.Level, 10);
            parameters.Add(p => p.Text, "Clamped");
        });

        Assert.Contains("<h6", cut.Markup);
    }
}
