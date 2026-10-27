---
id: T086
title: Emoji shortcode parser (:rocket: → 🚀)
priority: P1
status: pending
estimated_time: 4h
dependencies: T079
---

# T086: Emoji shortcode parser (:rocket: → 🚀)

## Objetivo
Implementar parser de shortcodes estilo GitHub: `:rocket:` → 🚀, `:fire:` → 🔥. Usado em AI chat (T060) e XFEmojiPicker (T085).

## Passos
1. Criar `Components/Emoji/XFEmojiShortcodeParser.cs`
2. Implementar Dictionary<string, string> shortcodeToName com 500+ entradas
3. Implementar Dictionary<string, string> nameToShortcode (reverse, 500+ entradas)
4. Metodo: `string Parse(string text)` → substitui todos `:shortcode:` por emojis
5. Metodo: `string Reverse(string text)` → substitui emojis por `:shortcode:`
6. Suportar aliases: :thumbsup: :+1: :thumbup: → 👍
7. Suportar skin tone modifiers: :wave::skin-tone-2: → 👋🏽
8. Case-insensitive: :ROCKET: == :rocket:
9. Word boundary: `:rocketlaunch:` NAO substitui (apenas match exato)
10. Performance: pre-compiled regex com 500+ patterns
11. Tests: 20+ (parse, reverse, aliases, skintone, case, boundary)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiShortcodeParser.cs`
- Criar: `src/XForge.DesignSystem/Components/Emoji/XFEmojiShortcodes.cs` (dictionary data)
- Criar: `tests/XForge.DesignSystem.Tests/Emoji/EmojiShortcodeParserTests.cs` (20+ tests)

## Criterios de Aceite
- [ ] 500+ shortcodes (top usages GitHub)
- [ ] Parse: `:rocket:` → 🚀
- [ ] Reverse: 🚀 → `:rocket:`
- [ ] Aliases: :+1: :thumbsup: → 👍
- [ ] Skintone: :wave::skin-tone-2: → 👋🏽
- [ ] Case-insensitive
- [ ] Word boundary (nao substitui em palavras maiores)
- [ ] Performance: < 1ms para 1KB de texto com 50 shortcodes
- [ ] Tests: 20+ passando
- [ ] Build: 0 erros, 0 warnings

## Notas
- Inspirado em GitHub emoji shortcodes
- Lista base: https://api.github.com/emojis (json)
- Reusar XFEmojiCatalog (T079) para lookup reverso
- Usado em T060 (AIChatService) e T077 (HxSmart AI)
