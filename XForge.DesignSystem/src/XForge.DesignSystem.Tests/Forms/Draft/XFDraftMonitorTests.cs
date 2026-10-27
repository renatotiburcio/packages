using XForge.Components.Forms.Draft;
using Xunit;

namespace XForge.DesignSystem.Tests.Forms.Draft;

public class XFDraftMonitorTests
{
    [Fact]
    public void Constructor_NullSave_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new XFDraftMonitor(null!));
    }

    [Fact]
    public void Snapshot_DefaultIsFresh()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        Assert.Equal(XFDraftState.Fresh, m.Snapshot.State);
    }

    [Fact]
    public void MarkDirty_SetsDirtyState()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        m.MarkDirty("form1", "{}");
        Assert.Equal(XFDraftState.Dirty, m.Snapshot.State);
        Assert.Equal("form1", m.Snapshot.FormId);
        Assert.Equal("{}", m.Snapshot.SerializedForm);
    }

    [Fact]
    public void MarkDirty_RaisesStateChanged()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        var fired = false;
        m.StateChanged += (_, _) => fired = true;
        m.MarkDirty("form1", "{}");
        Assert.True(fired);
    }

    [Fact]
    public async Task SaveAsync_NoSerializedForm_DoesNotChangeState()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        await m.SaveAsync();
        Assert.Equal(XFDraftState.Fresh, m.Snapshot.State);
    }

    [Fact]
    public async Task SaveAsync_WithSerializedForm_SetsSaved()
    {
        var saved = false;
        var m = new XFDraftMonitor(s => { saved = true; return Task.CompletedTask; });
        m.MarkDirty("form1", "{}");
        await m.SaveAsync();
        Assert.Equal(XFDraftState.Saved, m.Snapshot.State);
        Assert.True(saved);
        Assert.NotNull(m.Snapshot.LastSavedAt);
    }

    [Fact]
    public async Task SaveAsync_StorageThrows_SetsError()
    {
        var m = new XFDraftMonitor(_ => throw new Exception("fail"));
        m.MarkDirty("form1", "{}");
        await m.SaveAsync();
        Assert.Equal(XFDraftState.Error, m.Snapshot.State);
    }

    [Fact]
    public async Task SaveAsync_RaisesStateChanged()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        var fired = false;
        m.StateChanged += (_, _) => fired = true;
        m.MarkDirty("form1", "{}");
        await m.SaveAsync();
        Assert.True(fired);
    }

    [Fact]
    public void Reset_GoesBackToFresh()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        m.MarkDirty("form1", "{}");
        m.Reset();
        Assert.Equal(XFDraftState.Fresh, m.Snapshot.State);
        Assert.Null(m.Snapshot.FormId);
    }

    [Fact]
    public void Reset_RaisesStateChanged()
    {
        var m = new XFDraftMonitor(_ => Task.CompletedTask);
        var fired = false;
        m.StateChanged += (_, _) => fired = true;
        m.Reset();
        Assert.True(fired);
    }

    [Fact]
    public void XFDraftState_HasFourMembers()
    {
        Assert.Equal(4, Enum.GetValues<XFDraftState>().Length);
    }

    [Fact]
    public void XFDraftSnapshot_DefaultIsFresh()
    {
        var s = new XFDraftSnapshot();
        Assert.Equal(XFDraftState.Fresh, s.State);
    }
}
