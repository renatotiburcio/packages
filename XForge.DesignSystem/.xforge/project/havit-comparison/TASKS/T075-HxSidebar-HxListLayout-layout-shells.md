---
id: T075
title: HxSidebar HxListLayout layout shells
priority: P1
status: pending
estimated_time: 8h
dependencies: T067
origin: Havit
---

# T075: HxSidebar HxListLayout layout shells

## Objetivo
Implementar XFSidebar (app-level nav) + XFListLayout (page shell w/ filter in offcanvas) como layout shells reutilizaveis. Pattern Havit.

## Passos
1. Criar XFSidebar.razor + .razor.cs + .razor.css (collapsible app-level nav)
2. Adicionar [Parameter] public RenderFragment Brand (logo)
3. Adicionar [Parameter] public RenderFragment ChildContent (menu items)
4. Adicionar [Parameter] public RenderFragment Footer (user info)
5. Adicionar [Parameter] public SidebarResponsiveBreakpoint Breakpoint
6. Criar XFSidebarItem.razor (item with ShouldMatch.cs route detection)
7. Criar XFSidebarBrand, XFSidebarFooter (.razor.cs w/ template contexts)
8. Criar XFListLayout.razor + .razor.cs + .razor.css (page shell: filter+grid+offcanvas)
9. Adicionar [Parameter] public RenderFragment FilterContent (filter form)
10. Adicionar [Parameter] public RenderFragment DataContent (grid/list)
11. Adicionar [Parameter] public RenderFragment ActionContent (buttons)
12. Adicionar NamedView class para named layout regions
13. Criar ILayoutColumnComponent interface (segmentation)

## Arquivos
- Criar: src/XForge.DesignSystem/Components/Layouts/XFSidebar.razor + .cs + .css
- Criar: src/XForge.DesignSystem/Components/Layouts/XFSidebarItem.razor + .razor.ShouldMatch.cs
- Criar: src/XForge.DesignSystem/Components/Layouts/XFSidebarBrand.razor + .razor.cs + .razor.css
- Criar: src/XForge.DesignSystem/Components/Layouts/XFSidebarFooter.razor + .razor.cs + .razor.css
- Criar: src/XForge.DesignSystem/Components/Layouts/XFListLayout.razor + .razor.cs + .razor.css
- Criar: src/XForge.DesignSystem/Components/Layouts/SidebarResponsiveBreakpoint.cs
- Criar: src/XForge.DesignSystem/Components/Layouts/NamedView.cs
- Criar: src/XForge.DesignSystem/Components/Layouts/ILayoutColumnComponent.cs

## Criterios de Aceite
- [ ] XFSidebar renderiza com brand, items, footer
- [ ] XFSidebarItem suporta ShouldMatch (active route highlight)
- [ ] XFListLayout shell funcional (filter+grid+offcanvas)
- [ ] Responsive breakpoint funciona (collapse em mobile)
- [ ] Testes bUnit: 5 layout scenarios
