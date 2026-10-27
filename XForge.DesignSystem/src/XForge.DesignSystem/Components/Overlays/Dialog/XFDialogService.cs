using Microsoft.AspNetCore.Components;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Components.Overlays.Dialog
{
    public sealed class XFDialogService
    {
        internal event Func<XFDialogRequest, Task>? Requested;

        public async Task<XFDialogResult> ShowAsync(
            XFDialogOptions options,
            RenderFragment<XFDialogController>? childContent = null,
            RenderFragment<XFDialogController>? footer = null)
        {
            var request = new XFDialogRequest(options, childContent, footer);

            if (Requested is null)
            {
                return XFDialogResult.CancelledResult;
            }

            await Requested.Invoke(request);
            return await request.Task;
        }

        public Task<XFDialogResult> ConfirmAsync(
            string title,
            string? description = null,
            string confirmText = "Confirmar",
            string cancelText = "Cancelar",
            XFSemanticColor tone = XFSemanticColor.Danger,
            Func<Task<object?>>? onConfirm = null,
            string busyText = "Processando...",
            string defaultDescription = "Confirme para continuar.")
        {
            var options = new XFDialogOptions
            {
                Title = title,
                Description = description,
                Tone = tone,
                Alert = tone == XFSemanticColor.Danger,
                CloseOnOverlayClick = false
            };

            return ShowAsync(
                options,
                _ => builder =>
                {
                    builder.OpenElement(0, "p");
                    builder.AddContent(1, description ?? defaultDescription);
                    builder.CloseElement();
                },
                controller => builder =>
                {
                    builder.OpenElement(0, "button");
                    builder.AddAttribute(1, "type", "button");
                    builder.AddAttribute(2, "class", "inline-flex h-9 items-center justify-center rounded-lg border border-surface-200 bg-bg-surface px-3 text-sm font-semibold text-text-secondary transition-all duration-150 hover:bg-surface-100 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-primary)_24%,transparent)]");
                    builder.AddAttribute(3, "onclick", EventCallback.Factory.Create(controller, controller.CancelAsync));
                    builder.AddContent(4, cancelText);
                    builder.CloseElement();

                    builder.OpenElement(5, "button");
                    builder.AddAttribute(6, "type", "button");
                    builder.AddAttribute(7, "disabled", controller.IsBusy);
                    builder.AddAttribute(8, "class", "inline-flex h-9 items-center justify-center rounded-lg bg-[var(--color-danger)] px-3 text-sm font-semibold text-white transition-all duration-150 hover:bg-[color-mix(in_oklab,var(--color-danger)_88%,black)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[color-mix(in_oklab,var(--color-danger)_24%,transparent)] disabled:cursor-wait disabled:opacity-75");
                    builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(controller, () => onConfirm is null
                        ? controller.CloseAsync(true)
                        : controller.ExecuteAsync(onConfirm)));
                    builder.AddContent(10, controller.IsBusy ? busyText : confirmText);
                    builder.CloseElement();
                });
        }
    }

    public sealed class XFDialogOptions
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public XFOverlaySize Size { get; set; } = XFOverlaySize.Md;
        public XFSemanticColor Tone { get; set; } = XFSemanticColor.Default;
        public XFDialogPlacement Placement { get; set; } = XFDialogPlacement.Center;
        public bool Alert { get; set; }
        public bool CloseOnEscape { get; set; } = true;
        public bool CloseOnOverlayClick { get; set; } = true;
        public bool ShowCloseButton { get; set; } = true;
    }

    public sealed class XFDialogResult
    {
        public static XFDialogResult CancelledResult { get; } = new(true, null);

        public XFDialogResult(bool cancelled, object? value)
        {
            Cancelled = cancelled;
            Value = value;
        }

        public bool Cancelled { get; }
        public object? Value { get; }

        public T? GetValue<T>() => Value is T typed ? typed : default;
    }

    public sealed class XFDialogController
    {
        private readonly Func<object?, bool, Task> complete;
        private readonly Func<bool, Task> busyChanged;

        internal XFDialogController(Func<object?, bool, Task> complete, Func<bool, Task> busyChanged)
        {
            this.complete = complete;
            this.busyChanged = busyChanged;
        }

        public bool IsBusy { get; private set; }

        public Task CloseAsync(object? value = null) => complete(value, false);

        public Task CancelAsync() => complete(null, true);

        public async Task ExecuteAsync(Func<Task<object?>> action)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            await busyChanged.Invoke(true);

            try
            {
                var value = await action.Invoke();
                await CloseAsync(value ?? true);
            }
            finally
            {
                if (IsBusy)
                {
                    IsBusy = false;
                    await busyChanged.Invoke(false);
                }
            }
        }
    }

    internal sealed class XFDialogRequest
    {
        private readonly TaskCompletionSource<XFDialogResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public XFDialogRequest(
            XFDialogOptions options,
            RenderFragment<XFDialogController>? childContent,
            RenderFragment<XFDialogController>? footer)
        {
            Options = options;
            ChildContent = childContent;
            Footer = footer;
            Controller = new XFDialogController(CompleteAsync, SetBusyAsync);
        }

        public XFDialogOptions Options { get; }
        public RenderFragment<XFDialogController>? ChildContent { get; }
        public RenderFragment<XFDialogController>? Footer { get; }
        public XFDialogController Controller { get; }
        public Task<XFDialogResult> Task => completion.Task;
        internal Func<Task>? Closed { get; set; }
        internal Func<Task>? Changed { get; set; }

        public async Task CompleteAsync(object? value, bool cancelled)
        {
            completion.TrySetResult(new XFDialogResult(cancelled, value));
            if (Closed is not null)
            {
                await Closed.Invoke();
            }
        }

        private Task SetBusyAsync(bool _)
        {
            return Changed?.Invoke() ?? System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
