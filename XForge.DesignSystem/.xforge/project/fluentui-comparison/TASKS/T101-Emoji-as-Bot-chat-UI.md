---
id: T101
title: Emoji as Bot (chat UI) variant
priority: P3
status: pending
estimated_time: 8h
dependencies: T082, T060
---

# T101: Emoji as Bot (chat UI) variant

## Objetivo
Variante emoji do AIChatService (T060 Radzen) e HxSmartPasteButton (T077 Havit). Emoji shortcode parsing em chat AI messages.

## Passos
1. Criar `Components/AI/XFAiChatWithEmoji.razor` (extension de XFChatMessage)
2. Hook no AIChatService: parsear emojis em mensagens (T086 shortcode parser)
3. Renderizar emojis em mensagens AI/user com XFFluentEmoji (T082)
4. Suportar emoji picker inline (botao na input)
5. Suportar skin tone picker (T091) para emojis humanos
6. Testar com AI providers: OpenAI, Ollama, Gemini (verificar emoji rendering)
7. Tests: 10+ (parsing, rendering, picker integration, providers)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/AI/XFAiChatWithEmoji.razor`
- Criar: `src/XForge.DesignSystem/Components/AI/XFAiChatWithEmoji.razor.cs`
- Modificar: `src/XForge.DesignSystem.AI/XFAiChatService.cs` (integrate emoji parsing)
- Modificar: `src/XForge.DesignSystem/Components/AI/XFChatMessage.razor` (emoji rendering)
- Criar: `tests/XForge.DesignSystem.Tests/AI/AiChatEmojiTests.cs` (10+ tests)

## Criterios de Aceite
- [ ] AI chat messages parseia emojis via shortcode
- [ ] XFFluentEmoji renderiza emojis em mensagens
- [ ] Emoji picker inline funciona
- [ ] Skin tone picker funciona
- [ ] Testa com 3 AI providers (OpenAI, Ollama, Gemini)
- [ ] Tests: 10+ (parsing, rendering, picker, providers)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Integra T060 (Radzen AIChatService) + T077 (Havit HxSmart) + T086 (shortcode)
- Emoji em chat AI adiciona expressividade (Hugging Face, Discord)
- UX similar a Slack, Teams, GitHub
