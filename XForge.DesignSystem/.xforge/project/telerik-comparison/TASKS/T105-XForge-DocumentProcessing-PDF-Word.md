---
id: T105
title: XForge DocumentProcessing: PDF/Word libs (read+edit)
priority: P0
status: pending
estimated_time: 40h
dependencies: nenhuma
---

# T105: XForge DocumentProcessing: PDF/Word libs

## Objetivo
Adicionar libraries de leitura/edicao de PDF e Word (read+edit) ao XForge. Complementa T028 (QuestPDF generate).

## Passos
1. Criar projeto `XForge.DocumentProcessing.Pdf` (.NET 10, multi-TFM net8/9/10)
2. Wrapper sobre `PdfPig` (Apache 2.0) para read (extract text, images, metadata)
3. Wrapper sobre `PdfSharpCore` (MIT) para edit (merge, split, watermark, annotate)
4. API fluent: `XFPdfDocument.Open(stream)`, `.ExtractText()`, `.ToImage(pageNum)`
5. Criar projeto `XForge.DocumentProcessing.Word`
6. Wrapper sobre `DocumentFormat.OpenXml` (MIT) para read+edit Word
7. API fluent: `XFWordDocument.Open(stream)`, `.ReplaceText(old, new)`, `.AppendParagraph(text)`
8. Validar com suite de testes: PDFs com 10 cenarios (contratos, NF-e, faturas)
9. Documentacao com exemplos LATAM: contrato social, certificado, declaracao IR
10. Publicar como `XForge.DocumentProcessing.Pdf` + `XForge.DocumentProcessing.Word` NuGet

## Arquivos
- Criar: `src/XForge.DocumentProcessing.Pdf/XFPdfDocument.cs`
- Criar: `src/XForge.DocumentProcessing.Pdf/XFPdfPage.cs`
- Criar: `src/XForge.DocumentProcessing.Word/XFWordDocument.cs`
- Criar: `src/XForge.DocumentProcessing.Word/XFWordParagraph.cs`
- Criar: `tests/XForge.DocumentProcessing.Tests/` (40+ tests)
- Criar: `docs/document-processing/pdf.md` + `word.md`

## Criterios de Aceitacao
- [ ] Read PDF: extract text + imagens + metadata
- [ ] Edit PDF: merge, split, watermark, annotate, password protect
- [ ] Read Word: paragraphs, tables, images, headers/footers
- [ ] Edit Word: replace text, insert/delete paragraphs, formatacao basica
- [ ] Fluent API com task-based async (T009 CancellationToken)
- [ ] Testes com 10 PDFs + 10 Word docs reais
- [ ] Performance: PDF 100 paginas processado em <2s
- [ ] Documentacao LATAM com exemplos: contrato social, NFe, IR

## Notas
- Inspirado em Telerik PDFProcessing + WordsProcessing
- Alternativas open source: PdfPig + PdfSharpCore + DocumentFormat.OpenXml
- Complementa T028 (QuestPDF generate) com read+edit