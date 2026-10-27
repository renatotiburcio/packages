---
id: T150
title: XFLayoutBreakpoint-responsive
priority: P1
status: pending
estimated_time: 8h
dependencies: T002
---

# T150: XFLayoutBreakpoint-responsive

## Objetivo
Componente responsivo com deteccao de viewport e breakpoints sm/md/lg/xl/2xl.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFLayoutBreakpoint detecta viewport size via JS interop, aplica classes Tailwind responsivas (sm/md/lg/xl/2xl), expoe CSS variables, debounce resize events e suporta server-side prerender hint via cookie.

## Passos
1. Estudar API DevExpress LayoutBreakpoint no docs.devexpress.com
2. Definir modelo XForge (XFLayoutBreakpoint, BreakpointSize enum)
3. Implementar JS interop com debounce 200ms
4. Persistir hint inicial em cookie para SSR
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T150/XFLayoutBreakpoint.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T150/BreakpointSize.cs
- Criar: src/XForge.DesignSystem/Components/T150/wwwroot/layoutBreakpoint.js
- Criar: src/XForge.DesignSystem/Components/T150/ViewportDetector.cs
- Criar: src/XForge.DesignSystem.Tests/T150LayoutBreakpointTests.cs
- Criar: docs/components/t150-layout-breakpoint/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10
- [ ] SSR-safe (sem flicker no primeiro paint)

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap