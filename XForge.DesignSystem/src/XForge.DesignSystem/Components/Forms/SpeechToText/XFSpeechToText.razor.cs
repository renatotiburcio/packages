using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Forms.SpeechToText
{
    public partial class XFSpeechToText : XFInputComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private IJSObjectReference? _module;
        private DotNetObjectReference<XFSpeechToText>? _dotNetRef;
        private bool _isRecording;
        private bool _moduleLoaded;
        private bool _micAvailable;

        /// <summary>Language for speech recognition (e.g., "pt-BR", "en-US").</summary>
        [Parameter] public string? Language { get; set; }

        /// <summary>Whether to show continuous recognition (keeps listening until stopped).</summary>
        [Parameter] public bool Continuous { get; set; }

        /// <summary>Whether to return interim (partial) results.</summary>
        [Parameter] public bool InterimResults { get; set; }

        /// <summary>Label for the start button.</summary>
        [Parameter] public string StartLabel { get; set; } = "Iniciar reconhecimento";

        /// <summary>Label for the stop button.</summary>
        [Parameter] public string StopLabel { get; set; } = "Parar gravação";

        /// <summary>Custom content for start state.</summary>
        [Parameter] public RenderFragment? StartContent { get; set; }

        /// <summary>Custom content for recording state.</summary>
        [Parameter] public RenderFragment? RecordingContent { get; set; }

        /// <summary>Event callback with recognized speech text.</summary>
        [Parameter] public EventCallback<string> Change { get; set; }

        /// <summary>Event callback when recording starts.</summary>
        [Parameter] public EventCallback RecordingStarted { get; set; }

        /// <summary>Event callback when recording stops.</summary>
        [Parameter] public EventCallback RecordingStopped { get; set; }

        /// <summary>Event callback when an error occurs.</summary>
        [Parameter] public EventCallback<string> Error { get; set; }

        /// <summary>Whether the button takes full width.</summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>Visual variant.</summary>
        [Parameter] public XFFormVariant Variant { get; set; } = XFFormVariant.Outline;

        /// <summary>Size of the button.</summary>
        [Parameter] public XFSize Size { get; set; } = XFSize.Md;

        /// <summary>Color tone.</summary>
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;

        /// <summary>Whether speech recognition is currently active.</summary>
        public bool IsRecording => _isRecording;

        private string ButtonClass => Join(
            "xf-speech-to-text inline-flex items-center justify-center rounded-lg border font-medium transition-all duration-200",
            "focus:outline-none focus:ring-2 focus:ring-offset-1",
            SizeClass, VariantClass, ToneClass, StateClass,
            _isRecording ? "recording" : null,
            FullWidth ? "w-full" : null,
            Class);

        private string SizeClass => Size switch
        {
            XFSize.Sm => "h-8 px-3 text-sm gap-1.5",
            XFSize.Lg => "h-12 px-5 text-base gap-2",
            _ => "h-10 px-4 text-sm gap-2"
        };

        private string VariantClass => Variant switch
        {
            XFFormVariant.Filled => "border-transparent text-white",
            XFFormVariant.Ghost => "border-transparent bg-transparent",
            _ => "border-surface-200 bg-bg-surface"
        };

        private string ToneClass => Tone switch
        {
            XFSemanticColor.Primary => _isRecording
                ? "border-danger bg-danger text-white focus:ring-danger"
                : "border-primary bg-primary text-white focus:ring-primary",
            XFSemanticColor.Danger => "border-danger bg-danger text-white focus:ring-danger",
            _ => _isRecording
                ? "border-danger bg-danger/10 text-danger focus:ring-danger"
                : "border-surface-200 text-text-primary focus:ring-primary"
        };

        private string StateClass => Disabled || !_micAvailable && _moduleLoaded
            ? "cursor-not-allowed opacity-50"
            : "cursor-pointer";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await JS.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/XForge.DesignSystem/js/xf-speech-to-text.js");
                    _dotNetRef = DotNetObjectReference.Create(this);
                    _moduleLoaded = true;
                    _micAvailable = await _module.InvokeAsync<bool>("checkMicAvailability");
                    StateHasChanged();
                }
                catch
                {
                    // JS not available during prerender
                }
            }
        }

        private async Task ToggleRecordingAsync()
        {
            if (Disabled || !_moduleLoaded || _module is null) return;
            if (!_micAvailable && !_isRecording)
            {
                if (Error.HasDelegate)
                    await Error.InvokeAsync("Microphone not available");
                return;
            }

            if (_isRecording)
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
            if (_module is null || _dotNetRef is null) return;

            try
            {
                var started = await _module.InvokeAsync<bool>("startRecognition", _dotNetRef, Language, Continuous, InterimResults);
                if (started)
                {
                    _isRecording = true;
                    StateHasChanged();
                    if (RecordingStarted.HasDelegate)
                        await RecordingStarted.InvokeAsync();
                }
                else
                {
                    if (Error.HasDelegate)
                        await Error.InvokeAsync("Speech recognition not supported in this browser");
                }
            }
            catch (JSException ex)
            {
                if (Error.HasDelegate)
                    await Error.InvokeAsync(ex.Message);
            }
        }

        private async Task StopRecordingAsync()
        {
            if (_module is null) return;

            try
            {
                await _module.InvokeVoidAsync("stopRecognition");
                _isRecording = false;
                StateHasChanged();
                if (RecordingStopped.HasDelegate)
                    await RecordingStopped.InvokeAsync();
            }
            catch (JSException ex)
            {
                if (Error.HasDelegate)
                    await Error.InvokeAsync(ex.Message);
            }
        }

        /// <summary>
        /// Called from JS when speech recognition produces a result.
        /// </summary>
        [JSInvokable]
        public async Task OnSpeechResult(string transcript)
        {
            if (Change.HasDelegate)
                await Change.InvokeAsync(transcript);
        }

        /// <summary>
        /// Called from JS when speech recognition ends.
        /// </summary>
        [JSInvokable]
        public void OnSpeechEnd()
        {
            _isRecording = false;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Called from JS when speech recognition encounters an error.
        /// </summary>
        [JSInvokable]
        public async Task OnSpeechError(string error)
        {
            _isRecording = false;
            StateHasChanged();
            if (Error.HasDelegate)
                await Error.InvokeAsync(error);
        }

        public override async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                try
                {
                    if (_isRecording)
                        await _module.InvokeVoidAsync("stopRecognition");
                    await _module.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
                _module = null;
            }

            _dotNetRef?.Dispose();

            await base.DisposeAsync();
        }

        private static string Join(params string?[] values)
        {
            return string.Join(" ", values.Where(v => !string.IsNullOrWhiteSpace(v)));
        }
    }
}
