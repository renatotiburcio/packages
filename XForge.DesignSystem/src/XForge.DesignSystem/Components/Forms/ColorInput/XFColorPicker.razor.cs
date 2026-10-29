using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.ColorInput
{
    public partial class XFColorPicker : XForgeComponentBase, IAsyncDisposable
    {
        private const string FallbackColor = "#000000";
        private static readonly Regex HexColorRegex = new("^#?[0-9a-fA-F]{6}$", RegexOptions.Compiled);
        private static readonly Regex HexColorWithAlphaRegex = new("^#?[0-9a-fA-F]{8}$", RegexOptions.Compiled);

        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public string? Label { get; set; }
        [Parameter] public string? AriaLabel { get; set; }
        [Parameter] public string? HelperText { get; set; }
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }
        [Parameter] public EventCallback<string?> Change { get; set; }
        [Parameter] public bool Immediate { get; set; } = true;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public bool FullWidth { get; set; } = true;
        [Parameter] public bool ShowRgb { get; set; } = true;
        [Parameter] public bool ShowColorModels { get; set; } = true;
        [Parameter] public bool ShowAlpha { get; set; }
        [Parameter] public bool ShowHsvPicker { get; set; } = true;
        [Parameter] public bool ShowPickerTab { get; set; } = true;
        [Parameter] public IReadOnlyList<string> Palette { get; set; } = Array.Empty<string>();
        [Parameter] public IReadOnlyList<IReadOnlyList<string>>? ThemePalette { get; set; }
        [Parameter] public bool OpenOnFocus { get; set; }
        [Parameter] public string PaletteLabel { get; set; } = "Color palette";
        [Parameter] public string NativeInputLabel { get; set; } = "Open color picker";
        [Parameter] public string ToggleLabel { get; set; } = "Open color palette";
        [Parameter] public string DialogLabel { get; set; } = "Color picker";
        [Parameter] public string ThemeColorTitle { get; set; } = "Theme color";
        [Parameter] public string ThemeScaleLabel { get; set; } = "Theme color scale";
        [Parameter] public string CustomColorsTitle { get; set; } = "My colors";
        [Parameter] public string AddColorText { get; set; } = "Add";
        [Parameter] public string HexLabel { get; set; } = "Hex";
        [Parameter] public string AlphaLabel { get; set; } = "Alpha";
        [Parameter] public string CancelText { get; set; } = "Cancel";
        [Parameter] public string ApplyText { get; set; } = "Apply";
        [Parameter] public string RgbUnavailableText { get; set; } = "RGB unavailable";
        [Parameter] public string RgbaUnavailableText { get; set; } = "RGBA unavailable";
        [Parameter] public string ColorModelUnavailableText { get; set; } = "unavailable";
        [Parameter] public string SwatchLabelFormat { get; set; } = "Select {0}";
        [Parameter] public string SvPickerLabel { get; set; } = "Saturation and brightness picker";
        [Parameter] public string HueSliderLabel { get; set; } = "Hue slider";
        [Parameter] public string PickerTabLabel { get; set; } = "Picker";
        [Parameter] public string PaletteTabLabel { get; set; } = "Palette";
        [Parameter] public string DetailsTabLabel { get; set; } = "Details";
        [Parameter] public string TabBarLabel { get; set; } = "Color picker sections";
        [Parameter] public string NativePickerTitle { get; set; } = "Native picker";

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('\u0001', Label, AriaLabel, HelperText, Value, Immediate, ReadOnly, Disabled);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        private string? CurrentValue { get; set; }
        private string? LastParameterValue { get; set; }
        private string? DraftText { get; set; }
        private string? DraftValue { get; set; }
        private bool IsOpen { get; set; }
        private double _hsvHue;
        private double _hsvSaturation;
        private double _hsvValue;
        private int _rgbR;
        private int _rgbG;
        private int _rgbB;
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<XFColorPicker>? _dotNetRef;
        private ElementReference _svAreaRef;
        private ElementReference _hueSliderRef;
        private int _activeTab;
        private bool _needsDragInit;

        private string TextInputId => $"{Id}-hex";
        private string NativeInputId => $"{Id}-native";
        private string HelperId => $"{Id}-helper";
        private string PaletteId => $"{Id}-palette";
        private string AriaInvalid => TextValue.Length > 0 && NormalizeColor(TextValue, ShowAlpha) is null ? "true" : "false";
        private string NativeValue => ToNativeColor(CurrentValue) ?? FallbackColor;
        private string DraftNativeValue => ToNativeColor(DraftValue) ?? FallbackColor;
        private string TextValue => DraftText ?? CurrentValue ?? string.Empty;
        private string DraftTextValue => DraftText ?? DraftValue ?? string.Empty;
        private string TextPlaceholder => ShowAlpha ? "#RRGGBBAA" : "#RRGGBB";
        private int TextMaxLength => ShowAlpha ? 9 : 7;
        private int AlphaPercent => GetAlphaPercent(DraftValue);
        private IReadOnlyList<IReadOnlyList<string>> ThemePaletteRows => ThemePalette ?? DefaultThemePalette;
        private string CommittedPreviewStyle => string.Create(CultureInfo.InvariantCulture, $"background-color: {ToCssColor(CurrentValue ?? FallbackColor)};");
        private string DraftPreviewStyle => string.Create(CultureInfo.InvariantCulture, $"background-color: {ToCssColor(DraftValue ?? FallbackColor)};");
        private bool ToggleDisabled => Disabled || ReadOnly;
        private bool ApplyDisabled => NormalizeColor(DraftTextValue, ShowAlpha) is null && !string.IsNullOrWhiteSpace(DraftTextValue);
        private bool ShowDetailsTab => ShowRgb || ShowColorModels;
        private bool HasMultipleTabs => (ShowPickerTab ? 1 : 0) + 1 + (ShowDetailsTab ? 1 : 0) > 1;
        private string? DescribedBy => string.Join(" ", new[] { HelperText is null ? null : HelperId, Palette.Count > 0 ? PaletteId : null }.Where(value => !string.IsNullOrWhiteSpace(value)));

        private string RootClass => Join("xf-color-picker flex flex-col gap-1.5", FullWidth ? "w-full" : "inline-flex", Class);
        private string LabelClass => "flex items-center gap-1 text-sm font-medium leading-5 text-text-primary";
        private string PickerShellClass => "relative";
        private string FieldShellClass => "group relative flex h-10 items-center gap-2 rounded-lg border border-surface-200 bg-bg-surface py-2 pl-3.5 pr-1.5 shadow-[0_1px_2px_rgb(15_23_42/.035)] ring-1 ring-surface-200/80 transition-all duration-200 ease-out hover:border-[color-mix(in_oklab,var(--color-primary)_45%,transparent)] focus-within:border-[var(--color-primary)] focus-within:ring-2 focus-within:ring-[color-mix(in_oklab,var(--color-primary)_18%,transparent)]";
        private string PopoverClass => "absolute left-0 top-full z-50 mt-2 w-[min(25rem,calc(100vw-2rem))] rounded-xl border border-surface-200 bg-bg-surface p-4 shadow-[0_18px_40px_rgb(15_23_42/.14)] ring-1 ring-surface-200/80";
        private string NativeInputClass => "h-10 w-10 shrink-0 cursor-pointer appearance-none overflow-hidden rounded-md border border-surface-200 bg-transparent p-0 outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] disabled:cursor-not-allowed disabled:opacity-60 [&::-moz-color-swatch]:border-0 [&::-moz-color-swatch]:rounded-md [&::-webkit-color-swatch]:border-0 [&::-webkit-color-swatch]:rounded-md [&::-webkit-color-swatch-wrapper]:p-0";
        private string TextInputClass => "min-w-0 flex-1 border-0 bg-transparent p-0 font-mono text-sm text-text-primary outline-none placeholder:text-text-muted disabled:cursor-not-allowed disabled:text-text-muted";
        private string IconButtonClass => "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-md bg-transparent text-primary outline-none ring-1 ring-transparent transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] hover:ring-[color-mix(in_oklab,var(--color-primary)_20%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] active:scale-95 disabled:pointer-events-none disabled:text-text-muted";
        private string FooterButtonClass => "inline-flex h-9 items-center rounded-md px-3 text-sm font-semibold text-text-secondary outline-none transition-all duration-150 hover:bg-surface-100 hover:text-text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        private string ApplyButtonClass => "inline-flex h-9 items-center rounded-md bg-primary px-3 text-sm font-semibold text-white shadow-[0_1px_2px_rgb(15_23_42/.12)] outline-none transition-all duration-150 hover:bg-primary/90 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_28%,transparent)] disabled:pointer-events-none disabled:opacity-50";
        private string PaletteClass => "grid grid-cols-8 gap-1.5 sm:grid-cols-10";
        private string HelperClass => "text-xs leading-5 text-text-secondary";
        private string SvAreaStyle => string.Create(CultureInfo.InvariantCulture, $"background-color: hsl({_hsvHue:0}, 100%, 50%)");
        private string SvHandleStyle => string.Create(CultureInfo.InvariantCulture, $"left: {_hsvSaturation * 100:0.##}%; top: {(1 - _hsvValue) * 100:0.##}%");
        private string HueHandleStyle => string.Create(CultureInfo.InvariantCulture, $"left: {_hsvHue / 360 * 100:0.##}%");
        private string RgbInputClass => "w-full rounded-md border border-surface-200 bg-bg-surface px-2 py-1.5 font-mono text-xs text-text-primary outline-none transition-all duration-150 focus:border-[var(--color-primary)] focus:ring-1 focus:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)] disabled:cursor-not-allowed disabled:text-text-muted";

        private static readonly IReadOnlyList<IReadOnlyList<string>> DefaultThemePalette =
        [
            ["#FFFFFF", "#F3F4F6", "#D1D5DB", "#9CA3AF", "#4B5563", "#374151", "#27272A", "#3F1D2B", "#4A102A", "#3B071E"],
            ["#ECFEFF", "#CFFAFE", "#A5F3FC", "#67E8F9", "#22D3EE", "#0EA5E9", "#0284C7", "#0369A1", "#075985", "#0C4A6E"],
            ["#FEF9C3", "#FEF08A", "#FDE047", "#FACC15", "#EAB308", "#CA8A04", "#A16207", "#854D0E", "#713F12", "#431407"],
            ["#FFE4E6", "#FDA4AF", "#FB7185", "#F43F5E", "#E11D48", "#BE123C", "#9F1239", "#881337", "#701A75", "#4A044E"],
            ["#DCFCE7", "#BBF7D0", "#86EFAC", "#4ADE80", "#22C55E", "#16A34A", "#15803D", "#166534", "#14532D", "#052E16"]
        ];

        private string ColorReadoutText
        {
            get
            {
                var color = NormalizeColor(DraftTextValue, ShowAlpha) ?? DraftValue;
                if (color is null)
                {
                    return ShowAlpha ? RgbaUnavailableText : RgbUnavailableText;
                }

                var r = Convert.ToInt32(color.Substring(1, 2), 16);
                var g = Convert.ToInt32(color.Substring(3, 2), 16);
                var b = Convert.ToInt32(color.Substring(5, 2), 16);
                if (!ShowAlpha)
                {
                    return string.Create(CultureInfo.InvariantCulture, $"RGB {r}, {g}, {b}");
                }

                var alpha = GetAlphaPercent(color);
                return string.Create(CultureInfo.InvariantCulture, $"RGBA {r}, {g}, {b}, {alpha}%");
            }
        }

        private string HslText => TryGetRgb(DraftTextValue, out var r, out var g, out var b)
            ? FormatHsl(r, g, b)
            : ColorModelUnavailableText;

        private string HsvText => TryGetRgb(DraftTextValue, out var r, out var g, out var b)
            ? FormatHsv(r, g, b)
            : ColorModelUnavailableText;

        protected override void OnParametersSet()
        {
            var normalized = NormalizeColor(Value, ShowAlpha);
            if (normalized != LastParameterValue)
            {
                CurrentValue = normalized;
                DraftText = normalized;
                DraftValue = normalized;
                LastParameterValue = normalized;
                SyncHsvFromHex(normalized);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                        "import", "/_content/XForge.DesignSystem/js/xf-color-picker.js");
                }
                catch
                {
                    // JS interop not available (e.g., bUnit)
                }
            }

            if (_needsDragInit && _jsModule is not null)
            {
                _needsDragInit = false;
                try
                {
                    _dotNetRef ??= DotNetObjectReference.Create(this);
                    await _jsModule.InvokeVoidAsync("xfColorPicker.initBySelector", Id, _dotNetRef);
                }
                catch
                {
                    // JS interop not available (e.g., bUnit)
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _dotNetRef?.Dispose();
            _dotNetRef = null;
            if (_jsModule is not null)
            {
                try
                {
                    await _jsModule.DisposeAsync();
                }
                catch
                {
                    // JS module disposal failed
                }
            }
        }

        private async Task HandleFocusAsync()
        {
            if (OpenOnFocus && !ToggleDisabled)
            {
                await OpenAsync();
            }
        }

        private async Task ToggleAsync()
        {
            if (!ToggleDisabled)
            {
                if (IsOpen)
                {
                    await CancelAsync();
                }
                else
                {
                    await OpenAsync();
                }
            }
        }

        private async Task HandleRootKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Escape")
            {
                await CancelAsync();
                return;
            }
            else if ((args.Key == "ArrowDown" || args.Key == "Enter") && !IsOpen && !ToggleDisabled)
            {
                await OpenAsync();
            }
        }

        private Task OpenAsync()
        {
            if (IsOpen)
            {
                return Task.CompletedTask;
            }

            DraftValue = CurrentValue;
            DraftText = CurrentValue;
            IsOpen = true;
            _activeTab = ShowPickerTab ? 0 : 1;
            SyncHsvFromHex(DraftValue);
            _needsDragInit = ShowPickerTab;
            return Task.CompletedTask;
        }

        private Task SetTabAsync(int tab)
        {
            _activeTab = tab;
            if (tab == 0 && ShowHsvPicker)
            {
                _needsDragInit = true;
            }
            return Task.CompletedTask;
        }

        private string GetTabClass(int tab)
        {
            return _activeTab == tab
                ? "flex-1 rounded-md bg-bg-surface px-3 py-1.5 text-xs font-semibold text-text-primary shadow-sm outline-none transition-all duration-150 focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]"
                : "flex-1 rounded-md px-3 py-1.5 text-xs font-medium text-text-secondary outline-none transition-all duration-150 hover:text-text-primary focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]";
        }

        private string GetAriaSelected(int tab) => _activeTab == tab ? "true" : "false";


        private Task HandleNativeInputAsync(ChangeEventArgs args)
        {
            return SetDraftFromStringAsync(ApplyCurrentAlpha(args.Value?.ToString()));
        }

        private Task HandleNativeChangeAsync(ChangeEventArgs args)
        {
            return SetDraftFromStringAsync(ApplyCurrentAlpha(args.Value?.ToString()));
        }

        private async Task HandleTextInputAsync(ChangeEventArgs args)
        {
            await OpenAsync();
            DraftText = args.Value?.ToString();
            await SetDraftFromStringAsync(DraftText);
        }

        private async Task HandleTextChangeAsync(ChangeEventArgs args)
        {
            await OpenAsync();
            DraftText = args.Value?.ToString();
            await SetDraftFromStringAsync(DraftText);
        }

        private Task SelectSwatchAsync(string color)
        {
            return SetDraftFromStringAsync(ApplyCurrentAlpha(color));
        }

        private Task HandleAlphaInputAsync(ChangeEventArgs args)
        {
            return SetAlphaAsync(args.Value?.ToString());
        }

        private Task HandleAlphaChangeAsync(ChangeEventArgs args)
        {
            return SetAlphaAsync(args.Value?.ToString());
        }

        private Task SetDraftFromStringAsync(string? value)
        {
            var normalized = NormalizeColor(value, ShowAlpha);
            if (normalized is not null || string.IsNullOrWhiteSpace(value))
            {
                DraftValue = normalized;
                DraftText = normalized;
                if (ShowHsvPicker)
                {
                    SyncHsvFromHex(normalized);
                }
            }

            return Task.CompletedTask;
        }

        private Task SetAlphaAsync(string? value)
        {
            var alpha = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? Math.Clamp(parsed, 0, 100)
                : 100;

            var baseColor = ToNativeColor(DraftValue) ?? FallbackColor;
            var alphaHex = PercentToAlphaHex(alpha);
            return SetDraftFromStringAsync($"{baseColor}{alphaHex}");
        }

        private Task ApplyAsync()
        {
            var normalized = NormalizeColor(DraftTextValue, ShowAlpha);
            if (normalized is null && !string.IsNullOrWhiteSpace(DraftTextValue))
            {
                return Task.CompletedTask;
            }

            return SetValueAsync(normalized, notifyChange: true, close: true);
        }

        private Task CancelAsync()
        {
            DraftValue = CurrentValue;
            DraftText = CurrentValue;
            IsOpen = false;
            _activeTab = 0;
            return Task.CompletedTask;
        }

        private Task HandleRedChange(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var r))
            {
                _rgbR = Math.Clamp(r, 0, 255);
                UpdateDraftFromRgb();
            }

            return Task.CompletedTask;
        }

        private Task HandleGreenChange(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var g))
            {
                _rgbG = Math.Clamp(g, 0, 255);
                UpdateDraftFromRgb();
            }

            return Task.CompletedTask;
        }

        private Task HandleBlueChange(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var b))
            {
                _rgbB = Math.Clamp(b, 0, 255);
                UpdateDraftFromRgb();
            }

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task OnSvDrag(double saturation, double value)
        {
            _hsvSaturation = Math.Clamp(saturation, 0, 1);
            _hsvValue = Math.Clamp(value, 0, 1);
            UpdateDraftFromHsv();
            StateHasChanged();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task OnHueDrag(double hue)
        {
            _hsvHue = ((hue % 360) + 360) % 360;
            UpdateDraftFromHsv();
            StateHasChanged();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task OnSvDragEnd()
        {
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task OnHueDragEnd()
        {
            return Task.CompletedTask;
        }

        private void UpdateDraftFromHsv()
        {
            var (r, g, b) = HsvToRgb(_hsvHue, _hsvSaturation, _hsvValue);
            _rgbR = r;
            _rgbG = g;
            _rgbB = b;
            var hex = string.Create(CultureInfo.InvariantCulture, $"#{r:X2}{g:X2}{b:X2}");
            DraftValue = ApplyCurrentAlpha(hex);
            DraftText = DraftValue;
        }

        private void UpdateDraftFromRgb()
        {
            var (h, s, v) = RgbToHsv(_rgbR, _rgbG, _rgbB);
            _hsvHue = h;
            _hsvSaturation = s;
            _hsvValue = v;
            var hex = string.Create(CultureInfo.InvariantCulture, $"#{_rgbR:X2}{_rgbG:X2}{_rgbB:X2}");
            DraftValue = ApplyCurrentAlpha(hex);
            DraftText = DraftValue;
        }

        private void SyncHsvFromHex(string? hex)
        {
            var normalized = NormalizeColor(hex, allowAlpha: true);
            if (normalized is not null && normalized.Length >= 7)
            {
                var r = Convert.ToInt32(normalized.Substring(1, 2), 16);
                var g = Convert.ToInt32(normalized.Substring(3, 2), 16);
                var b = Convert.ToInt32(normalized.Substring(5, 2), 16);
                var (h, s, v) = RgbToHsv(r, g, b);
                _hsvHue = h;
                _hsvSaturation = s;
                _hsvValue = v;
                _rgbR = r;
                _rgbG = g;
                _rgbB = b;
            }
            else
            {
                _hsvHue = 0;
                _hsvSaturation = 0;
                _hsvValue = 0;
                _rgbR = 0;
                _rgbG = 0;
                _rgbB = 0;
            }
        }

        private async Task SetValueAsync(string? value, bool notifyChange, bool close)
        {
            CurrentValue = value;
            DraftValue = value;
            DraftText = value;
            Value = value;
            LastParameterValue = value;

            if (close)
            {
                IsOpen = false;
            }

            if (notifyChange && ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (notifyChange && Change.HasDelegate)
            {
                await Change.InvokeAsync(Value);
            }
        }

        private string GetSwatchClass(string color)
        {
            return Join(
                "aspect-square rounded-full border border-surface-200 shadow-[inset_0_0_0_1px_rgb(255_255_255/.35)] outline-none ring-1 ring-transparent transition-all duration-150 hover:-translate-y-0.5 hover:ring-[color-mix(in_oklab,var(--color-primary)_25%,transparent)] focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_35%,transparent)] disabled:cursor-not-allowed disabled:opacity-60",
                NormalizeColor(ApplyCurrentAlpha(color), ShowAlpha) == DraftValue ? "ring-2 ring-primary ring-offset-2 ring-offset-bg-surface" : null);
        }

        private string GetThemeSwatchClass(string color)
        {
            return Join(
                "h-6 border-0 outline-none ring-0 transition-transform duration-150 hover:scale-y-110 focus-visible:z-10 focus-visible:ring-2 focus-visible:ring-primary disabled:cursor-not-allowed disabled:opacity-60",
                NormalizeColor(ApplyCurrentAlpha(color), ShowAlpha) == DraftValue ? "z-10 scale-y-110 ring-2 ring-primary" : null);
        }

        private static string? NormalizeColor(string? value, bool allowAlpha)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            if (HexColorRegex.IsMatch(trimmed))
            {
                return (trimmed.StartsWith("#", StringComparison.Ordinal) ? trimmed : $"#{trimmed}").ToUpperInvariant();
            }

            if (!allowAlpha || !HexColorWithAlphaRegex.IsMatch(trimmed))
            {
                return null;
            }

            return (trimmed.StartsWith("#", StringComparison.Ordinal) ? trimmed : $"#{trimmed}").ToUpperInvariant();
        }

        private string GetSwatchStyle(string color)
        {
            var normalized = NormalizeColor(ApplyCurrentAlpha(color), ShowAlpha) ?? FallbackColor;
            return string.Create(CultureInfo.InvariantCulture, $"background-color: {ToCssColor(normalized)};");
        }

        private string GetSwatchLabel(string color)
        {
            return string.Format(CultureInfo.CurrentCulture, SwatchLabelFormat, NormalizeColor(ApplyCurrentAlpha(color), ShowAlpha) ?? color);
        }

        private string? ApplyCurrentAlpha(string? color)
        {
            var normalized = NormalizeColor(color, allowAlpha: true);
            if (normalized is null || !ShowAlpha)
            {
                return color;
            }

            if (normalized.Length == 9)
            {
                return normalized;
            }

            var alpha = DraftValue?.Length == 9 ? DraftValue[7..9] : "FF";
            return $"{normalized}{alpha}";
        }

        private static string? ToNativeColor(string? color)
        {
            var normalized = NormalizeColor(color, allowAlpha: true);
            return normalized is null ? null : normalized[..7];
        }

        private static string ToCssColor(string color)
        {
            var normalized = NormalizeColor(color, allowAlpha: true) ?? FallbackColor;
            if (normalized.Length == 7)
            {
                return normalized;
            }

            var r = Convert.ToInt32(normalized.Substring(1, 2), 16);
            var g = Convert.ToInt32(normalized.Substring(3, 2), 16);
            var b = Convert.ToInt32(normalized.Substring(5, 2), 16);
            var alpha = Convert.ToInt32(normalized.Substring(7, 2), 16) / 255d;
            return string.Create(CultureInfo.InvariantCulture, $"rgba({r}, {g}, {b}, {alpha:0.###})");
        }

        private static bool TryGetRgb(string? color, out int r, out int g, out int b)
        {
            r = 0;
            g = 0;
            b = 0;

            var normalized = NormalizeColor(color, allowAlpha: true);
            if (normalized is null)
            {
                return false;
            }

            r = Convert.ToInt32(normalized.Substring(1, 2), 16);
            g = Convert.ToInt32(normalized.Substring(3, 2), 16);
            b = Convert.ToInt32(normalized.Substring(5, 2), 16);
            return true;
        }

        private static string FormatHsl(int r, int g, int b)
        {
            var (hue, saturation, lightness, _) = ConvertRgbToHslHsv(r, g, b);
            return string.Create(CultureInfo.InvariantCulture, $"{hue:0}deg {saturation:0}% {lightness:0}%");
        }

        private static string FormatHsv(int r, int g, int b)
        {
            var (hue, _, _, value, saturationValue) = ConvertRgbToHsv(r, g, b);
            return string.Create(CultureInfo.InvariantCulture, $"{hue:0}deg {saturationValue:0}% {value:0}%");
        }

        private static (double Hue, double Saturation, double Lightness, double Value) ConvertRgbToHslHsv(int r, int g, int b)
        {
            var rf = r / 255d;
            var gf = g / 255d;
            var bf = b / 255d;
            var max = Math.Max(rf, Math.Max(gf, bf));
            var min = Math.Min(rf, Math.Min(gf, bf));
            var delta = max - min;

            var hue = delta == 0 ? 0 :
                max == rf ? 60 * (((gf - bf) / delta) % 6) :
                max == gf ? 60 * (((bf - rf) / delta) + 2) :
                60 * (((rf - gf) / delta) + 4);

            if (hue < 0)
            {
                hue += 360;
            }

            var lightness = (max + min) / 2;
            var saturation = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * lightness - 1));

            return (hue, saturation * 100, lightness * 100, max * 100);
        }

        private static (double Hue, double Saturation, double Lightness, double Value, double SaturationValue) ConvertRgbToHsv(int r, int g, int b)
        {
            var hsl = ConvertRgbToHslHsv(r, g, b);
            var rf = r / 255d;
            var gf = g / 255d;
            var bf = b / 255d;
            var max = Math.Max(rf, Math.Max(gf, bf));
            var min = Math.Min(rf, Math.Min(gf, bf));
            var delta = max - min;
            var saturationValue = max == 0 ? 0 : delta / max * 100;
            return (hsl.Hue, hsl.Saturation, hsl.Lightness, hsl.Value, saturationValue);
        }

        public static (double Hue, double Saturation, double Value) RgbToHsv(int r, int g, int b)
        {
            var rf = r / 255d;
            var gf = g / 255d;
            var bf = b / 255d;
            var max = Math.Max(rf, Math.Max(gf, bf));
            var min = Math.Min(rf, Math.Min(gf, bf));
            var delta = max - min;

            var hue = delta == 0 ? 0 :
                max == rf ? 60 * (((gf - bf) / delta) % 6) :
                max == gf ? 60 * (((bf - rf) / delta) + 2) :
                60 * (((rf - gf) / delta) + 4);

            if (hue < 0)
            {
                hue += 360;
            }

            var saturation = max == 0 ? 0 : delta / max;
            return (hue, saturation, max);
        }

        public static (int R, int G, int B) HsvToRgb(double h, double s, double v)
        {
            var c = v * s;
            var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            var m = v - c;
            double r1, g1, b1;

            if (h < 60) { r1 = c; g1 = x; b1 = 0; }
            else if (h < 120) { r1 = x; g1 = c; b1 = 0; }
            else if (h < 180) { r1 = 0; g1 = c; b1 = x; }
            else if (h < 240) { r1 = 0; g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = x; }

            return (
                (int)Math.Round((r1 + m) * 255),
                (int)Math.Round((g1 + m) * 255),
                (int)Math.Round((b1 + m) * 255));
        }

        private static int GetAlphaPercent(string? color)
        {
            var normalized = NormalizeColor(color, allowAlpha: true);
            if (normalized?.Length != 9)
            {
                return 100;
            }

            var alpha = Convert.ToInt32(normalized.Substring(7, 2), 16);
            return (int)Math.Round(alpha / 255d * 100);
        }

        private static string PercentToAlphaHex(int percent)
        {
            var alpha = (int)Math.Round(Math.Clamp(percent, 0, 100) / 100d * 255);
            return alpha.ToString("X2", CultureInfo.InvariantCulture);
        }

        private static string Join(params string?[] values) => string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }
}
