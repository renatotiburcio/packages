---
id: T028
title: MCP server para documentação (AI-queryable)
priority: P2
status: pending
estimated_time: 32h
dependencies: T021
---

# T028: MCP server para documentação

## Objetivo
Expor a documentação de XForge.DesignSystem via Model Context Protocol (MCP), permitindo que AI agents (Kilo, Claude, etc.) consultem docs em tempo real.

## Contexto
antd-blazor adicionou MCP server em PR #4758 (Fev 2026). Site de docs serve tambem como MCP endpoint, retornando documentacao estruturada em JSON quando consultado.

## Passos
1. Criar `site/XForge.Docs.MCP/` projeto (Microsoft.Extensions.Hosting)
2. Implementar MCP server com tools: `get_component_docs`, `list_components`, `get_parameter_info`, `get_event_info`
3. Endpoint HTTP: `/mcp/v1/tools/get_component_docs?component=XFButton`
4. Carregar docs a partir de `*Docs.razor` files (parser simples)
5. Registrar MCP server no `XForge.Docs.Server` startup
6. Documentar uso em `.xforge/knowledge/domains/design-system/mcp-server.md`
7. Testar com Kilo / Claude / Cursor

## Arquivos
- Criar: `site/XForge.Docs.MCP/McpServer.cs`
- Criar: `site/XForge.Docs.MCP/Tools/GetComponentDocs.cs`
- Criar: `site/XForge.Docs.MCP/Tools/ListComponents.cs`
- Modificar: `site/XForge.Docs.Server/Program.cs` (registrar MCP)

## Criterios de Aceite
- [ ] MCP server compila e roda
- [ ] 4 tools implementados + testes
- [ ] Kilo / Claude conseguem consultar docs
- [ ] Documentacao publicada

## Notas
- Padrao vem de antd-blazor (PR #4758, Fev 2026)
- MCP eh o padrao emergente para AI-tool integration
- Pode ser publicado como Docker image tambem
- Performance: cache docs em memoria
