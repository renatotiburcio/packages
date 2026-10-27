using Microsoft.AspNetCore.Components;

namespace XForge.DesignSystem.Components.Utilities.BreakpointObserver
{
    public partial class XFBreakpointObserver : IAsyncDisposable
    {
        [Parameter] public RenderFragment<XFBreakpoint>? ChildContent { get; set; }
        [Parameter] public EventCallback<XFBreakpoint> BreakpointChanged { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                BreakpointService.BreakpointChanged += HandleBreakpointChanged;
                await BreakpointService.InitializeAsync();
            }
        }

        private void HandleBreakpointChanged(XFBreakpoint breakpoint)
        {
            InvokeAsync(async () =>
            {
                if (BreakpointChanged.HasDelegate)
                {
                    await BreakpointChanged.InvokeAsync(breakpoint);
                }
                StateHasChanged();
            });
        }

        public async ValueTask DisposeAsync()
        {
            BreakpointService.BreakpointChanged -= HandleBreakpointChanged;
            await ValueTask.CompletedTask;
        }
    }
}
