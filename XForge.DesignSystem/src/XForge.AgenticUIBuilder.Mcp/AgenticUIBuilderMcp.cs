using Microsoft.AspNetCore.Components;
using ModelContextProtocol.Server;

namespace XForge.AgenticUIBuilder.Mcp;

/// <summary>T126: XForge AgenticUIBuilder MCP -- NL to Blazor layout. Syncfusion. P0. 32h. Dep T102+T104.</summary>
[McpServerToolType]
public class AgenticUIBuilderMcp
{
    [McpServerTool(Name = "generate_layout", Description = "Generate Blazor layout from natural language")]
    public Task<string> GenerateLayoutAsync(string intent) { return Task.FromResult($"<div>{intent}</div>"); }
    [McpServerTool(Name = "list_templates", Description = "List available layout templates")]
    public Task<string[]> ListTemplatesAsync() { return Task.FromResult(new[] { "form", "dashboard", "kanban" }); }
}