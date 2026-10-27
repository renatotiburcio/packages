---
id: T116
title: XFSignature pad (handwritten signature capture)
priority: P2
status: pending
estimated_time: 12h
dependencies: nenhuma
---

# T116: XFSignature pad

## Objetivo
Componente para capturar assinatura manuscrita (canvas) e exportar como SVG/PNG.

## Passos
1. Criar `XForge.DesignSystem/Components/Signature/XFSignaturePad.razor`
2. Canvas HTML5 com drawing API
3. Mouse + touch + pen input
4. Smooth bezier interpolation
5. Undo/Redo stack
6. Clear button
7. Export: SVG (path) ou PNG (canvas.toDataURL)
8. Storage: base64 inline, ou upload via T027 (Upload component)
9. Stamp: timestamp + IP + user agent (LGPD)
10. Integracao com T105 (incorporar em PDF de contrato)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Signature/XFSignaturePad.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Signature/Engine/SignaturePath.cs`
- Criar: `src/XForge.DesignSystem/Components/Signature/Engine/BezierInterpolator.cs`
- Criar: `tests/XForge.DesignSystem.Tests/SignaturePadTests.cs` (10 tests)
- Criar: `docs/signature-pad/getting-started.md`

## Criterios de Aceitacao
- [ ] Mouse + touch + pen input
- [ ] Smooth bezier interpolation
- [ ] Undo/Redo stack
- [ ] Clear button
- [ ] Export SVG/PNG
- [ ] Upload via T027 opcional
- [ ] LGPD stamp (timestamp + IP + user agent)
- [ ] Integracao com T105 (PDF embed)
- [ ] Acessibilidade: keyboard navigation
- [ ] Documentacao + 2 demos

## Notas
- Inspirado em Telerik Signature
- Canvas HTML5 nativo (zero dependencia)
- LGPD: capturar consentimento antes de salvar