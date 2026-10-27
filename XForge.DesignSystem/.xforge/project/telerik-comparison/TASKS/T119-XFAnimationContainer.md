---
id: T119
title: XFAnimationContainer (entrance/exit CSS animations)
priority: P2
status: pending
estimated_time: 8h
dependencies: T002
---

# T119: XFAnimationContainer

## Objetivo
Wrapper que adiciona animations de entrada/saida (fade, slide, scale) via CSS.

## Passos
1. Criar `XFAnimationContainer.razor`
2. Tipos: FadeIn, FadeOut, SlideIn (Left/Right/Up/Down), ScaleIn/Out (8 tipos)
3. Duration: 100-2000ms, Delay: 0-5000ms
4. Easing: ease, ease-in, ease-out, ease-in-out, linear
5. Trigger: OnAppear (Intersection Observer), OnClick, OnHover
6. prefers-reduced-motion respect (a11y)
7. Stagger: animar children com delay incremental
8. Documentacao + 5 demos

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Animation/XFAnimationContainer.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Animation/AnimationType.cs`
- Criar: `tests/XForge.DesignSystem.Tests/AnimationTests.cs` (10 tests)
- Criar: `docs/animation/getting-started.md`

## Criterios de Aceitacao
- [ ] 8 tipos de animation
- [ ] Duration 100-2000ms, Delay 0-5000ms
- [ ] 5 easings, 3 triggers
- [ ] prefers-reduced-motion respect
- [ ] Stagger children
- [ ] Documentacao + 5 demos

## Notas
- Inspirado em Telerik AnimationContainer
- CSS-based (zero JS)