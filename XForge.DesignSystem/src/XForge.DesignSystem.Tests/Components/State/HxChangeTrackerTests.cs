using XForge.DesignSystem.Components.State;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.State;

public class HxChangeTrackerTests
{
    [Fact]
    public void Snapshot_SetsOriginalAndCurrent()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        Assert.False(t.IsDirty);
        Assert.Equal(1, t.ChangeCount);
    }

    [Fact]
    public void Update_SameValue_DoesNotChange()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        t.Update("a", 1);
        Assert.False(t.IsDirty);
    }

    [Fact]
    public void Update_DifferentValue_MarksDirty()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        t.Update("a", 2);
        Assert.True(t.IsDirty);
        Assert.Equal(2, t.Changes["a"]);
    }

    [Fact]
    public void Update_NewProperty_MarksDirty()
    {
        var t = new HxChangeTracker();
        t.Update("new", "value");
        Assert.True(t.IsDirty);
        Assert.Equal("value", t.Changes["new"]);
    }

    [Fact]
    public void AcceptChanges_ClearsDirty()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        t.Update("a", 2);
        t.AcceptChanges();
        Assert.False(t.IsDirty);
    }

    [Fact]
    public void RejectChanges_RevertsToOriginal()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        t.Update("a", 2);
        t.RejectChanges();
        Assert.False(t.IsDirty);
        Assert.Equal(1, t.Changes["a"]);
    }

    [Fact]
    public void Reset_ClearsEverything()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        t.Update("a", 2);
        t.Reset();
        Assert.False(t.IsDirty);
        Assert.Equal(0, t.ChangeCount);
    }

    [Fact]
    public void PropertyChanged_RaisedOnUpdate()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        var fired = false;
        t.PropertyChanged += (_, _) => fired = true;
        t.Update("a", 2);
        Assert.True(fired);
    }

    [Fact]
    public void Changes_ReturnsCurrentDictionary()
    {
        var t = new HxChangeTracker();
        t.Snapshot("a", 1);
        Assert.Equal(1, t.Changes["a"]);
    }
}
