using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
namespace XForge.McpServer.Tools;
public class McpToolRequest(string name, Dictionary<string,object?> args)
{
    public string Name { get; } = name;
    public Dictionary<string,object?> Args { get; } = args;
    public T? GetArg<T>(string key) { if (Args.TryGetValue(key, out var v) && v is JsonElement el) return JsonSerializer.Deserialize<T>(el.GetRawText()); return default; }
}
public record McpToolResponse(bool Success, object? Data, string? Error);
public class XForgeDocsMcpServer
{
    private readonly Dictionary<string, Func<McpToolRequest, Task<McpToolResponse>>> _tools = new();
    public IReadOnlyList<string> AvailableTools => _tools.Keys.ToList();
    public XForgeDocsMcpServer()
    {
        _tools["list_components"] = _ => Task.FromResult(new McpToolResponse(true, new[] { "XFButton", "XFInput", "XFDataGrid" }, null));
        _tools["get_component"] = req => { var name = req.GetArg<string>("name") ?? ""; return Task.FromResult(new McpToolResponse(true, new { name, base = "XForgeComponentBase", events = Array.Empty<string>() }, null)); };
        _tools["search_docs"] = req => { var q = req.GetArg<string>("q") ?? ""; return Task.FromResult(new McpToolResponse(true, new { query = q, results = new[] { q } }, null)); };
    }
    public async Task<McpToolResponse> InvokeAsync(McpToolRequest req) { if (!_tools.TryGetValue(req.Name, out var fn)) return new McpToolResponse(false, null, $"Unknown tool: {req.Name}"); return await fn(req); }
    public bool HasTool(string name) => _tools.ContainsKey(name);
    public int ToolCount => _tools.Count;
}