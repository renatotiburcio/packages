---
id: T079
title: XF Emoji catalog com compressao GZip
priority: P0
status: pending
estimated_time: 12h
dependencies: T078
---

# T079: XF Emoji catalog com compressao GZip

## Objetivo
Criar catalog de emojis (LATAM-focused, pt-BR/es/en) com compressao GZip byte[] e 3 styles (Color/Flat/HighContrast) + 5 skintones para PeopleBody.

## Passos
1. Criar enum XFEmojiSize { Size16, Size32, Size48 }
2. Criar enum XFEmojiStyle { Color, Flat, HighContrast }
3. Criar enum XFEmojiSkintone { Default, Light, MediumLight, Medium, MediumDark, Dark }
4. Criar enum XFEmojiGroup { Activities, AnimalsNature, Flags, FoodDrink, Objects, PeopleBody, SmileysEmotion, Symbols, TravelPlaces }
5. Criar record XFEmojiInfo { Name, Group, Style, Skintone, Size, Keywords, Data (GZip bytes) }
6. Criar helper XFEmojiCompress { Zip/Unzip byte[] using GZipStream }
7. Curar 200+ emojis essenciais (FluentUI tem 7885 - muito grande)
8. Para cada emoji: gerar 3 styles × 3 sizes × 6 skintones (quando aplicavel)
9. Embed em XFEmojiCatalog static partial class
10. Adicionar emoji shortcode mapping (:rocket: → 🚀)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiCatalog.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiInfo.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiSize.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiStyle.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiSkintone.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiGroup.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiCompress.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiShortcodes.cs`
- Modificar: `src/XForge.DesignSystem/Components/Icon/Component-registry.g.cs` (registrar emoji picker)

## Criterios de Aceite
- [ ] 200+ emojis curados (top usages LATAM)
- [ ] Compressao GZip 70%+ vs SVG inline
- [ ] Lazy decode automatico (Content property)
- [ ] 3 styles (Color/Flat/HighContrast) por emoji
- [ ] 5 skintones para PeopleBody (Unicode CLDR)
- [ ] 500+ shortcodes (:rocket:, :fire:, :heart: ...)
- [ ] Lookup: XFEmojiCatalog.GetByShortcode(":rocket:") → XFEmojiInfo
- [ ] Build: 0 erros, 0 warnings
- [ ] Tests: 15+ passando, cobertura 100%

## Notas
- Baseado em FluentUI Emoji architecture (Microsoft.FluentUI.AspNetCore.Components.Emoji)
- SVG inline raw seria 5-20 KB por emoji; GZip reduz para 1-3 KB
- 200 emojis × 3 styles × 3 sizes = 1800 classes (manageable)
- Curadoria: priorizar smileys, food, activities, common symbols
