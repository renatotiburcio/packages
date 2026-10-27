using System.Threading;
using XForge.DesignSystem.Components.Utilities.Timing;
using Xunit;
namespace XForge.DesignSystem.Tests.Utilities;
public class DebouncerTests
{
    [Fact] public void Debouncer_Starts_With_Zero_Pending() { var d = new Debouncer(50, () => { }); Assert.Equal(0, d.PendingCount); }
    [Fact] public void Debouncer_Trigger_Increments_Pending() { var d = new Debouncer(50, () => { }); d.Trigger(); Assert.Equal(1, d.PendingCount); }
    [Fact] public void Debouncer_Multiple_Triggers_Stay_At_One_Pending() { var d = new Debouncer(50, () => { }); d.Trigger(); d.Trigger(); d.Trigger(); Assert.Equal(1, d.PendingCount); }
    [Fact] public void Debouncer_Cancel_Resets_Pending() { var d = new Debouncer(50, () => { }); d.Trigger(); d.Cancel(); Assert.Equal(0, d.PendingCount); }
    [Fact] public void Debouncer_Action_Fires_After_Delay() { int n = 0; var d = new Debouncer(50, () => n++); d.Trigger(); Thread.Sleep(120); Assert.Equal(1, n); d.Dispose(); }
    [Fact] public void Throttler_Starts_With_Zero_Count() { var t = new Throttler(50, () => { }); Assert.Equal(0, t.ThrottleCount); }
    [Fact] public void Throttler_Trigger_Increments_Count() { var t = new Throttler(50, () => { }); t.Trigger(); Assert.Equal(1, t.ThrottleCount); }
    [Fact] public void Throttler_Cancel_Resets() { var t = new Throttler(50, () => { }); t.Trigger(); t.Cancel(); Assert.Equal(0, t.ThrottleCount); }
}