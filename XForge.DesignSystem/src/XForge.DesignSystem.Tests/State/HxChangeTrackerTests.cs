using XForge.DesignSystem.Components.State;
using Xunit;
namespace XForge.DesignSystem.Tests.State;
public class HxChangeTrackerTests
{
    [Fact] public void New_Tracker_Not_Dirty() { var t = new HxChangeTracker(); Assert.False(t.IsDirty); Assert.Equal(0, t.ChangeCount); }
    [Fact] public void Snapshot_Then_Update_Marks_Dirty() { var t = new HxChangeTracker(); t.Snapshot("Name","a"); t.Update("Name","b"); Assert.True(t.IsDirty); Assert.Equal(1, t.ChangeCount); }
    [Fact] public void AcceptChanges_Clears_Dirty() { var t = new HxChangeTracker(); t.Snapshot("Name","a"); t.Update("Name","b"); t.AcceptChanges(); Assert.False(t.IsDirty); }
    [Fact] public void RejectChanges_Restores_Originals() { var t = new HxChangeTracker(); t.Snapshot("Name","a"); t.Update("Name","b"); t.RejectChanges(); Assert.False(t.IsDirty); Assert.Equal("a", t.Changes["Name"]); }
    [Fact] public void Reset_Clears_All() { var t = new HxChangeTracker(); t.Snapshot("Name","a"); t.Reset(); Assert.Equal(0, t.ChangeCount); Assert.False(t.IsDirty); }
    [Fact] public void Update_Raises_PropertyChanged() { var t = new HxChangeTracker(); t.Snapshot("X",1); bool raised = false; t.PropertyChanged += (_,_) => raised = true; t.Update("X",2); Assert.True(raised); }
}