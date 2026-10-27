using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Services.AI;

namespace XForge.DesignSystem.Components.AI.Audio;

/// <summary>
/// Enterprise AI voice assistant with speech-to-text, text-to-speech, and audio analysis.
/// Supports real-time transcription streaming via IAiAudioProvider.
/// </summary>
public partial class XFAiVoiceAssistant : XForgeComponentBase, IDisposable
{
    private XFAiVoiceState _state = XFAiVoiceState.Idle;
    private readonly List<XFAiTranscriptionResult> _transcriptionResults = new();
    private string _currentPartialText = "";
    private string _ttsText = "";
    private string? _error;

    [Inject] public IAiAudioProvider? AudioProvider { get; set; }

    /// <summary>Title displayed in the header.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Whether to show the header.</summary>
    [Parameter] public bool ShowHeader { get; set; } = true;

    /// <summary>Whether to show text-to-speech section.</summary>
    [Parameter] public bool ShowTextToSpeech { get; set; } = true;

    /// <summary>Language hint for transcription (e.g., "pt-BR").</summary>
    [Parameter] public string? Language { get; set; }

    /// <summary>ARIA label for the assistant.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Assistente de voz AI";

    /// <summary>Custom header content.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary>Callback when transcription completes.</summary>
    [Parameter] public EventCallback<XFAiTranscriptionResult> OnTranscriptionComplete { get; set; }

    /// <summary>Callback when state changes.</summary>
    [Parameter] public EventCallback<XFAiVoiceState> OnStateChanged { get; set; }

    /// <summary>Callback when an error occurs.</summary>
    [Parameter] public EventCallback<string> OnError { get; set; }

    /// <summary>Current voice state.</summary>
    public XFAiVoiceState CurrentState => _state;

    /// <summary>All transcription results.</summary>
    public IReadOnlyList<XFAiTranscriptionResult> Transcriptions => _transcriptionResults;

    private string GetMicButtonClasses() => _state switch
    {
        XFAiVoiceState.Listening => "bg-emerald-500 hover:bg-emerald-600 ring-emerald-300 dark:ring-emerald-700",
        XFAiVoiceState.Processing => "bg-gray-400 cursor-wait ring-gray-300",
        XFAiVoiceState.Speaking => "bg-blue-500 hover:bg-blue-600 ring-blue-300",
        XFAiVoiceState.Error => "bg-red-500 hover:bg-red-600 ring-red-300",
        _ => "bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 ring-gray-300 dark:ring-gray-600 border border-gray-300 dark:border-gray-600"
    };

    private string GetStateLabel() => _state switch
    {
        XFAiVoiceState.Idle => "Toque para falar",
        XFAiVoiceState.Listening => "Ouvindo... toque para parar",
        XFAiVoiceState.Processing => "Processando áudio...",
        XFAiVoiceState.Speaking => "Reproduzindo áudio...",
        XFAiVoiceState.Error => "Erro — toque para tentar novamente",
        _ => ""
    };

    private static string GetConfidenceTextClass(float confidence) => confidence switch
    {
        >= 0.8f => "text-green-600 dark:text-green-400",
        >= 0.6f => "text-yellow-600 dark:text-yellow-400",
        _ => "text-red-600 dark:text-red-400"
    };

    private async Task ToggleRecordingAsync()
    {
        if (_state == XFAiVoiceState.Listening)
        {
            await StopRecordingAsync();
        }
        else
        {
            await StartRecordingAsync();
        }
    }

    private async Task StartRecordingAsync()
    {
        if (AudioProvider == null)
        {
            _error = "Provedor de áudio não configurado.";
            await OnError.InvokeAsync(_error);
            return;
        }

        _state = XFAiVoiceState.Listening;
        _error = null;
        _currentPartialText = "";
        await OnStateChanged.InvokeAsync(_state);
        StateHasChanged();

        // In production, this would use browser MediaRecorder API via JS interop
        // For now, simulate recording state
    }

    private async Task StopRecordingAsync()
    {
        _state = XFAiVoiceState.Processing;
        await OnStateChanged.InvokeAsync(_state);
        StateHasChanged();

        try
        {
            // In production, the recorded audio blob would be sent here
            // For now, simulate with empty audio
            if (AudioProvider != null)
            {
                var request = new AiTranscriptionRequest
                {
                    Audio = "", // Would be base64 from MediaRecorder
                    Language = Language ?? "pt-BR",
                    IncludeTimestamps = true,
                    Format = "webm"
                };

                var response = await AudioProvider.TranscribeAsync(request);

                var result = new XFAiTranscriptionResult
                {
                    Text = response.Text,
                    Language = response.DetectedLanguage,
                    DurationSeconds = response.DurationSeconds,
                    Confidence = response.Segments?.FirstOrDefault()?.Words?.Average(w => w.Confidence)
                };

                _transcriptionResults.Add(result);
                await OnTranscriptionComplete.InvokeAsync(result);
            }
        }
        catch (Exception ex)
        {
            _error = $"Erro na transcrição: {ex.Message}";
            _state = XFAiVoiceState.Error;
            await OnError.InvokeAsync(_error);
        }
        finally
        {
            if (_state != XFAiVoiceState.Error)
            {
                _state = XFAiVoiceState.Idle;
            }
            await OnStateChanged.InvokeAsync(_state);
            StateHasChanged();
        }
    }

    private async Task SynthesizeSpeechAsync()
    {
        if (AudioProvider == null || string.IsNullOrWhiteSpace(_ttsText))
            return;

        _state = XFAiVoiceState.Speaking;
        _error = null;
        await OnStateChanged.InvokeAsync(_state);
        StateHasChanged();

        try
        {
            var request = new AiSpeechRequest
            {
                Text = _ttsText,
                Language = Language ?? "pt-BR",
                Speed = 1.0f
            };

            var response = await AudioProvider.SynthesizeSpeechAsync(request);

            // In production, this would play the audio via JS interop
            // For now, just reset state
        }
        catch (Exception ex)
        {
            _error = $"Erro na síntese de fala: {ex.Message}";
            _state = XFAiVoiceState.Error;
            await OnError.InvokeAsync(_error);
        }
        finally
        {
            if (_state != XFAiVoiceState.Error)
            {
                _state = XFAiVoiceState.Idle;
            }
            await OnStateChanged.InvokeAsync(_state);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Clears all transcription results.
    /// </summary>
    public void ClearTranscriptions()
    {
        _transcriptionResults.Clear();
        _currentPartialText = "";
        StateHasChanged();
    }

    public void Dispose()
    {
        // No unmanaged resources to dispose
    }
}
