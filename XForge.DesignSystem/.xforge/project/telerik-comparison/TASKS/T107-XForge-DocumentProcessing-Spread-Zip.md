---
id: T107
title: XForge DocumentProcessing: SpreadProcessing + Zip
priority: P1
status: pending
estimated_time: 24h
dependencies: T105
---

# T107: XForge DocumentProcessing: SpreadProcessing + Zip

## Objetivo
Adicionar libraries para ler/editar planilhas Excel (.xlsx) e arquivos Zip. Round-trip com T106 Spreadsheet.

## Passos
1. Criar projeto `XForge.DocumentProcessing.Spread`
2. Wrapper sobre `ClosedXML` (MIT) para read+edit .xlsx
3. API fluent: `XFSpreadDocument.Open(stream)`, `.GetWorksheet(name)`, `.SetCell(row,col,value)`
4. Criar projeto `XForge.DocumentProcessing.Zip`
5. Wrapper sobre `SharpZipLib` (MIT) para read+write zip/7z
6. API fluent: `XFZipArchive.Open(stream)`, `.ExtractAll(path)`, `.AddFile(name, bytes)`
7. Streaming variant (SpreadStreamProcessing) para arquivos >100MB
8. Integrar com T106 (round-trip save/load .xlsx)
9. Testes com arquivos reais: planilhas bancarias LATAM, faturas
10. Publicar como NuGet packages separados

## Arquivos
- Criar: `src/XForge.DocumentProcessing.Spread/XFSpreadDocument.cs`
- Criar: `src/XForge.DocumentProcessing.Spread/XFWorksheet.cs`
- Criar: `src/XForge.DocumentProcessing.Zip/XFZipArchive.cs`
- Criar: `tests/XForge.DocumentProcessing.Tests/SpreadTests.cs` (15 tests)
- Criar: `tests/XForge.DocumentProcessing.Tests/ZipTests.cs` (10 tests)

## Criterios de Aceitacao
- [ ] Read .xlsx: worksheets, cells, formulas, formatacao
- [ ] Edit .xlsx: add/delete rows, formatacao, formulas
- [ ] Streaming read para arquivos >100MB
- [ ] Zip: read, write, extract, password protect
- [ ] Round-trip com T106 Spreadsheet
- [ ] Testes com 10 planilhas reais (bancarias, faturas, NFe)
- [ ] Documentacao LATAM

## Notas
- Complementa T105 (PDF/Word) e T106 (Spreadsheet)
- ClosedXML e a melhor opcao open source para .xlsx