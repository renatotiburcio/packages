---
id: T154
title: XFReportViewer
priority: P2
status: pending
estimated_time: 16h
dependencies: T105
---

# T154: XFReportViewer

## Objetivo
End-user report viewer (read-only) que renderiza T105 (PDF/Word) com paginacao e search.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFReportViewer oferece visualizador end-user (read-only) de relatorios T105, com paginacao, zoom in/out/fit, search, print, export (PDF/Word/Excel), multi-page, document map (TOC).

## Passos
1. Estudar API DevExpress ReportViewer no docs.devexpress.com
2. Definir modelo XForge (XFReportViewer, ReportPage, ReportSearchResult)
3. Implementar paginacao virtual com lazy load
4. Renderizar document map (TOC tree)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T154/XFReportViewer.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T154/ReportPage.cs
- Criar: src/XForge.DesignSystem/Components/T154/ReportSearchResult.cs
- Criar: src/XForge.DesignSystem/Components/T154/ReportSearchService.cs
- Criar: src/XForge.DesignSystem/Components/T154/ZoomController.cs
- Criar: src/XForge.DesignSystem.Tests/T154ReportViewerTests.cs
- Criar: docs/components/t154-report-viewer/index.md

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
- DEPENDE de T105 (XFReportEngine) para geracao de relatorios
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap