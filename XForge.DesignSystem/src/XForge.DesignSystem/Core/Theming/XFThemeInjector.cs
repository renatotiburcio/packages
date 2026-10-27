using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace XForge.DesignSystem.Core.Theming
{
    /// <summary>
    /// Injects CSS theme variables into the document head at runtime.
    /// Supports multiple brand registrations and dynamic switching.
    /// </summary>
    public class XFThemeInjector : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        private readonly Dictionary<string, string> _registeredBrands = new();
        private bool _isInitialized;

        public XFThemeInjector(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>Number of registered brands.</summary>
        public int BrandCount => _registeredBrands.Count;

        /// <summary>Initializes the injector module.</summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            try
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/XForge.DesignSystem/js/xf-theme.js");
                _isInitialized = true;
            }
            catch (JSException) { _isInitialized = true; }
        }

        /// <summary>Registers a brand's CSS (light variant). Overwrites if brand name exists.</summary>
        public async Task RegisterBrandAsync(string brandName, string css)
        {
            _registeredBrands[brandName] = css;
            await EnsureInitialized();
            if (_module != null)
                await _module.InvokeVoidAsync("xfTheme.injectBrandCss", brandName, css);
        }

        /// <summary>Registers a brand with both light and dark CSS variants.</summary>
        public async Task RegisterBrandAsync(string brandName, string lightCss, string darkCss)
        {
            _registeredBrands[brandName] = lightCss;
            await EnsureInitialized();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("xfTheme.injectBrandCss", brandName, lightCss);
                await _module.InvokeVoidAsync("xfTheme.injectDarkBrandCss", brandName, darkCss);
            }
        }

        /// <summary>Registers a brand from an XFThemeBuilder instance.</summary>
        public async Task RegisterBrandAsync(XFThemeBuilder builder)
        {
            var variables = builder.BuildVariables();
            var name = variables.TryGetValue("--_brand-name", out var n) ? n : "custom";
            var lightCss = builder.BuildBrandCss();
            var darkCss = builder.BuildDarkBrandCss();
            await RegisterBrandAsync(name, lightCss, darkCss);
        }

        /// <summary>Removes a registered brand's injected CSS.</summary>
        public async Task UnregisterBrandAsync(string brandName)
        {
            _registeredBrands.Remove(brandName);
            await EnsureInitialized();
            if (_module != null)
                await _module.InvokeVoidAsync("xfTheme.removeBrandCss", brandName);
        }

        /// <summary>Injects arbitrary CSS into the head with a given id (for deduplication).</summary>
        public async Task InjectCssAsync(string styleId, string css)
        {
            await EnsureInitialized();
            if (_module != null)
                await _module.InvokeVoidAsync("xfTheme.injectCss", styleId, css);
        }

        /// <summary>Removes previously injected CSS by id.</summary>
        public async Task RemoveCssAsync(string styleId)
        {
            await EnsureInitialized();
            if (_module != null)
                await _module.InvokeVoidAsync("xfTheme.removeCss", styleId);
        }

        /// <summary>Checks if a brand is registered.</summary>
        public bool IsBrandRegistered(string brandName) => _registeredBrands.ContainsKey(brandName);

        /// <summary>Returns all registered brand names.</summary>
        public IReadOnlyCollection<string> GetRegisteredBrands() => _registeredBrands.Keys.ToList().AsReadOnly();

        private async Task EnsureInitialized()
        {
            if (!_isInitialized) await InitializeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
