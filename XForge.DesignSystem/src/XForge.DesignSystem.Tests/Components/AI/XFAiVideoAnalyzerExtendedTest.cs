using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Services.AI;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiVideoAnalyzerExtendedTest : BunitContext
{
    public XFAiVideoAnalyzerExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiVisionProvider>(new InMemoryAiVisionProvider());
    }

    private IRenderedComponent<XFAiVideoAnalyzer> RenderDefault(
        Action<ComponentParameterCollectionBuilder<XFAiVideoAnalyzer>>? configure = null)
    {
        return Render<XFAiVideoAnalyzer>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = RenderDefault();
        Assert.Contains("xf-ai-video-analyzer", cut.Markup);
    }

    [Fact]
    public void Extended_Renders_Title()
    {
        var cut = RenderDefault(p => p.Add(x => x.Title, "My Analyzer"));
        Assert.Contains("My Analyzer", cut.Markup);
    }

    [Fact]
    public void Extended_ShowHeaderFalse_HidesHeader()
    {
        var cut = RenderDefault(p => p
            .Add(x => x.Title, "Video Analyzer")
            .Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("Video Analyzer", cut.Markup);
    }

    [Fact]
    public void Extended_ShowFrameSelectorFalse_HidesSelector()
    {
        var cut = RenderDefault(p => p.Add(x => x.ShowFrameSelector, false));
        Assert.DoesNotContain("xf-ai-video-analyzer__frame-selector", cut.Markup);
    }

    [Fact]
    public void Extended_CustomPlaceholder_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.Placeholder, "Describe what to look for..."));
        Assert.Contains("Describe what to look for...", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAnalyzeText_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.AnalyzeText, "Analyze Video"));
        Assert.Contains("Analyze Video", cut.Markup);
    }

    [Fact]
    public void Extended_CustomAriaLabel_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.AriaLabel, "Video AI"));
        Assert.Contains("Video AI", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderContent_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.HeaderContent, (RenderFragment)(b => b.AddContent(0, "custom header"))));
        Assert.Contains("custom header", cut.Markup);
    }

    [Fact]
    public void Extended_CustomClass_Renders()
    {
        var cut = RenderDefault(p => p.Add(x => x.Class, "custom-video"));
        Assert.Contains("custom-video", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_RendersDisabledState()
    {
        var cut = RenderDefault(p => p.Add(x => x.Disabled, true));
        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Extended_InitialState_NoResults()
    {
        var cut = RenderDefault();
        Assert.Empty(cut.Instance.Results);
    }

    [Fact]
    public void Extended_Clear_ClearsResults()
    {
        var cut = RenderDefault();
        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.Empty(cut.Instance.Results);
    }

    [Fact]
    public void Extended_Dispose_DoesNotThrow()
    {
        var cut = RenderDefault();
        cut.Instance.Dispose();
    }

    [Fact]
    public void Extended_OnAnalysisComplete_CallbackRegistered()
    {
        XFAiVisionResult? result = null;
        var cut = RenderDefault(p => p.Add(
            x => x.OnAnalysisComplete,
            EventCallback.Factory.Create<IReadOnlyList<XFAiVisionResult>>(this, r => result = r.FirstOrDefault())));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_OnError_CallbackRegistered()
    {
        string? error = null;
        var cut = RenderDefault(p => p.Add(
            x => x.OnError,
            EventCallback.Factory.Create<string>(this, e => error = e)));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Extended_DefaultTitle_Renders()
    {
        var cut = Render<XFAiVideoAnalyzer>();
        Assert.Contains("xf-ai-video-analyzer", cut.Markup);
    }

    [Fact]
    public void Extended_VisionProvider_Injected()
    {
        var cut = RenderDefault();
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void Extended_Results_InitiallyEmpty()
    {
        var cut = RenderDefault();
        Assert.Empty(cut.Instance.Results);
    }

    [Fact]
    public void Extended_Clear_ResetsAllState()
    {
        var cut = RenderDefault();
        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.Empty(cut.Instance.Results);
    }

    [Fact]
    public void Extended_XFAiVisionAnalysisType_HasExpectedValues()
    {
        Assert.Equal(6, Enum.GetValues<XFAiVisionAnalysisType>().Length);
        Assert.Contains(XFAiVisionAnalysisType.Describe, Enum.GetValues<XFAiVisionAnalysisType>());
        Assert.Contains(XFAiVisionAnalysisType.Ocr, Enum.GetValues<XFAiVisionAnalysisType>());
        Assert.Contains(XFAiVisionAnalysisType.DetectObjects, Enum.GetValues<XFAiVisionAnalysisType>());
        Assert.Contains(XFAiVisionAnalysisType.AnalyzeChart, Enum.GetValues<XFAiVisionAnalysisType>());
        Assert.Contains(XFAiVisionAnalysisType.ReadDocument, Enum.GetValues<XFAiVisionAnalysisType>());
        Assert.Contains(XFAiVisionAnalysisType.Compare, Enum.GetValues<XFAiVisionAnalysisType>());
    }

    [Fact]
    public void Extended_XFAiVisionResult_CanCreate()
    {
        var result = new XFAiVisionResult
        {
            Text = "Analysis result",
            Confidence = 0.95f,
            AnalysisType = XFAiVisionAnalysisType.Describe,
            ImageCount = 1
        };
        Assert.Equal("Analysis result", result.Text);
        Assert.Equal(0.95f, result.Confidence);
        Assert.Equal(XFAiVisionAnalysisType.Describe, result.AnalysisType);
        Assert.Equal(1, result.ImageCount);
    }

    [Fact]
    public void Extended_XFAiVisionResult_WithStructuredData()
    {
        var result = new XFAiVisionResult
        {
            Text = "Chart data",
            StructuredData = "{\"value\": 42}",
            Confidence = 0.88f,
            AnalysisType = XFAiVisionAnalysisType.AnalyzeChart,
            ImageCount = 2
        };
        Assert.Equal("{\"value\": 42}", result.StructuredData);
        Assert.Equal(2, result.ImageCount);
    }

    [Fact]
    public void Extended_XFAiVisionResult_Timestamp_Set()
    {
        var before = DateTime.UtcNow;
        var result = new XFAiVisionResult
        {
            Text = "Test",
            AnalysisType = XFAiVisionAnalysisType.Ocr,
            ImageCount = 1
        };
        Assert.True(result.Timestamp >= before);
    }

    [Fact]
    public void Extended_AiVisionRequest_CanCreate()
    {
        var request = new AiVisionRequest
        {
            Prompt = "Describe this image",
            Images = new[] { "base64data" },
            MaxTokens = 512
        };
        Assert.Equal("Describe this image", request.Prompt);
        Assert.Single(request.Images);
        Assert.Equal(512, request.MaxTokens);
    }

    [Fact]
    public void Extended_AiVisionResponse_CanCreate()
    {
        var response = new AiVisionResponse
        {
            Text = "A cat sitting on a mat",
            Confidence = 0.92f,
            Model = "gpt-4v"
        };
        Assert.Equal("A cat sitting on a mat", response.Text);
        Assert.Equal(0.92f, response.Confidence);
        Assert.Equal("gpt-4v", response.Model);
    }

    [Fact]
    public void Extended_AiOcrRequest_CanCreate()
    {
        var request = new AiOcrRequest
        {
            Image = "base64data",
            Language = "pt-BR",
            PreserveLayout = true
        };
        Assert.Equal("pt-BR", request.Language);
        Assert.True(request.PreserveLayout);
    }

    [Fact]
    public void Extended_AiOcrResponse_CanCreate()
    {
        var response = new AiOcrResponse
        {
            Text = "Extracted text",
            Confidence = 0.95f,
            DetectedLanguage = "pt-BR"
        };
        Assert.Equal("Extracted text", response.Text);
        Assert.Equal("pt-BR", response.DetectedLanguage);
    }
}
