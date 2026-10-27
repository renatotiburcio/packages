---
id: T106
title: XFFullSpreadsheet component (Excel-like + formulas)
priority: P0
status: pending
estimated_time: 48h
dependencies: nenhuma
---

# T106: XFFullSpreadsheet component

## Objetivo
Componente Blazor tipo Excel com formulas, formatacao, charts, multi-sheet. Diferencial viral para XForge.

## Passos
1. Criar projeto `XForge.DesignSystem.Components.Spreadsheet`
2. Engine de grid virtual (renderiza apenas celulas visiveis) — performance
3. Suporte a formulas: SUM, AVERAGE, IF, VLOOKUP, INDEX/MATCH, COUNTIF, etc.
4. Avaliador de formulas custom (parser recursivo descendente, ~30h de formulas core)
5. Formatacao: number (currency/percent/date), font, fill, border, alignment
6. Multi-sheet (tabs no rodape)
7. Copy/paste via clipboard (TSV/HTML)
8. Freeze panes, sort, filter, fill handle
9. Salvar/carregar em `.xlsx` (via T107 SpreadProcessing) ou JSON
10. Charts inline (sparkline + bar + line)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Spreadsheet/XFSpreadsheet.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Spreadsheet/Engine/FormulaEngine.cs`
- Criar: `src/XForge.DesignSystem/Components/Spreadsheet/Engine/CellRenderer.cs`
- Criar: `src/XForge.DesignSystem/Components/Spreadsheet/Engine/SheetModel.cs`
- Criar: `src/XForge.DesignSystem/Components/Spreadsheet/Formats/XlsxSerializer.cs`
- Criar: `tests/XForge.DesignSystem.Tests/SpreadsheetTests.cs` (30+ tests)
- Criar: `docs/spreadsheet/getting-started.md`

## Criterios de Aceitacao
- [ ] Renderizar grid 1000x1000 com scroll fluido (60fps)
- [ ] 30+ formulas core funcionando (SUM, AVERAGE, IF, VLOOKUP, etc.)
- [ ] Recalculo automatico ao editar celula
- [ ] Formatacao currency/percent/date
- [ ] Multi-sheet com add/remove/rename
- [ ] Copy/paste via clipboard
- [ ] Salvar/carregar `.xlsx` (round-trip com T107)
- [ ] Acessibilidade WCAG 2.2 (navegacao por teclado)
- [ ] Documentacao + 10 demos

## Notas
- Inspirado em Telerik Spreadsheet (referencia maxima)
- Diferencial viral: componente Excel-like em MIT e open source
- Complexidade alta (48h) mas ROI comprovado
- Avaliador de formulas: ~30h para 30 formulas + parser