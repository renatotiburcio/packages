using System;
using System.Threading.Tasks;

namespace XForge.DesignSystem.Components.Forms.Draft;

/// <summary>
/// Monitors form draft state (in-progress edits not yet submitted).
/// Inspired by antd-blazor DraftMonitor component.
/// </summary>
public enum XFDraftState { Fresh, Dirty, Saved, Error }

public sealed record XFDraftSnapshot
{
    public XFDraftState State { get; set; } = XFDraftState.Fresh;
    public string? FormId { get; set; }
    public DateTimeOffset? LastSavedAt { get; set; }
    public string? SerializedForm { get; set; }
}

public sealed class XFDraftMonitor : IAsyncDisposable
{
    private readonly Func<string, Task> _saveToStorage;
    private XFDraftSnapshot _snapshot = new();

    public XFDraftMonitor(Func<string, Task> saveToStorage)
    {
        _saveToStorage = saveToStorage ?? throw new ArgumentNullException(nameof(saveToStorage));
    }

    public XFDraftSnapshot Snapshot => _snapshot;

    public event EventHandler<XFDraftSnapshot>? StateChanged;

    public void MarkDirty(string formId, string serializedForm)
    {
        _snapshot = new XFDraftSnapshot
        {
            State = XFDraftState.Dirty,
            FormId = formId,
            SerializedForm = serializedForm,
        };
        StateChanged?.Invoke(this, _snapshot);
    }

    public async Task SaveAsync()
    {
        if (_snapshot.SerializedForm is null) return;
        try
        {
            await _saveToStorage(_snapshot.SerializedForm);
            _snapshot = _snapshot with { State = XFDraftState.Saved, LastSavedAt = DateTimeOffset.UtcNow };
        }
        catch
        {
            _snapshot = _snapshot with { State = XFDraftState.Error };
        }
        StateChanged?.Invoke(this, _snapshot);
    }

    public void Reset()
    {
        _snapshot = new XFDraftSnapshot();
        StateChanged?.Invoke(this, _snapshot);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}