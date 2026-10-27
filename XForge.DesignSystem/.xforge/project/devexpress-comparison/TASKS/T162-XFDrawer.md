---
id: T162
title: XFDrawer
priority: P1
status: pending
estimated_time: 12h
dependencies: T001
---

# T162: XFDrawer

## Objetivo
Off-canvas navigation (L/R/T/B) com overlay, swipe, focus trap e SSR-safe.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFDrawer oferece navigation off-canvas em 4 posicoes (Left/Right/Top/Bottom), overlay/backdrop, swipe gestures (mobile via touch events), ESC key dismiss, focus trap, ARIA dialog roles, e SSR-safe (renders closed state em SSR).

## Passos
1. Estudar API DevExpress Drawer no docs.devexpress.com
2. Definir modelo XForge (XFDrawer, DrawerPosition, DrawerState)
3. Implementar focus trap via JS interop
4. Implementar touch swipe handlers
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T162/XFDrawer.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T162/DrawerPosition.cs
- Criar: src/XForge.DesignSystem/Components/T162/DrawerState.cs
- Criar: src/XForge.DesignSystem/Components/T162/wwwroot/drawer.js
- Criar: src/XForge.DesignSystem/Components/T162/FocusTrapService.cs
- Criar: src/XForge.DesignSystem.Tests/T162DrawerTests.cs
- Criar: docs/components/t162-drawer/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA (ARIA dialog, focus trap)
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10
- [ ] SSR-safe (no flicker no prerender)

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap