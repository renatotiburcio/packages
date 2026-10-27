using XForge.DesignSystem.Components.Feedback.Progress;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.Feedback.Progress;

public class XFPageProgressServiceTests
{
    [Fact]
    public void Go_RaisesProgressChanged()
    {
        var svc = new XFPageProgressService();
        var fired = false;
        svc.ProgressChanged += (_, _) => fired = true;
        svc.Go(50);
        Assert.True(fired);
    }

    [Fact]
    public void Go_PassesPercentageAndColor()
    {
        var svc = new XFPageProgressService();
        XFPageProgressEventArgs? args = null;
        svc.ProgressChanged += (_, e) => args = e;
        svc.Go(75, XFProgressColor.Success);
        Assert.NotNull(args);
        Assert.Equal(75, args!.Percentage);
        Assert.Equal(XFProgressColor.Success, args.Color);
    }

    [Fact]
    public void Go_NullPercentage_Indeterminate()
    {
        var svc = new XFPageProgressService();
        XFPageProgressEventArgs? args = null;
        svc.ProgressChanged += (_, e) => args = e;
        svc.Go(null);
        Assert.Null(args!.Percentage);
    }

    [Fact]
    public void Go_DefaultColorIsPrimary()
    {
        var svc = new XFPageProgressService();
        XFPageProgressEventArgs? args = null;
        svc.ProgressChanged += (_, e) => args = e;
        svc.Go(50);
        Assert.Equal(XFProgressColor.Primary, args!.Color);
    }

    [Fact]
    public void ProgressChanged_NoSubscribers_DoesNotThrow()
    {
        var svc = new XFPageProgressService();
        svc.Go(50);
    }

    [Fact]
    public void Go_MultipleCalls_RaiseMultipleEvents()
    {
        var svc = new XFPageProgressService();
        var count = 0;
        svc.ProgressChanged += (_, _) => count++;
        svc.Go(25);
        svc.Go(50);
        svc.Go(75);
        Assert.Equal(3, count);
    }

    [Fact]
    public void XFProgressColor_Has10Members()
    {
        Assert.Equal(9, Enum.GetValues<XFProgressColor>().Length);
    }

    [Fact]
    public void XFProgressVariant_Has3Members()
    {
        Assert.Equal(3, Enum.GetValues<XFProgressVariant>().Length);
    }
}
