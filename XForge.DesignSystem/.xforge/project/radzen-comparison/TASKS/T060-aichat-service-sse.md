---
id: T060
title: AIChatService com SSE streaming + session memory
priority: P0
status: pending
estimated_time: 16h
dependencies: nenhuma
---

# T060: AIChatService com SSE streaming + session memory

## Objetivo
Implementar `XFAIChatService` provider-agnostic com SSE streaming, session memory e primary constructor C# 12, alinhado com 11 AI providers existentes.

## Passos
1. Criar `IAIChatService` interface com `IAsyncEnumerable<string> GetCompletionsAsync(...)`
2. Criar `ConversationSession` class (Id, MaxMessages, Messages list)
3. Criar `AIChatServiceOptions` (Model, Endpoint, ApiKey, SystemPrompt, MaxMessages, Temperature, MaxTokens)
4. Implementar `AIChatService` com primary ctor `(IServiceProvider, IOptions<AIChatServiceOptions>)`
5. Implementar session dict com `lock` thread-safe
6. Implementar `GetCompletionsAsync` com SSE parsing (`data: ` lines, `[DONE]`)
7. Adicionar suporte a `proxy/endpoint/apiKey/apiKeyHeader` parameters
8. Adicionar suporte a `model/systemPrompt/temperature/maxTokens` overrides
9. Integrar com `XFAIChat.razor` (novo componente)
10. Criar testes com WireMock mockando SSE response

## Arquivos
- Criar: `src/XForge.DesignSystem/Services/AI/IAIChatService.cs`
- Criar: `src/XForge.DesignSystem/Services/AI/AIChatService.cs`
- Criar: `src/XForge.DesignSystem/Services/AI/AIChatServiceOptions.cs`
- Criar: `src/XForge.DesignSystem/Services/AI/ConversationSession.cs`
- Criar: `src/XForge.DesignSystem/Components/AI/Chat/XFAIChat.razor` + `.razor.cs`
- Criar: `tests/XForge.DesignSystem.Tests/AI/AIChatServiceTests.cs`

## Critérios de Aceite
- [ ] SSE parsing funciona (data: [DONE] stop)
- [ ] Session memory persiste em dict thread-safe
- [ ] Funciona com 11 providers (OpenAI, Ollama, Gemini, Anthropic, etc)
- [ ] Bearer e custom header auth suportados
- [ ] CancellationToken propaga via `[EnumeratorCancellation]`
- [ ] Build 0 warnings
- [ ] Testes cobrem session create/clear/memory

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/AIChatService.cs:231`
- Pattern: `IAsyncEnumerable<string>` + `EnumeratorCancellation` + primary ctor
- **Complementa** 11 AI providers existentes (XForge unique)
- In-memory sessions; multi-server precisa Redis (v2.1)