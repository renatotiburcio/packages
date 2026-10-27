---
description: Configura um provider de IA com provedor, tipo, base URL, API key env, modelo padrão, modelo de código, modelo de raciocínio e fallback.
agent: code
---

# /configurar-provedor-ia

## Objetivo

Configurar provider de IA de forma completa.

## Campos obrigatórios

- providerId;
- displayName;
- providerType;
- baseUrl;
- apiKeyEnv;
- defaultModel;
- codingModel;
- reasoningModel;
- embeddingModel;
- privacyLevel;
- costTier;
- enabled.

## Providers suportados

- OpenRouter
- Gemini
- OpenAI-compatible genérico
- Ollama/local
- XForge Engineer Server
- generic-http

## Saídas

- `ai-providers.json`;
- `.env.example`;
- policy de roteamento;
- checklist de segurança;
- configuração para Kilo/Engineer.

## Exemplos

```
/configurar-provedor-ia
/configurar-provedor-ia ambiente de desenvolvimento
```
