// T049: Tests for XFParameterDirtyWeaver + XFParameterChangeTracker.
using Xunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Weavers;

namespace XForge.DesignSystem.Tests.Weavers;

[XFDirtyTracked]
public class DemoDirtyComponent
{
    [Parameter] [XFDirty] public string? Name { get; set; }
    [Parameter] public int Age { get; set; }
    [Parameter] [XFDirty] public string? Email { get; set; }
    [Parameter] [XFDirty(FireEvent = false)] public bool IsAdmin { get; set; }
}

public class XFParameterDirtyWeaverTests
{
    private readonly IXFParameterDirtyWeaver w = new XFParameterDirtyWeaver();

    [Fact] public void ShouldWeave_TrueForTracked() => Assert.True(w.ShouldWeave(typeof(DemoDirtyComponent)));

    [Fact] public void GetTrackedProperties_OnlyXFDirty()
    {
        var names = w.GetTrackedProperties(typeof(DemoDirtyComponent));
        Assert.Contains("Name", names);
        Assert.Contains("Email", names);
        Assert.Contains("IsAdmin", names);
        Assert.DoesNotContain("Age", names);
    }
}

public class XFParameterChangeTrackerTests
{
    [Fact] public void MarkDirty_UntrackedProperty_DoesNothing()
    {
        var t = new XFParameterChangeTracker(new XFParameterDirtyWeaver(), typeof(DemoDirtyComponent));
        t.MarkDirty("Age");
        Assert.Equal(0, t.DirtyCount);
    }

    [Fact] public void MarkDirty_TrackedProperty_Increments()
    {
        var t = new XFParameterChangeTracker(new XFParameterDirtyWeaver(), typeof(DemoDirtyComponent));
        t.MarkDirty("Name");
        Assert.True(t.IsDirty("Name"));
        Assert.Equal(1, t.DirtyCount);
    }

    [Fact] public void MarkDirty_Duplicate_NoDoubleFire()
    {
        var t = new XFParameterChangeTracker(new XFParameterDirtyWeaver(), typeof(DemoDirtyComponent));
        var fired = 0;
        t.ParameterChanged += (_, _) => fired++;
        t.MarkDirty("Name");
        t.MarkDirty("Name");
        Assert.Equal(1, fired);
    }

    [Fact] public void Reset_ClearsDirty()
    {
        var t = new XFParameterChangeTracker(new XFParameterDirtyWeaver(), typeof(DemoDirtyComponent));
        t.MarkDirty("Name");
        t.Reset();
        Assert.Equal(0, t.DirtyCount);
    }
}