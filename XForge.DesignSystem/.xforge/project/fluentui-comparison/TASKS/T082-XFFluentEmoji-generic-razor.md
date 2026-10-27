---
id: T082
title: XFFluentEmoji.razor generic (3 styles + 6 skintones)
priority: P1
status: pending
estimated_time: 6h
dependencies: T079
---

# T082: XFFluentEmoji.razor generic (3 styles + 6 skintones)

## Objetivo
Criar componente Blazor generico `<XFFluentEmoji Value="..." />` inspirado em FluentEmoji<Emoji>. Suporta 3 styles (Color/Flat/HighContrast) e 5 skintones para PeopleBody.

## Passos
1. Criar `Components/Emoji/XFFluentEmoji.razor` (markup)
2. Criar `Components/Emoji/XFFluentEmoji.razor.cs` (codigo)
3. Parametros: Value (TEmoji), Size, Skintone, Style, OnClick, Title
4. Validar: Skintone so funciona para emojis de PeopleBody
5. Renderizar SVG inline com viewBox correto
6. Compressao GZip transparent (EmojiCompress helper)
7. Aplicar XFCssBuilder
8. Suportar aria-label
9. Documentar em XML comments

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFFluentEmoji.razor`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFFluentEmoji.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFFluentEmoji.razor.types.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Emoji/FluentEmojiTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] Renderiza SVG inline (descomprimido lazy)
- [ ] 3 styles: Color/Flat/HighContrast
- [ ] 5 skintones para PeopleBody (Default + 5 tons)
- [ ] 3 sizes: 16/32/48
- [ ] OnClick + Title (tooltip)
- [ ] aria-label automatico (Name)
- [ ] CSS variables para color tokens
- [ ] Tests: 8+ (render, style, skintone, size, lazy decode)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Baseado em FluentUI.Emoji architecture
- Reusar XFEmojiCompress (T079)
- Lazy decode automatico via Content property
