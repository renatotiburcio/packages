using Xunit;
using XForge.DesignSystem.Components.DataDisplay.Sankey;

namespace XForge.DesignSystem.Tests.Components.DataDisplay;

/// <summary>
/// Tests for Sankey types at 0% coverage: XFSankeyAlignment enum,
/// XFSankeyNodeClickEventArgs, XFSankeyLinkClickEventArgs.
/// Note: XFSankeyNode, XFSankeyLink, XFSankeyComputedNode, XFSankeyComputedLink, XFSankeyLayout
/// are internal and cannot be tested directly.
/// </summary>
public sealed class XFSankeyTypesTest
{
    #region XFSankeyAlignment

    [Fact]
    public void XFSankeyAlignment_HasAllValues()
    {
        Assert.Equal(4, Enum.GetValues<XFSankeyAlignment>().Length);
        Assert.True(Enum.IsDefined<XFSankeyAlignment>(XFSankeyAlignment.Justify));
        Assert.True(Enum.IsDefined<XFSankeyAlignment>(XFSankeyAlignment.Left));
        Assert.True(Enum.IsDefined<XFSankeyAlignment>(XFSankeyAlignment.Right));
        Assert.True(Enum.IsDefined<XFSankeyAlignment>(XFSankeyAlignment.Center));
    }

    #endregion

    #region XFSankeyNodeClickEventArgs

    [Fact]
    public void XFSankeyNodeClickEventArgs_Defaults_AreCorrect()
    {
        var args = new XFSankeyNodeClickEventArgs();
        Assert.Equal(string.Empty, args.NodeId);
        Assert.Null(args.Label);
        Assert.Equal(0, args.Value);
        Assert.Equal(0, args.Incoming);
        Assert.Equal(0, args.Outgoing);
    }

    [Fact]
    public void XFSankeyNodeClickEventArgs_WithValues_AssignsCorrectly()
    {
        var args = new XFSankeyNodeClickEventArgs
        {
            NodeId = "node-1",
            Label = "Revenue",
            Value = 500,
            Incoming = 0,
            Outgoing = 500
        };
        Assert.Equal("node-1", args.NodeId);
        Assert.Equal("Revenue", args.Label);
        Assert.Equal(500, args.Value);
        Assert.Equal(0, args.Incoming);
        Assert.Equal(500, args.Outgoing);
    }

    #endregion

    #region XFSankeyLinkClickEventArgs

    [Fact]
    public void XFSankeyLinkClickEventArgs_Defaults_AreCorrect()
    {
        var args = new XFSankeyLinkClickEventArgs();
        Assert.Equal(string.Empty, args.Source);
        Assert.Equal(string.Empty, args.Target);
        Assert.Equal(0, args.Value);
    }

    [Fact]
    public void XFSankeyLinkClickEventArgs_WithValues_AssignsCorrectly()
    {
        var args = new XFSankeyLinkClickEventArgs
        {
            Source = "revenue",
            Target = "profit",
            Value = 200
        };
        Assert.Equal("revenue", args.Source);
        Assert.Equal("profit", args.Target);
        Assert.Equal(200, args.Value);
    }

    #endregion
}
