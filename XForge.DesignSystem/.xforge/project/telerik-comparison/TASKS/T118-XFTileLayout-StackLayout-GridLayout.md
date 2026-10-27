---
id: T118
title: XFTileLayout + XFStackLayout + XFGridLayout
priority: P2
status: pending
estimated_time: 20h
dependencies: nenhuma
---

# T118: XFTileLayout + XFStackLayout + XFGridLayout

## Objetivo
3 componentes de layout avancado para dashboards e apps complexos.

## Passos
1. Criar `XFTileLayout.razor`: Tiles redimensionaveis (drag borders), snap to grid, save/restore
2. Criar `XFStackLayout.razor`: Horizontal/vertical stack, spacing, alignment, wrap responsivo
3. Criar `XFGridLayout.razor`: CSS Grid wrapper, areas pre-definidas (header/sidebar/main/footer), responsive
4. Composicao com DockManager (T117)
5. Documentacao + 5 demos (dashboard, settings, login, etc.)

## Arquivos
- Criar: 3 .razor + 3 .razor.cs em `src/XForge.DesignSystem/Components/Layout/`
- Criar: `tests/XForge.DesignSystem.Tests/LayoutTests.cs` (20+ tests)
- Criar: `docs/layout/tile-stack-grid.md`

## Criterios de Aceitacao
- [ ] TileLayout: redimensionar, snap grid, save/restore
- [ ] StackLayout: H/V stack, spacing, alignment, wrap
- [ ] GridLayout: CSS Grid areas, responsive
- [ ] Composicao com DockManager (T117)
- [ ] Documentacao + 5 demos
- [ ] Testes: 20+

## Notas
- Inspirado em Telerik TileLayout, StackLayout, GridLayout
- Complementa Tailwind grid