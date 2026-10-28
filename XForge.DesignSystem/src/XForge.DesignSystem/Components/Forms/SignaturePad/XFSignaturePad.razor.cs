using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.SignaturePad;

public partial class XFSignaturePad : XForgeComponentBase, IAsyncDisposable
{
    private DotNetObjectReference<XFSignaturePad>? dotNetReference;
    private bool registered;
    private string? lastRegisteredValue;
    private string? lastRegisteredPenColor;
    private string? lastRegisteredBackgroundColor;
    private double lastRegisteredPenWidth = double.NaN;
    private double lastRegisteredSmoothingDistance = double.NaN;
    private bool lastRegisteredReadOnly;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IXFLocalizer? Localizer { get; set; }

    private string L(string key, string fallback) => Localizer?[key] ?? fallback;

    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback<string?> Change { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string AriaLabel { get; set; } = default!;
    [Parameter] public string HelperText { get; set; } = default!;
    [Parameter] public string ClearLabel { get; set; } = default!;
    [Parameter] public string UndoLabel { get; set; } = default!;
    [Parameter] public string RequiredText { get; set; } = default!;
    [Parameter] public int Width { get; set; } = 720;
    [Parameter] public int Height { get; set; } = 220;
    [Parameter] public double PenWidth { get; set; } = 3.2;
    [Parameter] public double SmoothingDistance { get; set; } = 1.8;
    [Parameter] public string PenColor { get; set; } = default!;
    [Parameter] public string BackgroundColor { get; set; } = default!;
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ShowToolbar { get; set; } = true;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrWhiteSpace(AriaLabel)) AriaLabel = L("signaturepad.label", AriaLabel);
        if (string.IsNullOrWhiteSpace(HelperText)) HelperText = L("signaturepad.instruction", HelperText);
        if (string.IsNullOrWhiteSpace(ClearLabel)) ClearLabel = L("signaturepad.clear", ClearLabel);
        if (string.IsNullOrWhiteSpace(UndoLabel)) UndoLabel = L("signaturepad.undo", UndoLabel);
        if (string.IsNullOrWhiteSpace(RequiredText)) RequiredText = L("signaturepad.required", RequiredText);
    }

    private string CanvasId => $"{Id}-canvas";
    private bool DisabledOrReadOnly => Disabled || ReadOnly;

    private string RootClass =>
        GetClass("xf-signature-pad grid gap-2");

    private string SurfaceClass =>
        "overflow-hidden rounded-lg border border-border-light bg-white shadow-[0_1px_2px_rgb(15_23_42/.035)]";

    private string CanvasClass => DisabledOrReadOnly
        ? "block h-auto w-full touch-none cursor-not-allowed opacity-70"
        : "block h-auto w-full touch-none cursor-crosshair";

    private string ToolButtonClass =>
        "inline-flex h-8 items-center justify-center rounded-md border border-border-light bg-bg-surface px-3 text-xs font-semibold text-text-primary outline-none hover:text-primary focus-visible:ring-2 focus-visible:ring-border-focus disabled:cursor-not-allowed disabled:opacity-45";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (registered && !ShouldRegister())
        {
            return;
        }

        dotNetReference ??= DotNetObjectReference.Create(this);

        try
        {
            await JSRuntime.InvokeVoidAsync(
                "xfSignaturePad.register",
                CanvasId,
                dotNetReference,
                new
                {
                    value = Value,
                    penColor = PenColor,
                    penWidth = PenWidth,
                    smoothingDistance = SmoothingDistance,
                    backgroundColor = BackgroundColor,
                    readOnly = DisabledOrReadOnly
                });
        }
        catch (InvalidOperationException)
        {
            return;
        }
        catch (JSDisconnectedException)
        {
            return;
        }
        catch (JSException)
        {
            return;
        }

        registered = true;
        lastRegisteredValue = Value;
        lastRegisteredPenColor = PenColor;
        lastRegisteredPenWidth = PenWidth;
        lastRegisteredSmoothingDistance = SmoothingDistance;
        lastRegisteredBackgroundColor = BackgroundColor;
        lastRegisteredReadOnly = DisabledOrReadOnly;
    }

    [JSInvokable]
    public async Task SetValueFromJsAsync(string? value)
    {
        Value = value;
        lastRegisteredValue = value;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }

        if (Change.HasDelegate)
        {
            await Change.InvokeAsync(value);
        }
    }

    private bool ShouldRegister()
    {
        return !registered
            || !string.Equals(Value, lastRegisteredValue, StringComparison.Ordinal)
            || !string.Equals(PenColor, lastRegisteredPenColor, StringComparison.Ordinal)
            || !string.Equals(BackgroundColor, lastRegisteredBackgroundColor, StringComparison.Ordinal)
            || Math.Abs(PenWidth - lastRegisteredPenWidth) > double.Epsilon
            || Math.Abs(SmoothingDistance - lastRegisteredSmoothingDistance) > double.Epsilon
            || DisabledOrReadOnly != lastRegisteredReadOnly;
    }

    public async Task ClearAsync()
    {
        if (DisabledOrReadOnly)
        {
            return;
        }

        try
        {
            await JSRuntime.InvokeVoidAsync("xfSignaturePad.clear", CanvasId);
        }
        catch (JSException)
        {
            return;
        }

        await SetValueFromJsAsync(null);
    }

    public async Task UndoAsync()
    {
        if (DisabledOrReadOnly)
        {
            return;
        }

        try
        {
            var value = await JSRuntime.InvokeAsync<string?>("xfSignaturePad.undo", CanvasId);
            await SetValueFromJsAsync(value);
        }
        catch (JSException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (registered)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("xfSignaturePad.release", CanvasId);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        dotNetReference?.Dispose();
    }
}
