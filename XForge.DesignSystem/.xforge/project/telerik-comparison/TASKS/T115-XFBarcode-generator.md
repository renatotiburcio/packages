---
id: T115
title: XFBarcode generator (1D + 2D)
priority: P2
status: pending
estimated_time: 8h
dependencies: T114
---

# T115: XFBarcode generator

## Objetivo
Componente para gerar codigos de barra 1D (EAN, UPC, Code128) e 2D (PDF417, DataMatrix).

## Passos
1. Criar `XForge.DesignSystem/Components/Barcode/XFBarcode.razor`
2. Wrapper sobre `ZXing.Net` (Apache 2.0) para generate
3. Suporte 1D: EAN-8, EAN-13, UPC-A, UPC-E, Code 39, Code 93, Code 128, ITF, Codabar
4. Suporte 2D: PDF417, DataMatrix, Aztec
5. SVG output
6. Parametros: Value, Format, Width, Height, ShowLabel
7. Boleto bancario BR: linha digitavel → barcode
8. Integracao com T105 (gerar barcode em PDF)
9. Documentacao + 5 demos

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Barcode/XFBarcode.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Barcode/BarcodeFormat.cs`
- Criar: `tests/XForge.DesignSystem.Tests/BarcodeTests.cs` (12 tests)
- Criar: `docs/barcode/getting-started.md`

## Criterios de Aceitacao
- [ ] 9 formatos 1D (EAN/UPC/Code39/Code93/Code128/ITF/Codabar)
- [ ] 3 formatos 2D (PDF417/DataMatrix/Aztec)
- [ ] SVG output
- [ ] Boleto bancario BR: linha digitavel → barcode
- [ ] Custom Width/Height/ShowLabel
- [ ] Acessibilidade: aria-label
- [ ] Documentacao + 5 demos
- [ ] Testes: 12 (1 por formato)

## Notas
- Inspirado em Telerik Barcode
- ZXing.Net = melhor opcao Apache 2.0
- Boleto BR: integracao com T107 (CNAB) futuro