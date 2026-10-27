# XForge.Docs.MCP

MCP (Model Context Protocol) server for XForge.DesignSystem documentation.

## Endpoints
- `GET /mcp/v1/info` - Server info (name, version, tools)
- `GET /mcp/v1/tools/list_components` - List all components
- `GET /mcp/v1/tools/get_component_docs?component=XFButton` - Get component docs
- `GET /mcp/v1/tools/get_parameter_info?component=XFButton&parameter=Variant` - Get parameter info
- `GET /mcp/v1/tools/get_event_info?component=XFButton&eventName=OnClick` - Get event info

## Run
```bash
dotnet run --project src/XForge.Docs.MCP
```

## Reference
- T028 in antd-blazor-comparison BACKLOG
- Inspired by antd-blazor PR #4758 (Feb 2026)