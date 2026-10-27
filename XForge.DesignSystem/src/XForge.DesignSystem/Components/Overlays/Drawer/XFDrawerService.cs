using Microsoft.AspNetCore.Components;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Overlays.Drawer
{
    public sealed class XFDrawerService
    {
        internal event Func<XFDrawerRequest, Task>? Requested;

        public async Task ShowAsync(
            XFDrawerOptions options,
            RenderFragment<XFDrawerController>? childContent = null,
            RenderFragment<XFDrawerController>? footer = null)
        {
            var request = new XFDrawerRequest(options, childContent, footer);

            if (Requested is null)
            {
                return;
            }

            await Requested.Invoke(request);
            await request.Task;
        }
    }

    public sealed class XFDrawerOptions
    {
        public string? AriaLabel { get; set; }
        public string? AriaLabelledBy { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public XFDrawerPlacement Placement { get; set; } = XFDrawerPlacement.Right;
        public XFOverlaySize Size { get; set; } = XFOverlaySize.Md;
        public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        public bool Modal { get; set; } = true;
        public bool TrapFocus { get; set; } = true;
        public bool CloseOnEscape { get; set; } = true;
        public bool CloseOnOverlayClick { get; set; } = true;
        public bool ShowCloseButton { get; set; } = true;
        public string CloseLabel { get; set; } = "Fechar painel";
    }

    public sealed class XFDrawerController
    {
        private readonly Func<Task> close;

        internal XFDrawerController(Func<Task> close)
        {
            this.close = close;
        }

        public Task CloseAsync() => close();
    }

    internal sealed class XFDrawerRequest
    {
        private readonly TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public XFDrawerRequest(
            XFDrawerOptions options,
            RenderFragment<XFDrawerController>? childContent,
            RenderFragment<XFDrawerController>? footer)
        {
            Options = options;
            ChildContent = childContent;
            Footer = footer;
            Controller = new XFDrawerController(CompleteAsync);
        }

        public XFDrawerOptions Options { get; }
        public RenderFragment<XFDrawerController>? ChildContent { get; }
        public RenderFragment<XFDrawerController>? Footer { get; }
        public XFDrawerController Controller { get; }
        public Task Task => completion.Task;
        internal Func<Task>? Closed { get; set; }

        public async Task CompleteAsync()
        {
            completion.TrySetResult();

            if (Closed is not null)
            {
                await Closed.Invoke();
            }
        }
    }
}
