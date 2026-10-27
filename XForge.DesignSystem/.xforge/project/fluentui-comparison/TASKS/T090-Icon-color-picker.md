---
id: T090
title: Icon color picker (accent/lightweight/custom)
priority: P2
status: pending
estimated_time: 4h
dependencies: T081
---

# T090: Icon color picker (accent/lightweight/custom)

## Objetivo
UI dedicada para selecionar Icon color (Accent/Lightweight/Success/Warning/Error/Important/Custom). Usado em design tools e demos.

## Passos
1. Criar `Components/Icon/XFIconColorPicker.razor`
2. Grid 6 cores (Accent/Lightweight/Success/Warning/Error/Important) + botao Custom
3. Swatch visual (16x16px icone com cor aplicada)
4. Custom color: color picker (input type=color) + hex input
5. Live preview: icone exemplo muda de cor ao selecionar
6. Theme-aware: cores ajustam para light/dark
7. Parametros: Color (XFIconColor), CustomColor, OnColorChanged
8. Two-way binding: @bind-Color, @bind-CustomColor
9. Tests: 12+ (cores, custom, theme, events, binding)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconColorPicker.razor`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconColorPicker.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconColorPickerTests.cs` (12+ tests)
- Criar: `examples/Demo/Pages/IconColorPickerDemo.razor` (showcase)

## Criterios de Aceite
- [ ] 6 swatches pre-definidos + Custom
- [ ] Live preview funciona
- [ ] Theme-aware (light/dark)
- [ ] Custom color via color picker + hex
- [ ] Two-way binding funciona
- [ ] Tests: 12+ passando
- [ ] Build: 0 erros, 0 warnings

## Notas
- Complementa T083 (color tokens) com UX
- Custom color usa input type=color (native browser picker)
- Theme integration: usa design tokens de T001
