---
id: T155
title: XFBIDashboard
priority: P3
status: pending
estimated_time: 32h
dependencies: T154,T112
---

# T155: XFBIDashboard

## Objetivo
BI Dashboard combinando T154 ReportViewer + T112 Charts em layout com drag-drop builder.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFBIDashboard combina T154 (report viewer) + T112 (charts) em layout dashboard com cards/widgets/grid, drag-drop builder end-user, data source binding (SQL/JSON/CSV/EF), e scheduled refresh.

## Passos
1. Estudar API DevExpress Dashboard no docs.devexpress.com
2. Definir modelo XForge (XFBIDashboard, DashboardWidget, DashboardLayout)
3. Implementar grid layout com drag-drop (CSS Grid + JS interop)
4. Implementar data source refresh scheduler
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T155/XFBIDashboard.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T155/DashboardWidget.cs
- Criar: src/XForge.DesignSystem/Components/T155/DashboardLayout.cs
- Criar: src/XForge.DesignSystem/Components/T155/DataSourceBindingService.cs
- Criar: src/XForge.DesignSystem/Components/T155/RefreshScheduler.cs
- Criar: src/XForge.DesignSystem/Components/T155/wwwroot/dashboard.js
- Criar: src/XForge.DesignSystem.Tests/T155BIDashboardTests.cs
- Criar: docs/components/t155-bi-dashboard/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- DEPENDE de T154 (ReportViewer) e T112 (Charts)
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap