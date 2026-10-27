---
id: T098
title: Animated icons (CSS/SMIL)
priority: P3
status: pending
estimated_time: 6h
dependencies: T078
---

# T098: Animated icons (CSS/SMIL)

## Objetivo
Suporte a icones animados (spinner, pulse, rotate) via CSS animations ou SMIL. Inclui biblioteca de 20+ icones animados pre-definidos.

## Passos
1. Criar XFIconAnimation enum { None, Spin, Pulse, Rotate, Bounce }
2. Modificar XFFluentIcon.razor.cs: parametro Animation (XFIconAnimation)
3. Renderizar classe CSS correspondente (.xf-icon-spin, .xf-icon-pulse, etc.)
4. Adicionar CSS keyframes em xf-designsystem.css:
   - @keyframes xf-spin { from {transform: rotate(0deg);} to {transform: rotate(360deg);} }
   - @keyframes xf-pulse { 0%,100% {opacity: 1;} 50% {opacity: 0.5;} }
   - @keyframes xf-rotate { 0%,100% {transform: rotate(0deg);} 50% {transform: rotate(180deg);} }
   - @keyframes xf-bounce { 0%,100% {transform: translateY(0);} 50% {transform: translateY(-8px);} }
5. Criar 20+ icones animados pre-definidos (loader, checkmark-success, alert-warning, etc.)
6. Suportar custom animation via AnimationFrame CSS (advanced)
7. Accessibility: prefers-reduced-motion (pausa animacao)
8. Tests: 12+ (animacao classes, reduced motion, custom animation)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconAnimation.cs`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor.cs`
- Modificar: `src/XForge.DesignSystem/wwwroot/css/xf-designsystem.css` (keyframes)
- Criar: `src/XForge.DesignSystem/Components/Icon/AnimatedIcons.cs` (20+ pre-defined)
- Modificar: `src/XForge.DesignSystem/wwwroot/css/xf-designsystem.css` (reduced motion)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconAnimationTests.cs` (12+ tests)

## Criterios de Aceite
- [ ] 5 animations (None/Spin/Pulse/Rotate/Bounce)
- [ ] 20+ icones animados pre-definidos
- [ ] CSS keyframes implementados
- [ ] prefers-reduced-motion respeitado
- [ ] Custom animation via CSS class
- [ ] Tests: 12+ (animations, reduced motion, custom)
- [ ] Build: 0 erros, 0 warnings

## Notas
- CSS animations preferiveis (GPU-accelerated, mais simples)
- SMIL deprecated em alguns browsers
- Reduced motion: media query essencial para a11y
