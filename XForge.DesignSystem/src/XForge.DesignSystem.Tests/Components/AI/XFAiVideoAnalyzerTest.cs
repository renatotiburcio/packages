using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiVideoAnalyzerTest : XForgeTestContext
{
    public XFAiVideoAnalyzerTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
    }

    [Fact] public void Video_Renders_Default()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Title, "My Video Analyzer"));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithoutHeader()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithoutFrameSelector()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.ShowFrameSelector, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithCustomPlaceholder()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Placeholder, "Describe what to look for..."));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithCustomAnalyzeText()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.AnalyzeText, "Analyze Video"));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithCustomAriaLabel()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.AriaLabel, "Video AI"));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithHeaderContent()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "header"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithCustomClass()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_Renders_WithDisabledState()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Video_InitialState_NoResults()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        Assert.Empty(cut.Instance.Results);
    }

    [Fact] public void Video_Clear_DoesNotThrow()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    [Fact] public void Video_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        cut.Instance.Dispose();
    }
}
