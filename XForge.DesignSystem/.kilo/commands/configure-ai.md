---
description: Configura providers de IA como OpenRouter, Gemini, Ollama, OpenAI-compatible e XForge Engineer Server.
agent: code
---

# /configurar-ia

## Objetivo

Configurar a camada de IA do Engineer.

## Deve perguntar

1. Você usará local/Ollama?
2. Usará OpenRouter?
3. Usará Gemini?
4. Usará OpenAI-compatible?
5. Usará server interno?
6. Há dados sensíveis?
7. Qual modo padrão?
   - local
   - server
   - hybrid
   - cloud
   - offline

## Saídas

- `.env` exemplo;
- policy de roteamento;
- provider recomendado;
- riscos;
- checklist de segurança.

## Exemplos

```
/configurar-ia
/configurar-ia ambiente de desenvolvimento
```
