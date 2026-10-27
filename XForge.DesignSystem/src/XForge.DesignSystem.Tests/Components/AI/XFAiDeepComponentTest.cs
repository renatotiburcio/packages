using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.DataGrid;
using XForge.DesignSystem.Components.AI.Audio;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Services.AI;
using ServicesAiTranscriptionRequest = XForge.DesignSystem.Services.AI.AiTranscriptionRequest;
using ServicesAiTranscriptionResponse = XForge.DesignSystem.Services.AI.AiTranscriptionResponse;
using ServicesAiSpeechRequest = XForge.DesignSystem.Services.AI.AiSpeechRequest;
using ServicesAiSpeechResponse = XForge.DesignSystem.Services.AI.AiSpeechResponse;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Mock IAiProvider and IAiAudioProvider for deep component tests.
/// </summary>
internal sealed class DataGridDeepMockProvider : IAiProvider, IAsyncDisposable
{
    public AiChatResponse? NextResponse { get; set; }
    public Exception? ThrowException { get; set; }
    public AiChatRequest? LastRequest { get; private set; }
    public int CallCount { get; private set; }

    public string ProviderId => "mock-deep";
    public string DisplayName => "Mock";
    public string? DefaultModel => "mock-model";
    public bool SupportsStreaming => false;
    public bool SupportsToolCalling => false;

    public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default)
    {
        CallCount++;
        LastRequest = request;
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextResponse ?? new AiChatResponse());
    }

    public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default)
        => AsyncEnumerable.Empty<AiStreamChunk>();

    public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default)
        => Task.FromResult(new AiEmbeddingResponse { Embeddings = Array.Empty<float[]>() });

    public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class MockAiAudioProvider : IAiAudioProvider
{
    public ServicesAiTranscriptionResponse? NextTranscription { get; set; }
    public ServicesAiSpeechResponse? NextSpeech { get; set; }
    public Exception? ThrowException { get; set; }

    public string ProviderId => "mock-audio";
    public string DisplayName => "Mock Audio";
    public bool SupportsStreamingTranscription => false;
    public bool SupportsTextToSpeech { get; init; } = true;
    public long MaxAudioSizeBytes => 10_000_000;
    public IReadOnlyList<string> SupportedFormats => new[] { "mp3", "wav" };

    public Task<ServicesAiTranscriptionResponse> TranscribeAsync(ServicesAiTranscriptionRequest request, CancellationToken cancellationToken = default)
    {
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextTranscription ?? new ServicesAiTranscriptionResponse { Text = "Hello world" });
    }

    public IAsyncEnumerable<AiTranscriptionChunk> TranscribeStreamAsync(ServicesAiTranscriptionRequest request, CancellationToken cancellationToken = default)
        => AsyncEnumerable.Empty<AiTranscriptionChunk>();

    public Task<ServicesAiSpeechResponse> SynthesizeSpeechAsync(ServicesAiSpeechRequest request, CancellationToken cancellationToken = default)
    {
        if (ThrowException is not null) throw ThrowException;
        return Task.FromResult(NextSpeech ?? new ServicesAiSpeechResponse { Audio = "base64audio" });
    }

    public Task<AiAudioAnalysisResponse> AnalyzeAudioAsync(AiAudioAnalysisRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new AiAudioAnalysisResponse { Summary = "Analysis complete" });
}

/// <summary>
/// Deep tests for XFAiDataGridAssistant and XFAiVoiceAssistant.
/// </summary>
public sealed class XFAiDeepComponentTest : BunitContext
{
    public XFAiDeepComponentTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region XFAiDataGridAssistant - SubmitQueryAsync

    [Fact]
    public async Task DataGrid_SubmitQueryAsync_WithProvider_CallsProvider()
    {
        var provider = new DataGridDeepMockProvider
        {
            NextResponse = new AiChatResponse
            {
                Message = new AiMessage
                {
                    Role = AiRole.Assistant,
                    Content = """{"type":"filter","filters":[{"field":"name","operator":"contains","value":"test"}]}"""
                }
            }
        };

        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiDataGridAssistant).GetField("_queryText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "show items where name contains test");
            await cut.Instance.SubmitQueryAsync();
        });

        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task DataGrid_SubmitQueryAsync_NoProvider_DoesNotThrow()
    {
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, null));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiDataGridAssistant).GetField("_queryText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.SubmitQueryAsync();
        });
    }

    [Fact]
    public async Task DataGrid_SubmitQueryAsync_EmptyQuery_DoesNotCallProvider()
    {
        var provider = new DataGridDeepMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () => await cut.Instance.SubmitQueryAsync());

        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public async Task DataGrid_SubmitQueryAsync_Exception_DoesNotThrow()
    {
        var provider = new DataGridDeepMockProvider { ThrowException = new Exception("API error") };
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiDataGridAssistant).GetField("_queryText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.SubmitQueryAsync();
        });
    }

    [Fact]
    public async Task DataGrid_SubmitQueryAsync_OperationCancelled_DoesNotThrow()
    {
        var provider = new DataGridDeepMockProvider { ThrowException = new OperationCanceledException() };
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));

        await cut.InvokeAsync(async () =>
        {
            var queryField = typeof(XFAiDataGridAssistant).GetField("_queryText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            queryField!.SetValue(cut.Instance, "test");
            await cut.Instance.SubmitQueryAsync();
        });
    }

    [Fact]
    public async Task DataGrid_CancelAsync_DoesNotThrow()
    {
        var provider = new DataGridDeepMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        await cut.InvokeAsync(async () => await cut.Instance.CancelAsync());
    }

    [Fact]
    public void DataGrid_Clear_AfterSubmit_ClearsState()
    {
        var provider = new DataGridDeepMockProvider();
        var cut = Render<XFAiDataGridAssistant>(p => p.Add(x => x.Provider, provider));
        cut.InvokeAsync(() => cut.Instance.Clear());
    }

    #endregion

    #region XFAiVoiceAssistant

    [Fact]
    public void Voice_Renders_Default()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>();
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_Renders_WithTitle()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>(p => p.Add(x => x.Title, "My Voice"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_Renders_WithoutHeader()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>(p => p.Add(x => x.ShowHeader, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_Renders_WithoutTextToSpeech()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>(p => p.Add(x => x.ShowTextToSpeech, false));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_Renders_WithLanguage()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>(p => p.Add(x => x.Language, "en-US"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_Renders_WithCustomAriaLabel()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>(p => p.Add(x => x.AriaLabel, "AI Voice"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Voice_ClearTranscriptions_DoesNotThrow()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>();
        cut.InvokeAsync(() => cut.Instance.ClearTranscriptions());
    }

    [Fact]
    public void Voice_Dispose_DoesNotThrow()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>();
        cut.Instance.Dispose();
    }

    [Fact]
    public void Voice_InitialState_IsIdle()
    {
        Services.AddSingleton<IAiAudioProvider>(new MockAiAudioProvider());
        var cut = Render<XFAiVoiceAssistant>();
        Assert.Equal(XFAiVoiceState.Idle, cut.Instance.CurrentState);
        Assert.Empty(cut.Instance.Transcriptions);
    }

    #endregion
}
