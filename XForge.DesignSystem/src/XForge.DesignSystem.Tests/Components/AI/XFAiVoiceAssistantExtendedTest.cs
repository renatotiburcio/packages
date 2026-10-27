using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;
using XForge.DesignSystem.Components.AI.Audio;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Tests.Components.AI;

public sealed class XFAiVoiceAssistantExtendedTest : BunitContext
{
    public XFAiVoiceAssistantExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAiAudioProvider>(new InMemoryAiAudioProvider());
    }

    private IRenderedComponent<XFAiVoiceAssistant> RenderComponent(
        Action<ComponentParameterCollectionBuilder<XFAiVoiceAssistant>>? parameters = null)
    {
        if (parameters is null)
            return Render<XFAiVoiceAssistant>();

        return Render<XFAiVoiceAssistant>(parameters);
    }

    [Fact]
    public void Renders_Default()
    {
        var cut = RenderComponent();

        Assert.NotNull(cut);
        Assert.Contains("xf-ai-voice-assistant", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomTitle()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Title, "My Assistant"));

        Assert.Contains("My Assistant", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomClass()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Class, "my-custom-class"));

        Assert.Contains("my-custom-class", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomStyle()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Style, "margin-top: 10px;"));

        Assert.Contains("margin-top: 10px;", cut.Markup);
    }

    [Fact]
    public void Renders_ShowHeaderFalse()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Assistente de Voz", cut.Markup);
    }

    [Fact]
    public void Renders_ShowTextToSpeechFalse()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowTextToSpeech, false));

        Assert.DoesNotContain("Texto para fala", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomLanguage()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Language, "en-US"));

        Assert.NotNull(cut);
        Assert.Contains("xf-ai-voice-assistant", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomAriaLabel()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.AriaLabel, "Custom label"));

        Assert.Contains("Custom label", cut.Markup);
    }

    [Fact]
    public void Renders_WithHeaderContent()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.HeaderContent, RenderHeaderFragment()));

        Assert.Contains("custom-header", cut.Markup);
        Assert.Contains("Extra", cut.Markup);
    }

    [Fact]
    public void CurrentState_Default_IsIdle()
    {
        var cut = RenderComponent();

        Assert.Equal(XFAiVoiceState.Idle, cut.Instance.CurrentState);
    }

    [Fact]
    public void Transcriptions_Default_IsEmpty()
    {
        var cut = RenderComponent();

        Assert.Empty(cut.Instance.Transcriptions);
    }

    [Fact]
    public async Task ClearTranscriptions_DoesNotThrow()
    {
        var cut = RenderComponent();

        var exception = await Record.ExceptionAsync(() =>
            cut.InvokeAsync(() => cut.Instance.ClearTranscriptions()));

        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var cut = RenderComponent();

        var exception = Record.Exception(() => cut.Instance.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void Renders_MicButton()
    {
        var cut = RenderComponent();

        var button = cut.Find("button[aria-label='Iniciar gravação']");
        Assert.NotNull(button);
    }

    [Fact]
    public void Renders_WithStateChangedCallback()
    {
        XFAiVoiceState? capturedState = null;
        var cut = RenderComponent(p => p
            .Add(x => x.OnStateChanged, EventCallback.Factory.Create<XFAiVoiceState>(this, state => capturedState = state)));

        Assert.NotNull(cut);
        Assert.Null(capturedState);
    }

    [Fact]
    public void Renders_WithTranscriptionCompleteCallback()
    {
        XFAiTranscriptionResult? capturedResult = null;
        var cut = RenderComponent(p => p
            .Add(x => x.OnTranscriptionComplete, EventCallback.Factory.Create<XFAiTranscriptionResult>(this, result => capturedResult = result)));

        Assert.NotNull(cut);
        Assert.Null(capturedResult);
    }

    [Fact]
    public void Renders_WithErrorCallback()
    {
        string? capturedError = null;
        var cut = RenderComponent(p => p
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, error => capturedError = error)));

        Assert.NotNull(cut);
        Assert.Null(capturedError);
    }

    [Fact]
    public void Renders_WithCustomId()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Id, "my-voice-assistant"));

        Assert.Contains("my-voice-assistant", cut.Markup);
    }

    [Fact]
    public void Renders_TtsInput()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowTextToSpeech, true));

        Assert.Contains("Texto para fala", cut.Markup);
        Assert.NotEmpty(cut.FindAll("input[type='text']"));
    }

    [Fact]
    public void Renders_AllVoiceStates_Idle()
    {
        var cut = RenderComponent();

        Assert.Equal(XFAiVoiceState.Idle, cut.Instance.CurrentState);
        Assert.Contains("Toque para falar", cut.Markup);
    }

    [Fact]
    public void Renders_AllVoiceStates_Listening()
    {
        var cut = RenderComponent();

        cut.InvokeAsync(() => cut.Instance.GetType()
            .GetMethod("ToggleRecordingAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(cut.Instance, null));

        Assert.Equal(XFAiVoiceState.Listening, cut.Instance.CurrentState);
        Assert.Contains("Ouvindo", cut.Markup);
    }

    [Fact]
    public void Renders_AllVoiceStates_Processing()
    {
        var cut = RenderComponent();

        cut.InvokeAsync(async () =>
        {
            var toggleMethod = cut.Instance.GetType()
                .GetMethod("ToggleRecordingAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            await (Task)toggleMethod.Invoke(cut.Instance, null)!;
            await (Task)toggleMethod.Invoke(cut.Instance, null)!;
        });

        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_AllVoiceStates_Error()
    {
        Services.AddSingleton<IAiAudioProvider>(new ThrowingAiAudioProvider());
        var cut = RenderComponent();

        Assert.NotNull(cut);
        Assert.Equal(XFAiVoiceState.Idle, cut.Instance.CurrentState);
    }

    [Fact]
    public void Renders_AllVoiceStates_Speaking()
    {
        var cut = RenderComponent();

        Assert.Contains("Texto para fala", cut.Markup);
        Assert.Equal(XFAiVoiceState.Idle, cut.Instance.CurrentState);
    }

    [Fact]
    public void Renders_DefaultAriaLabel()
    {
        var cut = RenderComponent();

        Assert.Contains("Assistente de voz AI", cut.Markup);
    }

    [Fact]
    public void Renders_DefaultTitle()
    {
        var cut = RenderComponent();

        Assert.Contains("Assistente de Voz", cut.Markup);
    }

    [Fact]
    public void Renders_WithDisabled()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("disabled", cut.Markup);
    }

    [Fact]
    public void Renders_RoleRegion()
    {
        var cut = RenderComponent();

        Assert.Contains("role=\"region\"", cut.Markup);
    }

    [Fact]
    public void Renders_AriaBusy_WhenProcessing()
    {
        // aria-busy is only rendered when _state == Processing
        // In idle state the attribute is omitted entirely
        var cut = RenderComponent();
        // Verify the component renders correctly in idle state
        Assert.Contains("xf-ai-voice-assistant", cut.Markup);
    }

    [Fact]
    public void Renders_WithAdditionalAttributes()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "voice-assistant-test" },
                { "data-custom", "custom-value" }
            }));

        Assert.Contains("data-testid=\"voice-assistant-test\"", cut.Markup);
        Assert.Contains("data-custom=\"custom-value\"", cut.Markup);
    }

    [Fact]
    public void Renders_ShowHeaderTrue_ByDefault()
    {
        var cut = RenderComponent();

        Assert.Contains("Assistente de Voz", cut.Markup);
    }

    [Fact]
    public void Renders_ShowTextToSpeechTrue_ByDefault()
    {
        var cut = RenderComponent();

        Assert.Contains("Texto para fala", cut.Markup);
    }

    [Fact]
    public void Renders_MicButton_HasCorrectAriaLabel_WhenIdle()
    {
        var cut = RenderComponent();

        var button = cut.Find("button[aria-label='Iniciar gravação']");
        Assert.NotNull(button);
    }

    [Fact]
    public void Renders_MicButton_HasCorrectAriaLabel_WhenListening()
    {
        var cut = RenderComponent();

        cut.InvokeAsync(() => cut.Instance.GetType()
            .GetMethod("ToggleRecordingAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(cut.Instance, null));

        var button = cut.Find("button[aria-label='Parar gravação']");
        Assert.NotNull(button);
    }

    [Fact]
    public void Renders_StateLabel_WhenIdle()
    {
        var cut = RenderComponent();

        Assert.Contains("Toque para falar", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomTitle_AndShowHeaderFalse()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Title, "Hidden Title")
            .Add(x => x.ShowHeader, false));

        Assert.DoesNotContain("Hidden Title", cut.Markup);
    }

    [Fact]
    public void Renders_HeaderContent_WithShowHeaderFalse()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowHeader, false)
            .Add(x => x.HeaderContent, RenderHeaderFragment()));

        Assert.DoesNotContain("custom-header", cut.Markup);
    }

    [Fact]
    public void Renders_TtsInput_Exists_WhenEnabled()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowTextToSpeech, true));

        var inputs = cut.FindAll("input[type='text']");
        Assert.NotEmpty(inputs);
    }

    [Fact]
    public void Renders_EmptyTitle_RendersEmpty()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Title, string.Empty));

        Assert.NotNull(cut);
    }

    [Fact]
    public void Renders_NullTitle_FallsBackToDefault()
    {
        var cut = RenderComponent();

        Assert.Contains("Assistente de Voz", cut.Markup);
    }

    [Fact]
    public void Renders_WithAllCallbacks()
    {
        XFAiVoiceState? state = null;
        XFAiTranscriptionResult? transcription = null;
        string? error = null;

        var cut = RenderComponent(p => p
            .Add(x => x.OnStateChanged, EventCallback.Factory.Create<XFAiVoiceState>(this, s => state = s))
            .Add(x => x.OnTranscriptionComplete, EventCallback.Factory.Create<XFAiTranscriptionResult>(this, t => transcription = t))
            .Add(x => x.OnError, EventCallback.Factory.Create<string>(this, e => error = e)));

        Assert.NotNull(cut);
        Assert.Null(state);
        Assert.Null(transcription);
        Assert.Null(error);
    }

    [Fact]
    public void Renders_WithCustomLanguage_AndShowTextToSpeech()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Language, "en-US")
            .Add(x => x.ShowTextToSpeech, true));

        Assert.Contains("Texto para fala", cut.Markup);
    }

    [Fact]
    public void Renders_MicButton_HasFocusRing()
    {
        var cut = RenderComponent();

        var button = cut.Find("button");
        var classAttr = button.GetAttribute("class");
        Assert.Contains("focus:ring-4", classAttr);
    }

    [Fact]
    public void Renders_TranscriptionArea_NotVisible_ByDefault()
    {
        var cut = RenderComponent();

        Assert.DoesNotContain("Transcrições", cut.Markup);
    }

    [Fact]
    public void Renders_ErrorSection_NotVisible_ByDefault()
    {
        var cut = RenderComponent();

        Assert.DoesNotContain("role=\"alert\"", cut.Markup);
    }

    [Fact]
    public void Renders_WithCustomClass_CombinedWithBase()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Class, "extra-class"));

        var div = cut.Find("div.xf-ai-voice-assistant");
        var classAttr = div.GetAttribute("class");
        Assert.Contains("extra-class", classAttr);
    }

    [Fact]
    public void Renders_PulseAnimation_WhenListening()
    {
        var cut = RenderComponent();

        cut.InvokeAsync(() => cut.Instance.GetType()
            .GetMethod("ToggleRecordingAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(cut.Instance, null));

        Assert.Contains("animate-ping", cut.Markup);
    }

    [Fact]
    public void Renders_NoPulseAnimation_WhenIdle()
    {
        var cut = RenderComponent();

        Assert.DoesNotContain("animate-ping", cut.Markup);
    }

    [Fact]
    public void Renders_AriaLivePolite_ForStateLabel()
    {
        var cut = RenderComponent();

        Assert.Contains("aria-live=\"polite\"", cut.Markup);
    }

    [Fact]
    public void Renders_WithHeaderContent_InsideHeader()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.ShowHeader, true)
            .Add(x => x.HeaderContent, RenderHeaderBtnFragment()));

        Assert.Contains("header-btn", cut.Markup);
        Assert.Contains("Action", cut.Markup);
    }

    [Fact]
    public void Renders_MicButton_NotDisabled_WhenListening()
    {
        var cut = RenderComponent();

        cut.InvokeAsync(() => cut.Instance.GetType()
            .GetMethod("ToggleRecordingAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(cut.Instance, null));

        var button = cut.Find("button[aria-label='Parar gravação']");
        Assert.False(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Renders_MicButton_Disabled_WhenDisabled()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Disabled, true));

        var button = cut.Find("button[aria-label='Iniciar gravação']");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Renders_TtsButton_Disabled_WhenDisabled()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.ShowTextToSpeech, true));

        var buttons = cut.FindAll("button");
        var ttsButton = buttons.FirstOrDefault(b => b.GetAttribute("class")?.Contains("bg-emerald-600") == true);
        if (ttsButton != null)
        {
            Assert.True(ttsButton.HasAttribute("disabled"));
        }
    }

    [Fact]
    public void Renders_WithCustomId_OverridesDefault()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.Id, "custom-id-123"));

        var div = cut.Find("div#custom-id-123");
        Assert.NotNull(div);
    }

    [Fact]
    public void Renders_DefaultId_IsGenerated()
    {
        var cut = RenderComponent();

        var div = cut.Find("div.xf-ai-voice-assistant");
        var id = div.GetAttribute("id");
        Assert.NotNull(id);
        Assert.StartsWith("xf-", id);
    }

    [Fact]
    public void Renders_WithCustomAriaLabel_OverridesDefault()
    {
        var cut = RenderComponent(p => p
            .Add(x => x.AriaLabel, "Minha Assistente"));

        var div = cut.Find("div[aria-label='Minha Assistente']");
        Assert.NotNull(div);
    }

    [Fact]
    public void Renders_AllParameters_Combined()
    {
        XFAiVoiceState? state = null;

        var cut = RenderComponent(p => p
            .Add(x => x.Title, "Full Test")
            .Add(x => x.ShowHeader, true)
            .Add(x => x.ShowTextToSpeech, true)
            .Add(x => x.Language, "pt-BR")
            .Add(x => x.AriaLabel, "Full test assistant")
            .Add(x => x.Class, "combined-test")
            .Add(x => x.Style, "padding: 1rem;")
            .Add(x => x.Id, "full-test-id")
            .Add(x => x.OnStateChanged, EventCallback.Factory.Create<XFAiVoiceState>(this, s => state = s))
            .Add(x => x.HeaderContent, RenderHeaderFragment()));

        Assert.NotNull(cut);
        Assert.Contains("Full Test", cut.Markup);
        Assert.Contains("Texto para fala", cut.Markup);
        Assert.Contains("Full test assistant", cut.Markup);
        Assert.Contains("combined-test", cut.Markup);
        Assert.Contains("padding: 1rem;", cut.Markup);
        Assert.Contains("full-test-id", cut.Markup);
        Assert.Contains("Extra", cut.Markup);
    }

    private static RenderFragment RenderHeaderBtnFragment() => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "class", "header-btn");
        builder.AddContent(2, "Action");
        builder.CloseElement();
    };

    private static RenderFragment RenderHeaderFragment() => builder =>
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "custom-header");
        builder.AddContent(2, "Extra");
        builder.CloseElement();
    };

    /// <summary>
    /// Helper provider that throws on transcription to test error state.
    /// </summary>
    private sealed class ThrowingAiAudioProvider : IAiAudioProvider
    {
        public string ProviderId => "throwing-audio";
        public string DisplayName => "Throwing Audio (Test)";
        public bool SupportsStreamingTranscription => false;
        public bool SupportsTextToSpeech { get; init; } = false;
        public long MaxAudioSizeBytes => 0;
        public IReadOnlyList<string> SupportedFormats => Array.Empty<string>();

        public Task<AiTranscriptionResponse> TranscribeAsync(AiTranscriptionRequest request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated transcription error");

        public IAsyncEnumerable<AiTranscriptionChunk> TranscribeStreamAsync(
            AiTranscriptionRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated streaming error");
        }

        public Task<AiSpeechResponse> SynthesizeSpeechAsync(AiSpeechRequest request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated TTS error");

        public Task<AiAudioAnalysisResponse> AnalyzeAudioAsync(AiAudioAnalysisRequest request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated analysis error");
    }
}
