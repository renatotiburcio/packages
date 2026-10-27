---
id: T163
title: XFTabs
priority: P1
status: pending
estimated_time: 8h
dependencies: T001
---

# T163: XFTabs

## Objetivo
Tabbed container com scroll, closeable, lazy render e keyboard nav.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFTabs oferece tabbed container com scroll horizontal (overflow), tabs closeable (X icon), lazy render (so renderiza tab ativa), keyboard navigation (Arrow Left/Right/Home/End), ARIA tablist/tab/tabpanel roles.

## Passos
1. Estudar API DevExpress Tabs no docs.devexpress.com
2. Definir modelo XForge (XFTabs, XFTabPage, TabsOrientation)
3. Implementar lazy render via RenderFragment condicional
4. Renderizar ARIA tablist com keyboard handlers
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T163/XFTabs.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T163/XFTabPage.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T163/TabsOrientation.cs
- Criar: src/XForge.DesignSystem/Components/T163/KeyboardNavigationService.cs
- Criar: src/XForge.DesignSystem.Tests/T163TabsTests.cs
- Criar: docs/components/t163-tabs/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA (ARIA tablist/tab/tabpanel)
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap