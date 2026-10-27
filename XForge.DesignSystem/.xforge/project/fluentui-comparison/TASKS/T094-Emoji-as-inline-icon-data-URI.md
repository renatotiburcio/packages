---
id: T094
title: Emoji as inline icon (data URI inline)
priority: P2
status: pending
estimated_time: 2h
dependencies: T079
---

# T094: Emoji as inline icon (data URI inline)

## Objetivo
Renderizar emojis como data URI inline para uso em qualquer contexto (tooltip, badge, etc). Reutilizar XFEmoji Content via data:image/svg+xml;base64.

## Passos
1. Criar `Components/Emoji/XFEmojiDataUri.cs` (helper)
2. Metodo: `string ToDataUri(XFEmojiInfo emoji, int size)` → base64 data URI
3. Integrar em XFFluentEmoji.razor: parametro RenderMode (SVG vs DataUri)
4. Usar em: badges, tooltips, status indicators
5. Tests: 8+ (data URI generation, size, integration)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiDataUri.cs`
- Modificar: `src/XForge.DesignSystem/Components/Emoji/XFFluentEmoji.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/Emoji/EmojiDataUriTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] Data URI generation (base64)
- [ ] RenderMode parametro em XFFluentEmoji
- [ ] Integration em badges/tooltips
- [ ] Tests: 8+ (generation, size, integration)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Inspirado em FluentUI Icon.ToDataUri
- Reusar XFEmojiCompress + Content
- Use case: status icon em notification badge
