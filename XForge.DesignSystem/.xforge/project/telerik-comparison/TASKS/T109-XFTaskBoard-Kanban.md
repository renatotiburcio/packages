---
id: T109
title: XFTaskBoard (Kanban drag/drop)
priority: P1
status: pending
estimated_time: 16h
dependencies: T010, T020
---

# T109: XFTaskBoard (Kanban)

## Objetivo
Componente Kanban com colunas customizaveis e drag/drop de cards.

## Passos
1. Criar `XForge.DesignSystem/Components/TaskBoard/XFTaskBoard.razor`
2. Colunas: Backlog, In Progress, Review, Done (customizaveis)
3. Cards: title, description, assignee, due date, tags, priority
4. Drag/drop HTML5 nativo (ou interop com SortableJS)
5. Eventos: OnCardMove, OnCardAdd, OnCardUpdate
6. Virtualizacao (renderizar apenas cards visiveis)
7. Filtros: por assignee, tag, priority
8. WIP limits por coluna
9. Integracao com T021 Docs.Compiler (gerar taskboards em docs)
10. Tema custom via CSS variables (T103)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/TaskBoard/XFTaskBoard.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/TaskBoard/XFTaskCard.razor`
- Criar: `src/XForge.DesignSystem/Components/TaskBoard/XFTaskColumn.razor`
- Criar: `tests/XForge.DesignSystem.Tests/TaskBoardTests.cs` (12 tests)
- Criar: `docs/task-board/getting-started.md`

## Criterios de Aceitacao
- [ ] Drag/drop fluido entre colunas
- [ ] 4 colunas pre-definidas (customizaveis)
- [ ] Card properties: title, description, assignee, due, tags, priority
- [ ] Eventos: OnCardMove, OnCardAdd, OnCardUpdate
- [ ] Virtualizacao para 1000+ cards
- [ ] Filtros funcionais
- [ ] WIP limits por coluna
- [ ] Acessibilidade WCAG 2.2
- [ ] Documentacao + 3 demos

## Notas
- Inspirado em Telerik TaskBoard (Novo)
- Diferencial: open source + WCAG compliant
- Drag/drop: HTML5 nativo (sem dependencia externa)