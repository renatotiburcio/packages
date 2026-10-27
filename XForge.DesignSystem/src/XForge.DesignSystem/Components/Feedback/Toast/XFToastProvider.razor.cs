using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Components.Feedback.Toast
{
    public partial class XFToastProvider : IDisposable
    {
        private readonly List<XFToastItem> toasts = [];
        private readonly Dictionary<Guid, CancellationTokenSource> timers = [];

        [Parameter] public string? Id { get; set; }
        [Parameter] public string? ProviderId { get; set; }
        [Parameter] public XFToastPlacement Placement { get; set; } = XFToastPlacement.BottomEnd;
        [Parameter] public XFToastPlacement Position { get => Placement; set => Placement = value; }
        [Parameter] public int VisibleToasts { get; set; } = 3;
        [Parameter] public int MaxToastCount { get => VisibleToasts; set => VisibleToasts = value; }
        [Parameter] public int Gap { get; set; } = 14;
        [Parameter] public int ItemHeight { get; set; } = 88;
        [Parameter] public int Offset { get; set; } = 24;
        [Parameter] public int MobileOffset { get; set; } = 16;
        [Parameter] public int? Duration { get; set; }
        [Parameter] public bool Expand { get; set; }
        [Parameter] public bool CloseButton { get; set; }
        [Parameter] public bool RichColors { get; set; }
        [Parameter] public bool SolidColors { get; set; }
        [Parameter] public XFToastColorMode ColorMode { get; set; } = XFToastColorMode.Default;
        [Parameter] public XFAriaLive Live { get; set; } = XFAriaLive.Polite;

        private int VisibleToastCount => Math.Max(1, VisibleToasts);
        private IReadOnlyList<XFToastItem> RenderedToasts => toasts.Take(VisibleToastCount).ToArray();

        private string AriaLive => Live switch
        {
            XFAriaLive.Assertive => "assertive",
            XFAriaLive.Off => "off",
            _ => "polite"
        };

        private bool IsTopPlacement => Placement is XFToastPlacement.TopStart or XFToastPlacement.Top or XFToastPlacement.TopEnd;

        private string? ResolvedProviderId => ProviderId ?? Id;

        private int SafeGap => Math.Max(0, Gap);

        private int SafeItemHeight => Math.Max(64, ItemHeight);

        private int StackHeight => RenderedToasts.Count is 0
            ? 0
            : RenderedToasts.Count * SafeItemHeight + Math.Max(0, RenderedToasts.Count - 1) * SafeGap;

        private string ProviderStyle => $"--xf-toast-gap:{SafeGap}px;--xf-toast-item-height:{SafeItemHeight}px;--xf-toast-offset:{Math.Max(0, Offset)}px;--xf-toast-mobile-offset:{Math.Max(0, MobileOffset)}px;min-height:{StackHeight}px;";

        private string ProviderClass => Placement switch
        {
            XFToastPlacement.TopStart => "xf-toast-provider group/toaster pointer-events-auto fixed left-[var(--xf-toast-mobile-offset)] top-[var(--xf-toast-mobile-offset)] z-[10000] grid w-[calc(100vw-2rem)] max-w-sm content-start sm:left-[var(--xf-toast-offset)] sm:top-[var(--xf-toast-offset)]",
            XFToastPlacement.Top => "xf-toast-provider group/toaster pointer-events-auto fixed left-1/2 top-[var(--xf-toast-mobile-offset)] z-[10000] grid w-[calc(100vw-2rem)] max-w-sm -translate-x-1/2 content-start sm:top-[var(--xf-toast-offset)]",
            XFToastPlacement.TopEnd => "xf-toast-provider group/toaster pointer-events-auto fixed right-[var(--xf-toast-mobile-offset)] top-[var(--xf-toast-mobile-offset)] z-[10000] grid w-[calc(100vw-2rem)] max-w-sm content-start sm:right-[var(--xf-toast-offset)] sm:top-[var(--xf-toast-offset)]",
            XFToastPlacement.BottomStart => "xf-toast-provider group/toaster pointer-events-auto fixed bottom-[var(--xf-toast-mobile-offset)] left-[var(--xf-toast-mobile-offset)] z-[10000] grid w-[calc(100vw-2rem)] max-w-sm content-end sm:bottom-[var(--xf-toast-offset)] sm:left-[var(--xf-toast-offset)]",
            XFToastPlacement.Bottom => "xf-toast-provider group/toaster pointer-events-auto fixed bottom-[var(--xf-toast-mobile-offset)] left-1/2 z-[10000] grid w-[calc(100vw-2rem)] max-w-sm -translate-x-1/2 content-end sm:bottom-[var(--xf-toast-offset)]",
            _ => "xf-toast-provider group/toaster pointer-events-auto fixed bottom-[var(--xf-toast-mobile-offset)] right-[var(--xf-toast-mobile-offset)] z-[10000] grid w-[calc(100vw-2rem)] max-w-sm content-end sm:bottom-[var(--xf-toast-offset)] sm:right-[var(--xf-toast-offset)]"
        };

        protected override void OnInitialized()
        {
            ToastService.Requested += ShowAsync;
            ToastService.UpdateRequested += UpdateAsync;
            ToastService.ClearRequested += ClearAsync;
        }

        private async Task ShowAsync(XFToastRequest request)
        {
            if (!string.IsNullOrWhiteSpace(ResolvedProviderId) &&
                !string.Equals(request.Options.ToasterId, ResolvedProviderId, StringComparison.Ordinal))
            {
                return;
            }

            XFToastItem? item = null;

            await InvokeAsync(() =>
            {
                if (request.Options.Placement is { } placement)
                {
                    Placement = placement;
                }

                request.Options.Duration ??= Duration;
                item = new XFToastItem(request.Id, request.Options);
                toasts.Insert(0, item);
                TrimOverflow();
                StateHasChanged();
            });

            if (item?.Duration is > 0)
            {
                StartTimer(item);
            }
        }

        private Task UpdateAsync(Guid id, XFToastOptions options)
        {
            return InvokeAsync(() =>
            {
                var index = toasts.FindIndex(item => item.Id == id);
                if (index < 0)
                {
                    return;
                }

                if (options.Placement is { } placement)
                {
                    Placement = placement;
                }

                options.Duration ??= Duration;
                var item = new XFToastItem(id, options);
                toasts[index] = item;
                CancelTimer(id);

                if (item.Duration is > 0)
                {
                    StartTimer(item);
                }

                StateHasChanged();
            });
        }

        private void StartTimer(XFToastItem item)
        {
            var cts = new CancellationTokenSource();
            timers[item.Id] = cts;
            _ = AutoDismissAsync(item, cts.Token);
        }

        private async Task AutoDismissAsync(XFToastItem item, CancellationToken token)
        {
            try
            {
                await Task.Delay(item.Duration!.Value, token);

                if (!token.IsCancellationRequested)
                {
                    await InvokeAsync(() => DismissAsync(item));
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task DismissAsync(XFToastItem item)
        {
            if (!toasts.Remove(item))
            {
                return;
            }

            CancelTimer(item.Id);

            if (item.Closed is not null)
            {
                await item.Closed.Invoke(item);
            }

            StateHasChanged();
        }

        private Task ClearAsync(Guid? id)
        {
            return InvokeAsync(() =>
            {
                if (id is Guid value)
                {
                    var item = toasts.FirstOrDefault(t => t.Id == value);
                    if (item is not null)
                    {
                        toasts.Remove(item);
                        CancelTimer(item.Id);
                    }
                }
                else
                {
                    foreach (var item in toasts.ToArray())
                    {
                        CancelTimer(item.Id);
                    }

                    toasts.Clear();
                }

                StateHasChanged();
            });
        }

        private void TrimOverflow()
        {
            var max = VisibleToastCount;
            while (toasts.Count > max)
            {
                var last = toasts[^1];
                CancelTimer(last.Id);
                toasts.RemoveAt(toasts.Count - 1);
            }
        }

        private void CancelTimer(Guid id)
        {
            if (timers.Remove(id, out var timer))
            {
                timer.Cancel();
                timer.Dispose();
            }
        }

        public void Dispose()
        {
            ToastService.Requested -= ShowAsync;
            ToastService.UpdateRequested -= UpdateAsync;
            ToastService.ClearRequested -= ClearAsync;

            foreach (var timer in timers.Values)
            {
                timer.Cancel();
                timer.Dispose();
            }

            timers.Clear();
        }
    }
}
