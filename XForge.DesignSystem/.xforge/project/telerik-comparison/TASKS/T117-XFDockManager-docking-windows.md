---
id: T117
title: XFDockManager (docking windows, IDE-like)
priority: P2
status: pending
estimated_time: 32h
dependencies: nenhuma
---

# T117: XFDockManager

## Objetivo
Componente DockManager tipo Visual Studio (drag/dock panels com tabs).

## Passos
1. Criar `XForge.DesignSystem/Components/DockManager/XFDockManager.razor`
2. Layout: 5 regioes (center, left, right, top, bottom)
3. Panels: arrastaveis entre regioes
4. Tabs dentro de cada region
5. Split horizontal/vertical
6. Floating panels (popup sobre layout)
7. Maximize/Restore/Minimize
8. Save/restore layout (JSON serialize)
9. Resize splitters
10. Context menu (close, close all, close others)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/DockManager/XFDockManager.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/DockManager/Models/DockPanel.cs` + `DockLayout.cs`
- Criar: `src/XForge.DesignSystem/Components/DockManager/Engine/LayoutEngine.cs`
- Criar: `tests/XForge.DesignSystem.Tests/DockManagerTests.cs` (15+ tests)
- Criar: `docs/dock-manager/getting-started.md`

## Criterios de Aceitacao
- [ ] 5 regioes (center/left/right/top/bottom)
- [ ] Drag/dock panels
- [ ] Tabs por region
- [ ] Split horizontal/vertical
- [ ] Floating panels
- [ ] Maximize/Restore/Minimize
- [ ] Save/restore layout (JSON)
- [ ] Resize splitters
- [ ] Context menu
- [ ] Acessibilidade WCAG 2.2
- [ ] Documentacao + 3 demos

## Notas
- Inspirado em Telerik DockManager
- IDE-like: ideal para admin panels, dashboards complexos
- Layout JSON serializable para user preferences