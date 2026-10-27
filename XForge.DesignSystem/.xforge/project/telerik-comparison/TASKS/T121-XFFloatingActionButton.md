---
id: T121
title: XFFloatingActionButton (Material FAB pattern)
priority: P3
status: pending
estimated_time: 4h
dependencies: nenhuma
---

# T121: XFFloatingActionButton

## Objetivo
Botao flutuante (Material Design FAB) com speed dial opcional.

## Passos
1. Criar `XFFloatingActionButton.razor`
2. Posicoes: BottomRight, BottomLeft, TopRight, TopLeft
3. Tamanhos: Mini (40px), Default (56px), Extended (com label)
4. Cor: Primary, Secondary, Custom
5. Icone: XFFluentIcon ou custom
6. Animacao entrada (scale)
7. Speed dial: expande 2-5 acoes
8. Hide on scroll down, show on scroll up
9. Documentacao + 3 demos

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/FAB/XFFloatingActionButton.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/FAB/Enums/FabPosition.cs` + `FabSize.cs`
- Criar: `tests/XForge.DesignSystem.Tests/FloatingActionButtonTests.cs` (8 tests)
- Criar: `docs/fab/getting-started.md`

## Criterios de Aceitacao
- [ ] 4 posicoes, 3 tamanhos
- [ ] Cores (Primary/Secondary/Custom)
- [ ] Speed dial
- [ ] Hide/show on scroll
- [ ] A11y: aria-label
- [ ] Documentacao + 3 demos
- [ ] Testes: 8

## Notas
- Material Design FAB pattern
- 4h effort, alto valor UX