---
id: T123
title: XFFigma Bridge (live Figma embed in docs)
priority: P3
status: pending
estimated_time: 12h
dependencies: T122
---

# T123: XFFigma Bridge

## Objetivo
Embed Figma frames ao vivo dentro da documentacao XForge (Telerik Embed kit style).

## Passos
1. Criar `XFFigmaBridge.razor`
2. Input: Figma File Key + Node ID
3. Renderizar via Figma embed iframe
4. Parametros: Width, Height, ShowUI
5. Lazy load (Intersection Observer)
6. Tabs: Design | Code (mostra codigo XForge via T102 MCP)
7. Deep link: Figma element → XForge doc
8. Documentacao + 3 demos

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Figma/XFFigmaBridge.razor` + `.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/FigmaBridgeTests.cs` (8 tests)
- Criar: `docs/figma-bridge/getting-started.md`

## Criterios de Aceitacao
- [ ] Embed Figma iframe
- [ ] Parametros Width/Height/ShowUI
- [ ] Lazy load
- [ ] Tabs Design/Code
- [ ] Deep link Figma → XForge doc
- [ ] A11y: aria-label
- [ ] Documentacao + 3 demos
- [ ] Testes: 8

## Notas
- Inspirado em Telerik Figma Kits (Embed kit)
- Complementa T122 (sync)
- Live preview do design system no Figma