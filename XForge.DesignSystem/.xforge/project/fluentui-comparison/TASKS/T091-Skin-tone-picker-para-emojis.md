---
id: T091
title: Skin tone picker para emojis de PeopleBody
priority: P2
status: pending
estimated_time: 4h
dependencies: T082
---

# T091: Skin tone picker para emojis de PeopleBody

## Objetivo
UI dedicada para selecionar emoji skin tone (Default/Light/MediumLight/Medium/MediumDark/Dark). Usado em emoji picker e chat AI.

## Passos
1. Criar `Components/Emoji/XFSkintonePicker.razor`
2. Grid 6 swatches (cores de pele Fitzpatrick scale)
3. Live preview: emoji de pessoa (👋) muda de skin tone
4. Parametros: Skintone (XFEmojiSkintone), OnSkintoneChanged
5. Two-way binding: @bind-Skintone
6. Suportar emojis que nao tem skin tone (deve ficar disabled)
7. Tests: 10+ (binding, swatch, disabled state, preview)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFSkintonePicker.razor`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFSkintonePicker.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Emoji/SkintonePickerTests.cs` (10+ tests)
- Criar: `examples/Demo/Pages/SkintonePickerDemo.razor` (showcase)

## Criterios de Aceite
- [ ] 6 swatches (Default + 5 tones)
- [ ] Live preview funciona
- [ ] Two-way binding funciona
- [ ] Disabled state para emojis nao-humanos
- [ ] Tests: 10+ passando
- [ ] Build: 0 erros, 0 warnings

## Notas
- Complementa T082 (emoji component) com UX
- Skintones seguem Unicode CLDR (Fitzpatrick scale)
- Acessibilidade: representatividade importa
