// T053: Tests for XFDocsMcpServer.
using Xunit;
using XForge.DesignSystem.Mcp.Docs;
using XForge.DesignSystem.Mcp.Docs.Tools;

namespace XForge.DesignSystem.Tests.Mcp.Docs;

public class XFDocsMcpServerTests
{
    private readonly XFDocsMcpServer s = new(new ServiceProviderStub());

    [Fact] public void ServerName_IsXForgeDocs() => Assert.Equal("XForge.Docs.Mcp", XFDocsMcpServer.ServerName);
    [Fact] public void ToolCount_Is6() => Assert.Equal(6, XFDocsMcpServer.ToolCount);
    [Fact] public void Tools_HasExpectedCount() => Assert.Equal(6, s.Tools.Count);
    [Fact] public void FindTool_ReturnsMatch() => Assert.NotNull(s.FindTool("list-components"));
    [Fact] public void FindTool_Unknown_Null() => Assert.Null(s.FindTool("nope"));
    [Fact] public void ListComponentsTool_Execute_ReturnsOkJson()
    {
        var t = (IXFMcpTool)new ListComponentsTool();
        var json = t.Execute("{}");
        Assert.Contains("\"status\":\"ok\"", json);
    }

    private sealed class ServiceProviderStub : IServiceProvider { public object? GetService(Type t) => null; }
}