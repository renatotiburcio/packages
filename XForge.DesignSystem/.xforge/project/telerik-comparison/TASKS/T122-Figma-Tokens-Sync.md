---
id: T122
title: Figma Tokens Sync (push/pull design tokens)
priority: P3
status: pending
estimated_time: 16h
dependencies: T103
---

# T122: Figma Tokens Sync

## Objetivo
Sincronizar design tokens XForge (25 enums) com Figma Variables via Figma API. Push e Pull bidirecional.

## Passos
1. Criar `XForge.ThemeBuilder.FigmaSync.Cli` (.NET tool)
2. Autenticacao: FIGMA_TOKEN env var
3. Figma Variables API: listar/ler variables
4. Mapping: XForge Token ↔ Figma Variable
5. Push: ler tokens XForge (JSON), upload para Figma
6. Pull: ler Figma Variables, gerar tokens XForge
7. Diff visual antes de aplicar
8. CI/CD: `xforge-figma sync --direction=push`
9. Validate command
10. Documentacao + video tutorial

## Arquivos
- Criar: `tools/XForge.ThemeBuilder.FigmaSync/Program.cs`
- Criar: `tools/XForge.ThemeBuilder.FigmaSync/Commands/SyncCommand.cs` + `ValidateCommand.cs`
- Criar: `tools/XForge.ThemeBuilder.FigmaSync/Services/FigmaApiClient.cs`
- Criar: `tools/XForge.ThemeBuilder.FigmaSync/Services/TokenMapper.cs`
- Criar: `tests/XForge.ThemeBuilder.FigmaSync.Tests/`
- Criar: `docs/figma-sync/getting-started.md`

## Criterios de Aceitacao
- [ ] Autenticacao FIGMA_TOKEN
- [ ] Push XForge → Figma
- [ ] Pull Figma → XForge
- [ ] Diff visual
- [ ] CI/CD integration
- [ ] Validate inconsistencies
- [ ] Documentacao + video
- [ ] Testes: 15+

## Notas
- Inspirado em Telerik Figma Kits sync
- Design system round-trip XForge ↔ Figma