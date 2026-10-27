using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.AI.Utility;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

internal sealed class ValidationMockProvider : IAiProvider, IDisposable
{
    public string ProviderId => "mock-validation";
    public string DisplayName => "Mock Validation";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;
    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiChatResponse { Message = new AiMessage { Role = AiRole.Assistant, Content = "[]" } });
    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default) => AsyncEnumerable.Empty<AiStreamChunk>();
    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default) => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });
    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default) => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    public void Dispose() { }
}

public sealed class XFAiValidationSummaryTest : BunitContext
{
    public XFAiValidationSummaryTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void Validation_Renders_Default()
    {
        var cut = Render<XFAiValidationSummary>();
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithItems()
    {
        var items = new List<XFAiValidationItem> { new() { FieldName = "Name", Message = "Required" } };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, (IReadOnlyList<XFAiValidationItem>)items));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithEmptyItems()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, Array.Empty<XFAiValidationItem>()));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithProvider()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Provider, new ValidationMockProvider()));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithCustomTitle()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Title, "Validation Errors"));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithShowHeaderFalse()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithShowDismissFalse()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.ShowDismiss, false));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithCustomEmptyText()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.EmptyText, "No issues"));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithLoading()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Loading, true));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithCustomSystemPrompt()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.SystemPrompt, "Custom prompt"));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithCustomClass()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Class, "custom-class"));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithEmptyContent()
    {
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithSeverityError()
    {
        var items = new List<XFAiValidationItem> { new() { FieldName = "Name", Message = "Required", Severity = XFAiValidationSeverity.Error } };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, (IReadOnlyList<XFAiValidationItem>)items));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithSeverityWarning()
    {
        var items = new List<XFAiValidationItem> { new() { FieldName = "Email", Message = "Invalid format", Severity = XFAiValidationSeverity.Warning } };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, (IReadOnlyList<XFAiValidationItem>)items));
        Assert.NotNull(cut);
    }

    [Fact] public void Validation_Renders_WithSeverityInfo()
    {
        var items = new List<XFAiValidationItem> { new() { FieldName = "Phone", Message = "Optional field", Severity = XFAiValidationSeverity.Info } };
        var cut = Render<XFAiValidationSummary>(p => p.Add(x => x.Items, (IReadOnlyList<XFAiValidationItem>)items));
        Assert.NotNull(cut);
    }
}
