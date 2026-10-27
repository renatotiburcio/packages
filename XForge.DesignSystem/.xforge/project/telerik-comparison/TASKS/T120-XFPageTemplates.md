---
id: T120
title: XFPageTemplates (pre-designed page templates)
priority: P3
status: pending
estimated_time: 16h
dependencies: T001, T002
---

# T120: XFPageTemplates

## Objetivo
7 templates pre-designed: Login, Dashboard, Settings, List, Form, Error, EmptyState.

## Passos
1. Criar 7 templates em `src/XForge.DesignSystem/Templates/`
2. Todos: theme-aware (T103), responsive, WCAG 2.2
3. Parametros simples (copy/paste friendly)
4. Documentacao + 1 demo por template

## Arquivos
- Criar: `src/XForge.DesignSystem/Templates/{Login,Dashboard,Settings,List,Form,Error,EmptyState}Template.razor` + `.razor.cs` (14 files)
- Criar: `docs/templates/overview.md`

## Criterios de Aceitacao
- [ ] 7 templates funcionais
- [ ] Theme-aware (T103)
- [ ] Responsive (mobile/tablet/desktop)
- [ ] WCAG 2.2
- [ ] Documentacao + 7 demos

## Notas
- Inspirado em Telerik Page Templates + Building Blocks
- Cobre 80% use cases LATAM