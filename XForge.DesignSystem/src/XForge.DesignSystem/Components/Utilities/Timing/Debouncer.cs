using System;
using System.Timers;
namespace XForge.DesignSystem.Components.Utilities.Timing;
public class Debouncer : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly Action _action;
    public int PendingCount { get; private set; }
    public Debouncer(int delayMs, Action action) { _action = action; _timer = new System.Timers.Timer(delayMs) { AutoReset = false }; _timer.Elapsed += (_,_) => { PendingCount--; _action(); }; }
    public void Trigger() { PendingCount++; _timer.Stop(); _timer.Start(); }
    public void Cancel() { _timer.Stop(); PendingCount = 0; }
    public void Dispose() { _timer.Dispose(); }
}
public class Throttler : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly Action _action;
    private DateTime _lastFired = DateTime.MinValue;
    private readonly object _lock = new();
    public int ThrottleCount { get; private set; }
    public Throttler(int intervalMs, Action action) { _action = action; _timer = new System.Timers.Timer(intervalMs) { AutoReset = false }; _timer.Elapsed += (_,_) => { ThrottleCount--; _action(); }; }
    public void Trigger() { lock (_lock) { ThrottleCount++; _timer.Stop(); _timer.Start(); } }
    public void Cancel() { _timer.Stop(); ThrottleCount = 0; }
    public void Dispose() { _timer.Dispose(); }
}