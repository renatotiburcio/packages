using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Services.AI;
using AiStreamChunk = XForge.DesignSystem.Components.AI.Common.AiStreamChunk;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiVisionPanelExtendedTest : BunitContext
{
    private readonly InMemoryAiVisionProvider _provider = new();

    public XFAiVisionPanelExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiVisionProvider>(_provider);
    }

    [Fact]
    public void Extended_Renders_BaseCssClass()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.Contains("xf-ai-vision-panel", cut.Markup);
    }

    [Fact]
    public void Extended_CustomClass_AppendedToBaseClass()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Class, "my-extra"));
        Assert.Contains("my-extra", cut.Markup);
    }

    [Fact]
    public void Extended_Disabled_AnalyzeButtonIsDisabled()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Disabled, true));
        var button = cut.Find("button.px-4");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_Title_RendersCustomTitle()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Title, "Custom Vision Title"));
        Assert.Contains("Custom Vision Title", cut.Markup);
    }

    [Fact]
    public void Extended_ShowHeaderFalse_HidesHeader()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("Análise Visual", cut.Markup);
    }

    [Fact]
    public void Extended_ShowAnalysisTypeSelectorFalse_HidesSelector()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.ShowAnalysisTypeSelector, false));
        var radioGroup = cut.FindAll("[role='radiogroup']");
        Assert.Empty(radioGroup);
    }

    [Fact]
    public void Extended_Placeholder_RendersInTextarea()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.Placeholder, "Type your question..."));
        var textarea = cut.Find("textarea");
        Assert.Equal("Type your question...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void Extended_AriaLabel_RendersOnContainer()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.AriaLabel, "Vision Panel Label"));
        Assert.Contains("Vision Panel Label", cut.Markup);
    }

    [Fact]
    public void Extended_MaxImages_RendersDefaultText()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.Contains("PNG, JPG, WEBP", cut.Markup);
    }

    [Fact]
    public void Extended_HeaderContent_RendersInsideHeader()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.HeaderContent, (RenderFragment)(builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, "Custom Header");
            builder.CloseElement();
        })));
        Assert.Contains("Custom Header", cut.Markup);
    }

    [Fact]
    public void Extended_EmptyState_ShowsDropZoneText()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.Contains("Arraste imagens", cut.Markup);
    }

    [Fact]
    public void Extended_AddImage_ShowsImagePreview()
    {
        var cut = Render<XFAiVisionPanel>();
        cut.InvokeAsync(() => cut.Instance.AddImage("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="));
        var imgs = cut.FindAll("img");
        Assert.True(imgs.Count > 0);
    }

    [Fact]
    public void Extended_Clear_RemovesAllImages()
    {
        var cut = Render<XFAiVisionPanel>();
        cut.InvokeAsync(() => cut.Instance.AddImage("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="));
        cut.InvokeAsync(() => cut.Instance.Clear());
        Assert.Contains("Arraste imagens", cut.Markup);
    }

    [Fact]
    public void Extended_SelectAnalysisType_ChangesSelectedType()
    {
        var cut = Render<XFAiVisionPanel>();
        // Default selected type is "Describe" (Descrever), so OCR starts unchecked
        // Blazor omits aria-checked when value is false, so GetAttribute returns null
        var ocrButton = cut.FindAll("button").First(b => b.TextContent.Contains("OCR"));
        Assert.Null(ocrButton.GetAttribute("aria-checked"));
        // Click OCR to select it
        cut.InvokeAsync(() => ocrButton.Click());
        // SelectAnalysisType sets _selectedType but does NOT call StateHasChanged(),
        // so we verify via reflection that the field was updated
        var field = typeof(XFAiVisionPanel).GetField("_selectedType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var value = (XFAiVisionAnalysisType)field!.GetValue(cut.Instance)!;
        Assert.Equal(XFAiVisionAnalysisType.Ocr, value);
    }

    [Fact]
    public void Extended_RemoveImage_RemovesCorrectImage()
    {
        var cut = Render<XFAiVisionPanel>();
        cut.InvokeAsync(() => cut.Instance.AddImage("aaa"));
        cut.InvokeAsync(() => cut.Instance.AddImage("bbb"));
        var removeButtons = cut.FindAll("button.-top-1\\.5");
        Assert.Equal(2, removeButtons.Count);
        removeButtons[0].Click();
        var remaining = cut.FindAll("button.-top-1\\.5");
        Assert.Single(remaining);
    }

    [Fact]
    public async Task Extended_AnalyzeWithProvider_TriggersOnAnalysisComplete()
    {
        var completed = false;
        XFAiVisionResult? capturedResult = null;
        var cut = Render<XFAiVisionPanel>(p => p
            .Add(x => x.OnAnalysisComplete, EventCallback.Factory.Create<XFAiVisionResult>(this, r =>
            {
                completed = true;
                capturedResult = r;
            })));
        cut.InvokeAsync(() => cut.Instance.AddImage("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="));
        var textarea = cut.Find("textarea");
        textarea.Input("Describe this image");
        var analyzeButton = cut.Find("button.px-4");
        await cut.InvokeAsync(() => analyzeButton.Click());
        Assert.True(completed);
        Assert.NotNull(capturedResult);
        Assert.Equal(XFAiVisionAnalysisType.Describe, capturedResult.AnalysisType);
        Assert.Equal(1, capturedResult.ImageCount);
    }

    [Fact]
    public void Extended_AnalyzeError_SetsErrorState()
    {
        // Use a provider that throws on AnalyzeAsync
        var errorProvider = new ErroringAiVisionProvider();
        var cut = Render<XFAiVisionPanel>();
        // Replace the injected VisionProvider property via reflection (auto-property backing field)
        var providerField = typeof(XFAiVisionPanel).GetField("<VisionProvider>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(providerField);
        providerField!.SetValue(cut.Instance, errorProvider);

        // Set _prompt and add image so the analyze button is enabled
        var promptField = typeof(XFAiVisionPanel).GetField("_prompt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        promptField!.SetValue(cut.Instance, "test prompt");
        cut.InvokeAsync(() => cut.Instance.AddImage("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=="));

        // Find all buttons and locate the analyze button
        var allButtons = cut.FindAll("button");
        var analyzeButton = allButtons.FirstOrDefault(b => b.TextContent.Contains("Analisar"));
        Assert.NotNull(analyzeButton);
        cut.InvokeAsync(() => analyzeButton!.Click());

        // Verify error was set via reflection
        var errorField = typeof(XFAiVisionPanel).GetField("_error", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var errorValue = errorField!.GetValue(cut.Instance) as string;
        Assert.NotNull(errorValue);
        Assert.Contains("Erro na análise", errorValue);
    }

    [Fact]
    public void Extended_Dispose_DoesNotThrow()
    {
        var cut = Render<XFAiVisionPanel>();
        var exception = Record.Exception(() => cut.Instance.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Extended_DefaultAriaLabel_IsCorrect()
    {
        var cut = Render<XFAiVisionPanel>();
        Assert.Contains("Painel de análise visual AI", cut.Markup);
    }

    [Fact]
    public void Extended_RegionRole_IsPresent()
    {
        var cut = Render<XFAiVisionPanel>();
        var region = cut.Find("[role='region']");
        Assert.NotNull(region);
    }

    [Fact]
    public void Extended_AnalysisTypeSelector_HasRadioGroupRole()
    {
        var cut = Render<XFAiVisionPanel>();
        var radioGroup = cut.Find("[role='radiogroup']");
        Assert.NotNull(radioGroup);
    }

    [Fact]
    public void Extended_AnalyzeWithoutImages_ButtonIsDisabled()
    {
        var cut = Render<XFAiVisionPanel>();
        var analyzeButton = cut.Find("button.px-4");
        Assert.True(analyzeButton.HasAttribute("disabled"));
    }

    [Fact]
    public void Extended_AnalyzeText_CustomText_RendersOnButton()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.AnalyzeText, "Run Analysis"));
        Assert.Contains("Run Analysis", cut.Markup);
    }

    [Fact]
    public void Extended_DropText_CustomText_RendersInDropZone()
    {
        var cut = Render<XFAiVisionPanel>(p => p.Add(x => x.DropText, "Drop files here"));
        Assert.Contains("Drop files here", cut.Markup);
    }
}

/// <summary>
/// Test double that always throws, used to verify error handling in XFAiVisionPanel.
/// </summary>
internal sealed class ErroringAiVisionProvider : IAiVisionProvider
{
    public string ProviderId => "error-vision";
    public string DisplayName => "Error Vision (Test)";
    public bool SupportsStreaming => false;
    public int MaxImagesPerRequest => 5;
    public long MaxImageSizeBytes => 10 * 1024 * 1024;

    public Task<AiVisionResponse> AnalyzeAsync(AiVisionRequest request, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("Simulated vision provider failure");

    public async IAsyncEnumerable<AiStreamChunk> AnalyzeStreamAsync(AiVisionRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Simulated streaming failure");
        yield break;
    }

    public Task<AiOcrResponse> ExtractTextAsync(AiOcrRequest request, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException("Simulated OCR failure");
}
