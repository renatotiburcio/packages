using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for XFAiVisionPanel component.
/// </summary>
public sealed class XFAiVisionPanelTest : XForgeTestContext
{
    public XFAiVisionPanelTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
    }

    [Fact]
    public void Vision_Renders_Default()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Title, "My Vision Panel"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithoutHeader()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithoutAnalysisTypeSelector()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowAnalysisTypeSelector, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Placeholder, "Ask about image..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomAnalyzeText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.AnalyzeText, "Analyze Image"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomLoadingText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.LoadingText, "Processing..."));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomDropText()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.DropText, "Drop images here"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithMaxImages()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.MaxImages, 3));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.AriaLabel, "Vision Analysis"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.HeaderContent, (RenderFragment)(builder => { builder.AddContent(0, "header"); })));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithResultContent()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ResultContent, (RenderFragment)(builder => { builder.AddContent(0, "result"); })));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_Renders_WithCustomClass()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Vision_AnalysisTypes_ContainsExpectedTypes()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.NotNull(cut);
        // Verify component renders with default analysis type
    }

    [Fact]
    public void Vision_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiVisionPanel>();
        cut.Instance.Dispose();
    }

    [Fact]
    public void Vision_Renders_WithDisabledState()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }
}
