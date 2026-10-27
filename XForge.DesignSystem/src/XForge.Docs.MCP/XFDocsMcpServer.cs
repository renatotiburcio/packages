using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XForge.Docs.MCP;

/// <summary>
/// MCP server hosting XForge.DesignSystem documentation.
/// Inspired by antd-blazor PR #4758 (Feb 2026).
/// Endpoint: /mcp/v1/tools/{name}
/// </summary>
public class XFDocsMcpServer
{
    public string ServerName { get; } = "XForge.Docs.MCP";
    public string ServerVersion { get; } = "1.0.0";
    public IReadOnlyList<string> Tools { get; } = new[]
    {
        "get_component_docs",
        "list_components",
        "get_parameter_info",
        "get_event_info",
    };

    public WebApplication BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(this);
        builder.Services.AddRouting();
        var app = builder.Build();
        app.MapGet("/mcp/v1/info", () => Results.Ok(new { name = ServerName, version = ServerVersion, tools = Tools }));
        app.MapGet("/mcp/v1/tools/list_components", () => Results.Ok(new[] { "XFButton", "XFInput", "XFCard", "XFModal" }));
        app.MapGet("/mcp/v1/tools/get_component_docs", (string component) => Results.Ok(new { component, description = $"Documentation for {component}" }));
        app.MapGet("/mcp/v1/tools/get_parameter_info", (string component, string parameter) => Results.Ok(new { component, parameter, type = "string" }));
        app.MapGet("/mcp/v1/tools/get_event_info", (string component, string eventName) => Results.Ok(new { component, eventName, signature = "EventCallback" }));
        return app;
    }

    public void Run(string[] args) => BuildApplication(args).Run();
}