---
id: T103
title: ThemeBuilder Web App + CLI (CSS variables generator)
priority: P0
status: pending
estimated_time: 32h
dependencies: nenhuma
---

# T103: ThemeBuilder Web App + CLI

## Objetivo
Web app para gerar temas visuais do XForge via CSS variables + CLI .NET tool para integracao em build pipelines.

## Passos
1. Criar projeto `XForge.ThemeBuilder.Web` (Blazor WASM standalone, .NET 10)
2. UI: Color pickers para 25 design token enums (Spacing, Radius, FontSize, Color, etc.)
3. Preview live de componentes XForge renderizados com tema custom
4. Export para CSS variables + `appsettings.json` (XForge theme)
5. Criar projeto `XForge.ThemeBuilder.Cli` (.NET tool, `dotnet tool install xforge-theme`)
6. CLI: `xforge theme generate --input theme.json --output wwwroot/css/`
7. CLI: `xforge theme validate --input theme.json`
8. Suporte a 4 temas pre-definidos (Default, Dark, HighContrast, Branded)
9. WebMCP-compatible: agents podem ler/editar temas via MCP (integrar com T102)

## Arquivos
- Criar: `src/XForge.ThemeBuilder.Web/Program.cs` + paginas
- Criar: `src/XForge.ThemeBuilder.Web/Components/ColorPicker.razor`
- Criar: `src/XForge.ThemeBuilder.Cli/Program.cs`
- Criar: `src/XForge.ThemeBuilder.Cli/Commands/GenerateCommand.cs`
- Criar: `tests/XForge.ThemeBuilder.Cli.Tests/`

## Criterios de Aceitacao
- [ ] Web app roda em `localhost:5000` via `dotnet run`
- [ ] Usuario pode editar 25 tokens e ver preview live
- [ ] Export gera arquivo CSS + JSON valido
- [ ] CLI roda em CI/CD: `xforge theme generate --input theme.json --output wwwroot/css/`
- [ ] 4 temas pre-definidos: Default, Dark, HighContrast, Branded
- [ ] WebMCP: tools `get_theme_tokens` e `get_design_token` funcionam
- [ ] Documentacao em `docs/theme-builder/`
- [ ] Testes E2E com Playwright (5 cenarios)

## Notas
- Inspirado em Telerik ThemeBuilder (https://themebuilder.telerik.com)
- Diferencial: open source + self-hosted + CLI
- Figma sync: complementado por T122