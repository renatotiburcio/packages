---
id: T149
title: XFColorPalette
priority: P1
status: pending
estimated_time: 12h
dependencies: T001
---

# T149: XFColorPalette

## Objetivo
Color picker com grid 5x8, alpha channel, eyedropper e color groups.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFColorPalette oferece grid 5x8 cores basicas, custom colors, alpha channel slider, RGB/HSL/HEX inputs, eyedropper (API EyeDropper), recent colors e grupos pre-definidos (Material/FluentUI/Tailwind).

## Passos
1. Estudar API DevExpress ColorEdit no docs.devexpress.com
2. Definir modelo XForge (XFColorPalette, ColorValue, ColorGroup)
3. Implementar conversoes RGB/HSL/HEX
4. Renderizar 3 grupos de cores (Material/FluentUI/Tailwind)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T149/XFColorPalette.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T149/ColorValue.cs
- Criar: src/XForge.DesignSystem/Components/T149/ColorGroup.cs
- Criar: src/XForge.DesignSystem/Components/T149/ColorConverter.cs
- Criar: src/XForge.DesignSystem/Components/T149/Groups/MaterialColors.cs
- Criar: src/XForge.DesignSystem/Components/T149/Groups/FluentUIColors.cs
- Criar: src/XForge.DesignSystem/Components/T149/Groups/TailwindColors.cs
- Criar: src/XForge.DesignSystem.Tests/T149ColorPaletteTests.cs
- Criar: docs/components/t149-color-palette/index.md

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
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap