---
id: T157
title: XFReportDesigner
priority: P3
status: pending
estimated_time: 40h
dependencies: T105
---

# T157: XFReportDesigner

## Objetivo
Ad-hoc report designer end-user com drag-drop data sources, 20+ controls e drill-down.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFReportDesigner oferece designer end-user de relatorios com drag-drop data sources (SQL/JSON/Excel/EF/MongoDB), 20+ controls (tables, charts, labels, shapes, etc), calculated fields, summaries, drill-down e localization pt-BR/en-US.

## Passos
1. Estudar API DevExpress ReportDesigner no docs.devexpress.com
2. Definir modelo XForge (XFReportDesigner, ReportTemplate, DataSourceConfig)
3. Implementar 20+ controls base (TextBox, Table, Chart, Image, Shape, etc)
4. Implementar calculated field engine e summary aggregations
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T157/XFReportDesigner.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T157/ReportTemplate.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceConfig.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceProviders/SqlDataSourceProvider.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceProviders/JsonDataSourceProvider.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceProviders/ExcelDataSourceProvider.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceProviders/EfDataSourceProvider.cs
- Criar: src/XForge.DesignSystem/Components/T157/DataSourceProviders/MongoDataSourceProvider.cs
- Criar: src/XForge.DesignSystem/Components/T157/Controls/*.cs (20+ controls)
- Criar: src/XForge.DesignSystem/Components/T157/CalculatedFieldEngine.cs
- Criar: src/XForge.DesignSystem/Components/T157/SummaryEngine.cs
- Criar: src/XForge.DesignSystem/Components/T157/LocalizationService.cs
- Criar: src/XForge.DesignSystem.Tests/T157ReportDesignerTests.cs
- Criar: docs/components/t157-report-designer/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10
- [ ] 20+ controls implementados

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- DEPENDE de T105 (XFReportEngine) para renderizacao final
- MongoDB driver: usar pacote oficial MongoDB.Driver (Apache 2.0)
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap