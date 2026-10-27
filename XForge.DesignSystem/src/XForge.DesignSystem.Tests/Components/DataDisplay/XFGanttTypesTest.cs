using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Gantt;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Tests for Gantt types at 0% coverage: XFGanttView, XFGanttVariant, XFGanttDependencyType,
/// XFGanttDependency, XFGanttTaskClickEventArgs, XFGanttTaskMoveEventArgs.
/// Note: XFGanttNode is internal and cannot be tested directly.
/// </summary>
public sealed class XFGanttTypesTest
{
    #region XFGanttView

    [Fact]
    public void XFGanttView_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFGanttView>().Length);
        Assert.True(Enum.IsDefined<XFGanttView>(XFGanttView.Day));
        Assert.True(Enum.IsDefined<XFGanttView>(XFGanttView.Week));
        Assert.True(Enum.IsDefined<XFGanttView>(XFGanttView.Month));
        Assert.True(Enum.IsDefined<XFGanttView>(XFGanttView.Year));
    }

    #endregion

    #region XFGanttVariant

    [Fact]
    public void XFGanttVariant_HasAllValues()
    {
        Assert.Equal(3, Enum.GetValues<XFGanttVariant>().Length);
        Assert.True(Enum.IsDefined<XFGanttVariant>(XFGanttVariant.Default));
        Assert.True(Enum.IsDefined<XFGanttVariant>(XFGanttVariant.Compact));
        Assert.True(Enum.IsDefined<XFGanttVariant>(XFGanttVariant.Comfortable));
    }

    #endregion

    #region XFGanttDependencyType

    [Fact]
    public void XFGanttDependencyType_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFGanttDependencyType>().Length);
        Assert.True(Enum.IsDefined<XFGanttDependencyType>(XFGanttDependencyType.FinishToStart));
        Assert.True(Enum.IsDefined<XFGanttDependencyType>(XFGanttDependencyType.StartToStart));
        Assert.True(Enum.IsDefined<XFGanttDependencyType>(XFGanttDependencyType.FinishToFinish));
        Assert.True(Enum.IsDefined<XFGanttDependencyType>(XFGanttDependencyType.StartToFinish));
    }

    #endregion

    #region XFGanttDependency

    [Fact]
    public void XFGanttDependency_Defaults_AreCorrect()
    {
        var dep = new XFGanttDependency();
        Assert.Equal(string.Empty, dep.FromId);
        Assert.Equal(string.Empty, dep.ToId);
        Assert.Equal(XFGanttDependencyType.FinishToStart, dep.Type);
    }

    [Fact]
    public void XFGanttDependency_WithValues_AssignsCorrectly()
    {
        var dep = new XFGanttDependency
        {
            FromId = "task-1",
            ToId = "task-2",
            Type = XFGanttDependencyType.StartToStart
        };
        Assert.Equal("task-1", dep.FromId);
        Assert.Equal("task-2", dep.ToId);
        Assert.Equal(XFGanttDependencyType.StartToStart, dep.Type);
    }

    #endregion

    #region XFGanttTaskClickEventArgs

    [Fact]
    public void XFGanttTaskClickEventArgs_Constructs_WithValues()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 1, 15);
        var args = new XFGanttTaskClickEventArgs<string>
        {
            Item = "Task A",
            Start = start,
            End = end
        };
        Assert.Equal("Task A", args.Item);
        Assert.Equal(start, args.Start);
        Assert.Equal(end, args.End);
    }

    [Fact]
    public void XFGanttTaskClickEventArgs_DefaultItem_IsDefault()
    {
        var args = new XFGanttTaskClickEventArgs<int>();
        Assert.Equal(0, args.Item);
        Assert.Equal(default(DateTime), args.Start);
        Assert.Equal(default(DateTime), args.End);
    }

    #endregion

    #region XFGanttTaskMoveEventArgs

    [Fact]
    public void XFGanttTaskMoveEventArgs_Constructs_WithValues()
    {
        var origStart = new DateTime(2026, 1, 1);
        var origEnd = new DateTime(2026, 1, 15);
        var newStart = new DateTime(2026, 1, 5);
        var newEnd = new DateTime(2026, 1, 20);
        var args = new XFGanttTaskMoveEventArgs<string>
        {
            Item = "Task B",
            OriginalStart = origStart,
            OriginalEnd = origEnd,
            NewStart = newStart,
            NewEnd = newEnd
        };
        Assert.Equal("Task B", args.Item);
        Assert.Equal(origStart, args.OriginalStart);
        Assert.Equal(origEnd, args.OriginalEnd);
        Assert.Equal(newStart, args.NewStart);
        Assert.Equal(newEnd, args.NewEnd);
    }

    #endregion
}
