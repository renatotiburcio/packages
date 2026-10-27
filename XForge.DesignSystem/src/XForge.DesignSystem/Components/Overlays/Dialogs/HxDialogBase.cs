using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Overlays.Dialogs;
public abstract class HxDialogBase<TResult> : XForgeComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Title { get; set; }
    protected TaskCompletionSource<TResult> Tcs { get; } = new();
    public Task<TResult> WaitForResultAsync() => Tcs.Task;
    protected void Close(TResult result)
    {
        Tcs.TrySetResult(result);
    }
    protected void Cancel()
    {
        Tcs.TrySetCanceled();
    }
    public void Reset() { var tcs = Tcs; if (!tcs.Task.IsCompleted) tcs.TrySetCanceled(); }
}