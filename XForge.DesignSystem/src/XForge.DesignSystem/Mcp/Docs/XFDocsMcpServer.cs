// T053: Docs.Mcp server (Blazorise Docs.Mcp pattern, AI-queryable docs).
// P2, 32h, depends T021 (Docs.Compiler) and T046 (Snippets).
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Mcp.Docs.Tools;

namespace XForge.DesignSystem.Mcp.Docs;

/// <summary>
/// Self-hosted MCP server exposing XForge documentation tools.
/// Implements the 6 canonical Blazorise Docs.Mcp tools.
/// </summary>
public sealed class XFDocsMcpServer
{
    public const string ServerName = "XForge.Docs.Mcp";
    public const string ServerVersion = "1.0.0";
    public const int ToolCount = 6;

    private readonly IServiceProvider _services;
    public XFDocsMcpServer(IServiceProvider services) { _services = services; }

    public IReadOnlyList<IXFMcpTool> Tools { get; } = new IXFMcpTool[]
    {
        new ListComponentsTool(),
        new GetComponentTool(),
        new SearchDocsTool(),
        new ListSnippetsTool(),
        new GetSnippetTool(),
        new ListExamplesTool(),
    };

    public IXFMcpTool? FindTool(string name) => Tools.FirstOrDefault(t => t.Name == name);
}

public static class XFDocsMcpServiceCollectionExtensions
{
    public static IServiceCollection AddXFDocsMcp(this IServiceCollection services)
    {
        services.AddSingleton<XFDocsMcpServer>();
        return services;
    }
}