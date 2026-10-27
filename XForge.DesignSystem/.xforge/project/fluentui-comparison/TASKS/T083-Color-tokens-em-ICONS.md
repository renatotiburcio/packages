---
id: T083
title: Color tokens em ICONS (TokenStyle-aware CSS)
priority: P1
status: pending
estimated_time: 8h
dependencies: T078, T001
---

# T083: Color tokens em ICONS (TokenStyle-aware CSS)

## Objetivo
Integrar XFIcon com XForge Design Tokens (T001): Accent/Success/Warning/Error/Important via CSS variables, theme-aware (light/dark).

## Passos
1. Mapear XFIconColor enum → CSS custom properties:
   - Accent → var(--xf-color-accent)
   - Lightweight → var(--xf-color-text-secondary)
   - Success → var(--xf-color-success)
   - Warning → var(--xf-color-warning)
   - Error → var(--xf-color-error)
   - Important → var(--xf-color-important)
   - Custom → var(--xf-icon-custom-color) + set prop
2. Adicionar light/dark theme support (media query prefers-color-scheme)
3. Adicionar :hover/:focus state classes (.xf-icon-interactive)
4. Documentar tokens em XForge.DesignSystem/DesignTokens/IconColors.md
5. Atualizar xf-designsystem.css com --xf-icon-* variables
6. Testes: rendering em light/dark mode (Playwright snapshot)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconColor.cs`
- Modificar: `src/XForge.DesignSystem/wwwroot/css/xf-designsystem.css` (icon color tokens)
- Criar: `src/XForge.DesignSystem/DesignTokens/IconColors.md`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFFluentIcon.razor.cs` (use CSS variables)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconColorTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] 6 colors default (Accent/Lightweight/Success/Warning/Error/Important)
- [ ] Custom color via CSS variable
- [ ] Light theme (default) + dark theme (prefers-color-scheme)
- [ ] Hover/Focus state styles (.xf-icon-interactive)
- [ ] Theme switching: 0 erros, smooth transition
- [ ] Tokens documentados em MD
- [ ] Tests: 8+ (cada color + theme switching)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Baseado em FluentUI Color (AspNetCore.Components.Color)
- Tailwind v4 tem design tokens: integrar com classes Tailwind
- Dark mode: prefers-color-scheme + manual toggle (T062 theme service)
