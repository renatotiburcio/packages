---
id: T144
title: XFRichTextEditor-DOCX-RTF-TXT
priority: P0
status: pending
estimated_time: 40h
dependencies: T002
---

# T144: XFRichTextEditor-DOCX-RTF-TXT

## Objetivo
Estender o RichTextEditor com export DOCX/RTF/TXT e mail-merge v2.1.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
Adiciona capacidades de export multiplos formatos (DOCX via OpenXml, RTF nativo, TXT plain) ao RTE existente, alem de mail-merge v2.1 com templates, data source binding (JSON/CSV/EF) e geracao em batch.

## Passos
1. Estudar API DevExpress RichEdit no docs.devexpress.com
2. Adicionar pacote DocumentFormat.OpenXml (somente open source MIT)
3. Implementar DocxExporter, RtfExporter, TxtExporter
4. Implementar MailMerge engine v2.1 (templates, data binding, loops, conditionals)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T144/Exporters/DocxExporter.cs
- Criar: src/XForge.DesignSystem/Components/T144/Exporters/RtfExporter.cs
- Criar: src/XForge.DesignSystem/Components/T144/Exporters/TxtExporter.cs
- Criar: src/XForge.DesignSystem/Components/T144/MailMerge/MailMergeEngine.cs
- Criar: src/XForge.DesignSystem/Components/T144/MailMerge/MailMergeTemplate.cs
- Criar: src/XForge.DesignSystem/Components/T144/MailMerge/DataSourceProvider.cs
- Criar: src/XForge.DesignSystem.Tests/T144RichTextEditorExportTests.cs
- Criar: docs/components/t144-rte-export/index.md

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
- DocumentFormat.OpenXml e MIT license, livre para uso
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap