---
id: T102
title: MCP Server (self-hosted, 10 tools, HTTP)
priority: P0
status: done
estimated_time: 24h
dependencies: [T040]
---

## Objetivo
Expor 10 tools do DesignSystem via MCP HTTP self-hosted.

## Contexto
LGPD Art. 46: MCP server MUST be self-hosted (D040, D050).

## Descricao
Cria `XForge.McpServer` projeto com `[McpServerToolType]` tools.
Tools: get_component_catalog, get_component_docs, get_component_samples, get_type_doc, get_design_tokens, get_theme, get_installation, list_components, validate_component, get_migration_guide.
Transport: HTTP.
Stateless: true.
OpenTelemetry: McpToolActivitySource.
Health: GET /health.

## Passos
1. dotnet new web -n XForge.McpServer
2. Add ModelContextProtocol + OpenTelemetry packages
3. Implement 10 tools
4. Map GET/POST HTTP routes
5. Register McpToolActivitySource
6. Add /health endpoint

## Arquivos
- src/XForge.McpServer/Program.cs
- src/XForge.McpServer/Tools/ComponentTools.cs
- src/XForge.McpServer/Tools/TokenTools.cs
- src/XForge.McpServer/Tools/HealthEndpoint.cs
- tests/XForge.DesignSystem.Tests/T102McpServerTests.cs

## Criterios de Aceite
- 10 tools respondem HTTP 200
- /health retorna 200
- OpenTelemetry traces emitidos por tool
- Stateless=true (cada request independente)