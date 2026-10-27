---
id: T114
title: XFQRCode generator
priority: P2
status: pending
estimated_time: 6h
dependencies: T078
---

# T114: XFQRCode generator

## Objetivo
Componente para gerar QR Codes (PIX, URLs, vCards, WiFi).

## Passos
1. Criar `XForge.DesignSystem/Components/QRCode/XFQRCode.razor`
2. Wrapper sobre `Net.Codecrete.QrCodeGenerator` (BSD-2)
3. Parametros: Value, Size, ErrorCorrection, ForegroundColor, BackgroundColor
4. SVG output (zero dependencia JS)
5. Suporte a tipos comuns: URL, Email, Phone, SMS, WiFi, vCard, PIX (BR)
6. Integracao com T105 (gerar QRCode em PDF)
7. Documentacao + 5 demos (PIX, URL, WiFi, vCard, Email)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/QRCode/XFQRCode.razor` + `.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/QRCodeTests.cs` (8 tests)
- Criar: `docs/qrcode/getting-started.md`

## Criterios de Aceitacao
- [ ] 4 error correction levels
- [ ] Custom foreground/background colors
- [ ] Tipos: URL, Email, Phone, SMS, WiFi, vCard, PIX
- [ ] SVG output (zero JS)
- [ ] Acessibilidade: aria-label
- [ ] Documentacao + 5 demos
- [ ] Testes: 8 (1 por tipo + edge cases)

## Notas
- Inspirado em Telerik QR Code
- Net.Codecrete.QrCodeGenerator = melhor opcao MIT