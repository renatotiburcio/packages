using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.DataDisplay.Code
{
    public partial class XFCode : XForgeComponentBase, IAsyncDisposable
    {
        protected override string BaseClass => "xf-code";

        [Parameter] public string? Text { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Language { get; set; }
        [Parameter] public bool Copyable { get; set; }
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Wrap { get; set; }
        [Parameter] public XFCodeVariant Variant { get; set; } = XFCodeVariant.Inline;
        [Parameter] public XFSemanticColor Tone { get; set; } = XFSemanticColor.Neutral;
        [Parameter] public EventCallback<string> Copied { get; set; }
        [Parameter] public string CopyLabel { get; set; } = "Copy code";
        [Parameter] public string CopyText { get; set; } = "Copy";
        [Parameter] public string CopiedText { get; set; } = "Copied";

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private IJSObjectReference? ClipboardModule { get; set; }
        private bool WasCopied { get; set; }
        private CancellationTokenSource? _resetCts;

        private string DisplayText => Text ?? string.Empty;

        private bool ShowHeader => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Language) || Copyable;

        protected override string GetComponentClass()
        {
            if (Variant == XFCodeVariant.Inline)
            {
                return GetClass($"{BaseClass} inline-code rounded-md px-1.5 py-0.5 font-mono text-[0.875em] ring-1 transition-all duration-150 {InlineToneClass}");
            }

            return GetClass($"{BaseClass} block-code overflow-hidden rounded-xl border bg-bg-surface shadow-sm {BlockToneClass}");
        }

        private string HeaderClass => "flex items-center justify-between gap-3 border-b border-border-light bg-surface-50 px-4 py-2.5";

        private string PreClass => $"max-h-[28rem] overflow-auto p-4 text-sm leading-6 {WrapClass}";

        private string CodeClass => "font-mono text-text-primary";

        private string WrapClass => Wrap ? "whitespace-pre-wrap break-words" : "whitespace-pre";

        private string LanguageClass => "rounded-md bg-primary/8 px-2 py-0.5 font-mono text-xs font-semibold uppercase text-primary ring-1 ring-primary/14";

        private string CopyButtonClass => $"inline-flex h-10 min-w-[2.5rem] items-center justify-center gap-1.5 rounded-lg px-3 text-xs font-semibold outline-none transition-all duration-150 {CopyButtonStateClass}";

        private string CopyButtonStateClass => WasCopied
            ? "bg-success/10 text-success ring-1 ring-success/20"
            : "text-text-secondary ring-1 ring-border-light hover:bg-primary/7 hover:text-primary focus-visible:ring-2 focus-visible:ring-primary/24 disabled:pointer-events-none disabled:opacity-50";

        private string CopyButtonText => WasCopied ? CopiedText : CopyText;

        private string InlineToneClass => Tone switch
        {
            XFSemanticColor.Primary => "bg-primary/8 text-primary ring-primary/16",
            XFSemanticColor.Success => "bg-success/8 text-success ring-success/16",
            XFSemanticColor.Warning => "bg-warning/10 text-warning ring-warning/18",
            XFSemanticColor.Danger => "bg-danger/8 text-danger ring-danger/16",
            _ => "bg-surface-100 text-text-primary ring-border-light"
        };

        private string BlockToneClass => Tone switch
        {
            XFSemanticColor.Primary => "border-primary/18 ring-1 ring-primary/10",
            XFSemanticColor.Success => "border-success/18 ring-1 ring-success/10",
            XFSemanticColor.Warning => "border-warning/22 ring-1 ring-warning/12",
            XFSemanticColor.Danger => "border-danger/18 ring-1 ring-danger/10",
            _ => "border-border-light ring-1 ring-border-light"
        };

        private async Task CopyAsync()
        {
            if (Disabled)
            {
                return;
            }

            ClipboardModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/XForge.DesignSystem/js/xf-clipboard.js");
            await ClipboardModule.InvokeAsync<bool>("copyText", DisplayText);
            WasCopied = true;
            StateHasChanged();

            if (Copied.HasDelegate)
            {
                await Copied.InvokeAsync(DisplayText);
            }

            _resetCts?.Cancel();
            _resetCts?.Dispose();
            _resetCts = new CancellationTokenSource();
            _ = ResetCopyStateAsync(_resetCts.Token);
        }

        private async Task ResetCopyStateAsync(CancellationToken ct)
        {
            try
            {
                await Task.Delay(2500, ct);
                WasCopied = false;
                await InvokeAsync(StateHasChanged);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async ValueTask DisposeAsync()
        {
            _resetCts?.Cancel();
            _resetCts?.Dispose();

            if (ClipboardModule is not null)
            {
                try
                {
                    await ClipboardModule.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}
