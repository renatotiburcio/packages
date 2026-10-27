using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Toast
{
    public sealed class XFToastService
    {
        public string DefaultAsyncSuccessTitle { get; set; } = "Concluído";
        public string DefaultAsyncLoadingTitle { get; set; } = "Carregando...";

        internal event Func<XFToastRequest, Task>? Requested;
        internal event Func<Guid, XFToastOptions, Task>? UpdateRequested;
        internal event Func<Guid?, Task>? ClearRequested;

        public async Task<Guid> ShowAsync(XFToastOptions options)
        {
            var request = new XFToastRequest(options);

            if (Requested is not null)
            {
                await Requested.Invoke(request);
            }

            return request.Id;
        }

        public Task<Guid> InfoAsync(string title, string? message = null, int? duration = null)
            => ShowAsync(new XFToastOptions { Title = title, Message = message, Intent = XFIntent.Info, Duration = duration });

        public Task<Guid> SuccessAsync(string title, string? message = null, int? duration = null)
            => ShowAsync(new XFToastOptions { Title = title, Message = message, Intent = XFIntent.Success, Duration = duration });

        public Task<Guid> WarningAsync(string title, string? message = null, int? duration = null)
            => ShowAsync(new XFToastOptions { Title = title, Message = message, Intent = XFIntent.Warning, Duration = duration });

        public Task<Guid> DangerAsync(string title, string? message = null, int? duration = null)
            => ShowAsync(new XFToastOptions { Title = title, Message = message, Intent = XFIntent.Danger, Duration = duration, Live = XFAriaLive.Assertive });

        public Task<Guid> ErrorAsync(string title, string? message = null, int? duration = null)
            => DangerAsync(title, message, duration);

        public Task UpdateAsync(Guid id, XFToastOptions options)
        {
            return UpdateRequested?.Invoke(id, options) ?? Task.CompletedTask;
        }

        public async Task<Guid> Async<T>(Task<T> operation, XFToastAsyncOptions<T> options)
        {
            var id = await ShowAsync(new XFToastOptions
            {
                Title = options.Loading ?? DefaultAsyncLoadingTitle,
                Intent = XFIntent.Info,
                Duration = 0,
                Dismissible = false,
                Icon = options.LoadingIcon,
                ToasterId = options.ToasterId,
                Placement = options.Placement
            });

            try
            {
                var result = await operation;
                await UpdateAsync(id, new XFToastOptions
                {
                    Title = options.Success?.Invoke(result) ?? DefaultAsyncSuccessTitle,
                    Intent = XFIntent.Success,
                    Duration = options.Duration,
                    Dismissible = true,
                    ToasterId = options.ToasterId,
                    Placement = options.Placement
                });
            }
            catch (Exception ex)
            {
                await UpdateAsync(id, new XFToastOptions
                {
                    Title = options.Error?.Invoke(ex) ?? ex.Message,
                    Intent = XFIntent.Danger,
                    Duration = options.Duration,
                    Dismissible = true,
                    Live = XFAriaLive.Assertive,
                    ToasterId = options.ToasterId,
                    Placement = options.Placement
                });
            }

            return id;
        }

        public Task ClearAsync(Guid? id = null)
        {
            return ClearRequested?.Invoke(id) ?? Task.CompletedTask;
        }
    }

    public sealed class XFToastOptions
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public XFIntent Intent { get; set; } = XFIntent.Info;
        public int? Duration { get; set; } = 5000;
        public bool Dismissible { get; set; } = true;
        public bool? CloseButton { get; set; }
        public bool? RichColors { get; set; }
        public bool? SolidColors { get; set; }
        public XFToastColorMode? ColorMode { get; set; }
        public XFToastPlacement? Placement { get; set; }
        public string? ToasterId { get; set; }
        public string? ActionText { get; set; }
        public Func<XFToastItem, Task>? Action { get; set; }
        public Func<XFToastItem, Task>? Closed { get; set; }
        public RenderFragment<XFToastItem>? Content { get; set; }
        public RenderFragment<XFToastItem>? Icon { get; set; }
        public XFAriaLive Live { get; set; } = XFAriaLive.Polite;
        public object? Payload { get; set; }
    }

    internal sealed class XFToastRequest
    {
        public XFToastRequest(XFToastOptions options)
        {
            Options = options;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public XFToastOptions Options { get; }
    }

    public sealed class XFToastItem
    {
        public XFToastItem(Guid id, XFToastOptions options)
        {
            Id = id;
            Title = options.Title;
            Message = options.Message;
            Intent = options.Intent;
            Duration = options.Duration;
            Dismissible = options.Dismissible;
            CloseButton = options.CloseButton;
            RichColors = options.RichColors;
            SolidColors = options.SolidColors;
            ColorMode = options.ColorMode;
            Placement = options.Placement;
            ToasterId = options.ToasterId;
            ActionText = options.ActionText;
            Action = options.Action;
            Closed = options.Closed;
            Content = options.Content;
            Icon = options.Icon;
            Live = options.Live;
            Payload = options.Payload;
        }

        public Guid Id { get; }
        public string? Title { get; }
        public string? Message { get; }
        public XFIntent Intent { get; }
        public int? Duration { get; }
        public bool Dismissible { get; }
        public bool? CloseButton { get; }
        public bool? RichColors { get; }
        public bool? SolidColors { get; }
        public XFToastColorMode? ColorMode { get; }
        public XFToastPlacement? Placement { get; }
        public string? ToasterId { get; }
        public string? ActionText { get; }
        public Func<XFToastItem, Task>? Action { get; }
        public Func<XFToastItem, Task>? Closed { get; }
        public RenderFragment<XFToastItem>? Content { get; }
        public RenderFragment<XFToastItem>? Icon { get; }
        public XFAriaLive Live { get; }
        public object? Payload { get; }
    }

    public sealed class XFToastAsyncOptions<T>
    {
        public string? Loading { get; set; }
        public Func<T, string>? Success { get; set; }
        public Func<Exception, string>? Error { get; set; }
        public int? Duration { get; set; } = 5000;
        public string? ToasterId { get; set; }
        public XFToastPlacement? Placement { get; set; }
        public RenderFragment<XFToastItem>? LoadingIcon { get; set; }
    }
}
