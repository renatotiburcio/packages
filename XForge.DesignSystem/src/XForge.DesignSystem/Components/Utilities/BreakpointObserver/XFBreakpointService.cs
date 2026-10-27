using Microsoft.JSInterop;

namespace XForge.DesignSystem.Components.Utilities.BreakpointObserver
{
    public class XFBreakpointService : IAsyncDisposable, IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<XFBreakpointService>? _dotNetRef;
        private IJSObjectReference? _module;
        private XFBreakpoint _currentBreakpoint = XFBreakpoint.Xs;
        private bool _initialized;

        public event Action<XFBreakpoint>? BreakpointChanged;

        public XFBreakpoint CurrentBreakpoint => _currentBreakpoint;

        public XFBreakpointService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }

            try
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/XForge.DesignSystem/js/xf-breakpoint.js");
                await _module.InvokeVoidAsync("initialize", _dotNetRef);
                _initialized = true;
            }
            catch (InvalidOperationException)
            {
                // JS not available (prerender)
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected
            }
        }

        [JSInvokable]
        public void OnBreakpointChanged(string breakpoint)
        {
            if (Enum.TryParse<XFBreakpoint>(breakpoint, ignoreCase: true, out var parsed))
            {
                _currentBreakpoint = parsed;
                BreakpointChanged?.Invoke(parsed);
            }
        }

        public void Dispose()
        {
            _dotNetRef?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_module is not null)
                {
                    await _module.InvokeVoidAsync("dispose");
                    await _module.DisposeAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // JS not available
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected
            }

            _dotNetRef?.Dispose();
        }
    }
}
