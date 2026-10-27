---
id: T020
title: Padrao UnitTests + UnitTests.Viewer para testes complexos
priority: P2
status: pending
estimated_time: 16h
dependencies: T001
---

# T020: Adotar padrao UnitTests + UnitTests.Viewer

## Objetivo
Separar testes em dois projetos: UnitTests (bUnit puro) e UnitTests.Viewer (componentes de teste que precisam de render real).

## Passos
1. Criar `src/XForge.DesignSystem.UnitTests.Viewer/` para test components complexos
2. Reorganizar `src/XForge.DesignSystem.Tests/Components/` em pastas por componente
3. Regras: max 40 chars em nome de arquivo viewer; nunca cachear Find(); usar InvokeAsync() sempre
4. Testes async devem retornar Task, nunca async void
5. Documentar convencoes em `.xforge/knowledge/domains/design-system/testing-patterns.md`

## Arquivos
- Criar: `src/XForge.DesignSystem.UnitTests.Viewer/`
- Reorganizar: `src/XForge.DesignSystem.Tests/Components/<X>/<X>Tests.cs`
- Criar: `src/XForge.DesignSystem.UnitTests.Shared/` (helper test code)

## Criterios de Aceite
- [ ] UnitTests.Viewer compila e roda
- [ ] Testes existentes migrados para nova estrutura
- [ ] Coverage nao cai
- [ ] Documento de convencoes publicado

## Notas
- MudBlazor tem 3 test projects (UnitTests, Viewer, Shared)
- Viewer components sao Razor files com TaskCompletionSource gates
