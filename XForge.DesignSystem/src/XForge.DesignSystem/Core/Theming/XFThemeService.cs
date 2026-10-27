using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace XForge.DesignSystem.Core.Theming
{
    /// <summary>
    /// Service for managing theme (light/dark) with OS auto-detection support.
    /// Register as singleton: services.AddSingleton&lt;XFThemeService&gt;();
    /// </summary>
    public class XFThemeService : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        private DotNetObjectReference<XFThemeService>? _dotNetRef;

        private XFTheme _currentTheme = XFTheme.Light;
        private XFThemeMode _mode = XFThemeMode.Auto;
        private string? _currentBrand;
        private bool _isInitialized;

        /// <summary>Fires when the theme changes.</summary>
        public event EventHandler<XFThemeChangedEventArgs>? ThemeChanged;

        public XFThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>Current active theme (resolved).</summary>
        public XFTheme CurrentTheme => _currentTheme;

        /// <summary>Current theme mode (Light/Dark/Auto).</summary>
        public XFThemeMode Mode => _mode;

        /// <summary>Current brand name (null = no brand).</summary>
        public string? CurrentBrand => _currentBrand;

        /// <summary>Whether dark mode is currently active.</summary>
        public bool IsDark => _currentTheme == XFTheme.Dark;

        /// <summary>Initializes the theme service and applies saved preference.</summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/XForge.DesignSystem/js/xf-theme.js");

                _dotNetRef = DotNetObjectReference.Create(this);

                // Load saved preference
                var savedMode = await _module.InvokeAsync<string?>("xfTheme.getMode");
                if (Enum.TryParse<XFThemeMode>(savedMode, true, out var mode))
                {
                    _mode = mode;
                }

                // Apply initial theme
                await ApplyThemeAsync();

                // Start listening for OS changes (if in Auto mode)
                if (_mode == XFThemeMode.Auto)
                {
                    await _module.InvokeVoidAsync("xfTheme.startListening", _dotNetRef);
                }

                _isInitialized = true;
            }
            catch (JSException)
            {
                // JS interop not available (e.g., during prerendering)
                _isInitialized = true;
            }
        }

        /// <summary>Sets the theme mode (Light/Dark/Auto).</summary>
        public async Task SetModeAsync(XFThemeMode mode)
        {
            _mode = mode;
            await ApplyThemeAsync();

            if (_module != null)
            {
                if (mode == XFThemeMode.Auto)
                    await _module.InvokeVoidAsync("xfTheme.startListening", _dotNetRef);
                else
                    await _module.InvokeVoidAsync("xfTheme.stopListening");

                // Save preference
                await _module.InvokeVoidAsync("xfTheme.setMode", mode.ToString().ToLowerInvariant());
            }
        }

        /// <summary>Sets the theme directly (Light/Dark).</summary>
        public async Task SetThemeAsync(XFTheme theme)
        {
            _currentTheme = theme;
            _mode = theme == XFTheme.Dark ? XFThemeMode.Dark : XFThemeMode.Light;
            await ApplyThemeAsync();
            if (_module != null)
                await _module.InvokeVoidAsync("xfTheme.setMode", _mode.ToString().ToLowerInvariant());
        }

        /// <summary>Toggles between light and dark.</summary>
        public async Task ToggleAsync()
        {
            var newTheme = _currentTheme == XFTheme.Dark ? XFTheme.Light : XFTheme.Dark;
            await SetThemeAsync(newTheme);
        }

        /// <summary>Sets the active brand (e.g., "brand-components"). Pass null to remove brand.</summary>
        public async Task SetBrandAsync(string? brand)
        {
            _currentBrand = brand;
            await ApplyBrandAsync();
            ThemeChanged?.Invoke(this, new XFThemeChangedEventArgs(_currentTheme, _mode) { Brand = brand });
        }

        /// <summary>Removes the active brand.</summary>
        public async Task ClearBrandAsync()
        {
            await SetBrandAsync(null);
        }

        /// <summary>JS-invokable callback for OS preference changes.</summary>
        [JSInvokable]
        public async Task OnSystemThemeChanged(bool prefersDark)
        {
            if (_mode != XFThemeMode.Auto) return;

            var newTheme = prefersDark ? XFTheme.Dark : XFTheme.Light;
            if (newTheme == _currentTheme) return;

            _currentTheme = newTheme;
            await ApplyThemeAsync();
            ThemeChanged?.Invoke(this, new XFThemeChangedEventArgs(_currentTheme, _mode));
        }

        private async Task ApplyThemeAsync()
        {
            var theme = _mode switch
            {
                XFThemeMode.Light => XFTheme.Light,
                XFThemeMode.Dark => XFTheme.Dark,
                _ => _currentTheme // Auto: keep resolved theme
            };

            _currentTheme = theme;
            var themeValue = theme == XFTheme.Dark ? "dark" : "light";

            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "eval", $"document.documentElement.setAttribute('data-theme', '{themeValue}')");
                await ApplyBrandAsync();
            }
            catch (JSException)
            {
                // Prerendering fallback
            }
        }

        private async Task ApplyBrandAsync()
        {
            try
            {
                if (_currentBrand != null)
                    await _jsRuntime.InvokeVoidAsync(
                        "eval", $"document.documentElement.setAttribute('data-brand', '{_currentBrand}')");
                else
                    await _jsRuntime.InvokeVoidAsync(
                        "eval", "document.documentElement.removeAttribute('data-brand')");
            }
            catch (JSException)
            {
                // Prerendering fallback
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.InvokeVoidAsync("xfTheme.stopListening");
                await _module.DisposeAsync();
            }
            _dotNetRef?.Dispose();
        }
    }

    public enum XFTheme { Light, Dark }
    public enum XFThemeMode { Light, Dark, Auto }

    public class XFThemeChangedEventArgs : EventArgs
    {
        public XFTheme Theme { get; }
        public XFThemeMode Mode { get; }
        public string? Brand { get; set; }

        public XFThemeChangedEventArgs(XFTheme theme, XFThemeMode mode)
        {
            Theme = theme;
            Mode = mode;
        }
    }
}
