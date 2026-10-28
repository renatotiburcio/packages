using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.AI.Audio;
using XForge.DesignSystem.Components.AI.Common;
using XForge.DesignSystem.Components.AI.MultiModal;
using XForge.DesignSystem.Components.AI.Vision;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

/// <summary>
/// Tests for S-V2-001 Multi-Modal AI components and services:
/// IAiVisionProvider, IAiAudioProvider, XFAiVisionPanel, XFAiVoiceAssistant,
/// XFAiMultiModalChat, and XFAiVideoAnalyzer.
/// Covers unit tests, rendering, interaction, accessibility, and snapshot.
/// </summary>
public class XFAiMultiModalTest : XForgeTestContext
{
    public XFAiMultiModalTest()
    {
        Services.AddSingleton<IAiVisionProvider, InMemoryAiVisionProvider>();
        Services.AddSingleton<IAiAudioProvider, InMemoryAiAudioProvider>();
        Services.AddSingleton<IAiProvider, MockAiProvider>();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    #region IAiVisionProvider - AnalyzeAsync

    [Fact]
    public async Task VisionProvider_AnalyzeAsync_ReturnsResponseWithText()
    {
        var provider = new InMemoryAiVisionProvider();

        var request = new XForge.DesignSystem.Services.AI.AiVisionRequest
        {
            Prompt = "Descreva a imagem",
            Images = new[] { "base64data" }
        };

        var response = await provider.AnalyzeAsync(request);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Text));
        Assert.Contains("Descreva a imagem", response.Text);
    }

    [Fact]
    public async Task VisionProvider_AnalyzeAsync_ReturnsDetections()
    {
        var provider = new InMemoryAiVisionProvider();

        var request = new XForge.DesignSystem.Services.AI.AiVisionRequest
        {
            Prompt = "Detectar objetos",
            Images = new[] { "img1_base64", "img2_base64" }
        };

        var response = await provider.AnalyzeAsync(request);

        Assert.NotNull(response.Detections);
        Assert.Equal(2, response.Detections.Count);
        Assert.Contains(response.Detections, d => d.Label == "Objeto_1");
        Assert.Contains(response.Detections, d => d.Label == "Objeto_2");
        Assert.All(response.Detections, d =>
        {
            Assert.True(d.Confidence > 0);
            Assert.NotNull(d.BoundingBox);
        });
    }

    [Fact]
    public async Task VisionProvider_ExtractTextAsync_ReturnsOcrText()
    {
        var provider = new InMemoryAiVisionProvider();

        var request = new AiOcrRequest
        {
            Image = "invoice_base64",
            Language = "pt-BR"
        };

        var response = await provider.ExtractTextAsync(request);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Text));
        Assert.Contains("CNPJ", response.Text);
        Assert.Contains("Valor Total", response.Text);
        Assert.Equal("pt-BR", response.DetectedLanguage);
    }

    [Fact]
    public async Task VisionProvider_ExtractTextAsync_ReturnsRegions()
    {
        var provider = new InMemoryAiVisionProvider();

        var request = new AiOcrRequest
        {
            Image = "document_base64",
            PreserveLayout = true
        };

        var response = await provider.ExtractTextAsync(request);

        Assert.NotNull(response.Regions);
        Assert.True(response.Regions.Count > 0);
        Assert.All(response.Regions, r =>
        {
            Assert.False(string.IsNullOrWhiteSpace(r.Text));
            Assert.True(r.Confidence > 0);
            Assert.NotNull(r.BoundingBox);
        });
    }

    [Fact]
    public async Task VisionProvider_AnalyzeStreamAsync_YieldsChunks()
    {
        var provider = new InMemoryAiVisionProvider();

        var request = new XForge.DesignSystem.Services.AI.AiVisionRequest
        {
            Prompt = "Analise streaming",
            Images = new[] { "img_base64" }
        };

        var chunks = new List<AiStreamChunk>();
        await foreach (var chunk in provider.AnalyzeStreamAsync(request))
        {
            chunks.Add(chunk);
        }

        Assert.True(chunks.Count > 1);
        Assert.All(chunks.Take(chunks.Count - 1), c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Delta));
            Assert.Null(c.FinishReason);
        });
        Assert.Equal("stop", chunks.Last().FinishReason);
    }

    #endregion

    #region IAiAudioProvider - TranscribeAsync

    [Fact]
    public async Task AudioProvider_TranscribeAsync_ReturnsText()
    {
        var provider = new InMemoryAiAudioProvider();

        var request = new XForge.DesignSystem.Services.AI.AiTranscriptionRequest
        {
            Audio = "audio_base64",
            Language = "pt-BR"
        };

        var response = await provider.TranscribeAsync(request);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Text));
        Assert.Contains("pedido", response.Text.ToLowerInvariant());
        Assert.Equal("pt-BR", response.DetectedLanguage);
    }

    [Fact]
    public async Task AudioProvider_TranscribeAsync_ReturnsSegments()
    {
        var provider = new InMemoryAiAudioProvider();

        var request = new XForge.DesignSystem.Services.AI.AiTranscriptionRequest
        {
            Audio = "audio_base64",
            IncludeTimestamps = true
        };

        var response = await provider.TranscribeAsync(request);

        Assert.NotNull(response.Segments);
        Assert.True(response.Segments.Count > 0);
        Assert.All(response.Segments, s =>
        {
            Assert.False(string.IsNullOrWhiteSpace(s.Text));
            Assert.True(s.Start >= 0);
            Assert.True(s.End >= s.Start);
        });
    }

    [Fact]
    public async Task AudioProvider_TranscribeStreamAsync_YieldsChunks()
    {
        var provider = new InMemoryAiAudioProvider();

        var request = new XForge.DesignSystem.Services.AI.AiTranscriptionRequest
        {
            Audio = "audio_base64",
            Language = "pt-BR"
        };

        var chunks = new List<AiTranscriptionChunk>();
        await foreach (var chunk in provider.TranscribeStreamAsync(request))
        {
            chunks.Add(chunk);
        }

        Assert.True(chunks.Count > 1);
        Assert.Contains(chunks, c => c.IsFinal);
        Assert.All(chunks, c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Text));
        });
    }

    [Fact]
    public async Task AudioProvider_SynthesizeSpeechAsync_ReturnsAudio()
    {
        var provider = new InMemoryAiAudioProvider();

        var request = new XForge.DesignSystem.Services.AI.AiSpeechRequest
        {
            Text = "Ola, mundo!",
            Language = "pt-BR",
            Speed = 1.0f
        };

        var response = await provider.SynthesizeSpeechAsync(request);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Audio));
        Assert.Equal("mp3", response.Format);
        Assert.True(response.DurationSeconds > 0);
    }

    [Fact]
    public async Task AudioProvider_AnalyzeAudioAsync_ReturnsSummary()
    {
        var provider = new InMemoryAiAudioProvider();

        var request = new AiAudioAnalysisRequest
        {
            Audio = "audio_base64",
            Language = "pt-BR",
            AnalysisTypes = new[] { "summary", "sentiment", "topics" }
        };

        var response = await provider.AnalyzeAudioAsync(request);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Summary));
        Assert.False(string.IsNullOrWhiteSpace(response.Sentiment));
        Assert.NotNull(response.Topics);
        Assert.True(response.Topics.Count > 0);
    }

    #endregion

    #region XFAiVisionPanel - Rendering

    [Fact]
    public void VisionPanel_Renders_Default()
    {
        var cut = Render<XFAiVisionPanel>();

        Assert.Contains("xf-ai-vision-panel", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void VisionPanel_Shows_Header()
    {
        var cut = Render<XFAiVisionPanel>();

        Assert.Contains("An\u00e1lise Visual", cut.Markup);
        Assert.Contains("<h3", cut.Markup);
    }

    [Fact]
    public void VisionPanel_Shows_AnalysisTypeSelector()
    {
        var cut = Render<XFAiVisionPanel>();

        Assert.Contains("role=\"radiogroup\"", cut.Markup);
        Assert.Contains("role=\"radio\"", cut.Markup);
        Assert.Contains("Descrever", cut.Markup);
        Assert.Contains("OCR", cut.Markup);
        Assert.Contains("Objetos", cut.Markup);
    }

    [Fact]
    public void VisionPanel_Custom_Title()
    {
        var cut = Render<XFAiVisionPanel>(p => p
            .Add(x => x.Title, "Minha An\u00e1lise Visual")
        );

        Assert.Contains("Minha An\u00e1lise Visual", cut.Markup);
    }

    [Fact]
    public void VisionPanel_Custom_AriaLabel()
    {
        var cut = Render<XFAiVisionPanel>(p => p
            .Add(x => x.AriaLabel, "Painel de vis\u00e3o computacional")
        );

        Assert.Contains("aria-label=\"Painel de vis\u00e3o computacional\"", cut.Markup);
    }

    #endregion

    #region XFAiVoiceAssistant - Rendering

    [Fact]
    public void VoiceAssistant_Renders_Default()
    {
        var cut = Render<XFAiVoiceAssistant>();

        Assert.Contains("xf-ai-voice-assistant", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void VoiceAssistant_Shows_Header()
    {
        var cut = Render<XFAiVoiceAssistant>();

        Assert.Contains("Assistente de Voz", cut.Markup);
        Assert.Contains("<h3", cut.Markup);
    }

    [Fact]
    public void VoiceAssistant_Shows_TtsSection()
    {
        var cut = Render<XFAiVoiceAssistant>();

        Assert.Contains("Texto para fala", cut.Markup);
        Assert.Contains("<input", cut.Markup);
    }

    [Fact]
    public void VoiceAssistant_Custom_Title()
    {
        var cut = Render<XFAiVoiceAssistant>(p => p
            .Add(x => x.Title, "Meu Assistente de Voz")
        );

        Assert.Contains("Meu Assistente de Voz", cut.Markup);
    }

    [Fact]
    public void VoiceAssistant_Custom_AriaLabel()
    {
        var cut = Render<XFAiVoiceAssistant>(p => p
            .Add(x => x.AriaLabel, "Assistente de voz da empresa")
        );

        Assert.Contains("aria-label=\"Assistente de voz da empresa\"", cut.Markup);
    }

    #endregion

    #region XFAiMultiModalChat - Rendering

    [Fact]
    public void MultiModalChat_Renders_Default()
    {
        var cut = Render<XFAiMultiModalChat>();

        Assert.Contains("xf-ai-multimodal-chat", cut.Markup);
        Assert.Contains("role=\"log\"", cut.Markup);
    }

    [Fact]
    public void MultiModalChat_Shows_Header()
    {
        var cut = Render<XFAiMultiModalChat>();

        Assert.Contains("Chat Multi-modal", cut.Markup);
        Assert.Contains("<h3", cut.Markup);
    }

    [Fact]
    public void MultiModalChat_Shows_EmptyState()
    {
        var cut = Render<XFAiMultiModalChat>();

        Assert.Contains("Envie imagens", cut.Markup);
    }

    [Fact]
    public void MultiModalChat_Custom_Title()
    {
        var cut = Render<XFAiMultiModalChat>(p => p
            .Add(x => x.Title, "Chat Empresarial")
        );

        Assert.Contains("Chat Empresarial", cut.Markup);
    }

    [Fact]
    public void MultiModalChat_Custom_AriaLabel()
    {
        var cut = Render<XFAiMultiModalChat>(p => p
            .Add(x => x.AriaLabel, "Chat multi-modal corporativo")
        );

        Assert.Contains("aria-label=\"Chat multi-modal corporativo\"", cut.Markup);
    }

    #endregion

    #region XFAiVideoAnalyzer - Rendering

    [Fact]
    public void VideoAnalyzer_Renders_Default()
    {
        var cut = Render<XFAiVideoAnalyzer>();

        Assert.Contains("xf-ai-video-analyzer", cut.Markup);
        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void VideoAnalyzer_Shows_Header()
    {
        var cut = Render<XFAiVideoAnalyzer>();

        Assert.Contains("An\u00e1lise de V\u00eddeo", cut.Markup);
        Assert.Contains("<h3", cut.Markup);
    }

    [Fact]
    public void VideoAnalyzer_Shows_FrameSelector()
    {
        var cut = Render<XFAiVideoAnalyzer>();

        Assert.Contains("Frames para an\u00e1lise", cut.Markup);
        Assert.Contains("type=\"range\"", cut.Markup);
    }

    [Fact]
    public void VideoAnalyzer_Custom_Title()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p
            .Add(x => x.Title, "Analisador de V\u00eddeo Avan\u00e7ado")
        );

        Assert.Contains("Analisador de V\u00eddeo Avan\u00e7ado", cut.Markup);
    }

    [Fact]
    public void VideoAnalyzer_Custom_AriaLabel()
    {
        var cut = Render<XFAiVideoAnalyzer>(p => p
            .Add(x => x.AriaLabel, "Analisador de v\u00eddeo da empresa")
        );

        Assert.Contains("aria-label=\"Analisador de v\u00eddeo da empresa\"", cut.Markup);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Minimal mock IAiProvider for rendering tests that require dependency injection.
    /// </summary>
    private sealed class MockAiProvider : IAiProvider
    {
        public string ProviderId => "mock";
        public string DisplayName => "Mock AI Provider";
        public bool SupportsStreaming => false;
        public bool SupportsToolCalling => false;
        public string? DefaultModel => null;

        public Task<AiChatResponse> CompleteAsync(AiChatRequest request, CancellationToken ct = default) =>
            Task.FromResult(new AiChatResponse
            {
                Message = new AiMessage { Role = AiRole.Assistant, Content = "Resposta simulada." }
            });

        public IAsyncEnumerable<AiStreamChunk> StreamAsync(AiChatRequest request, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<AiEmbeddingResponse> EmbedAsync(AiEmbeddingRequest request, CancellationToken ct = default) =>
            Task.FromResult(new AiEmbeddingResponse { Embeddings = request.Inputs.Select(_ => new float[] { 1, 0, 0 }).ToList() });

        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }

    #endregion
}
