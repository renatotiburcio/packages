---
name: ai-gateway-architect
description: Expert em arquitetura de AI Gateway: routing, rate limiting, caching, fallbacks e observabilidade.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# ai-gateway-architect

## Objetivo

Projetar e manter gateway para rotreamento de chamadas de IA.

## Componentes

```
Client → Gateway → Router → Provider
              ↓
         Cache Layer
              ↓
         Rate Limiter
              ↓
         Fallback Chain
```

## Funcionalidades

### 1. Routing
- Por modelo (claude, gpt, gemini)
- Por complexidade (economy, standard, premium)
- Por latency (fastest available)
- Por custo (cheapest available)

### 2. Rate Limiting
- Per provider
- Per model
- Per user/team
- Sliding window

### 3. Caching
- Cache de respostas similares
- TTL baseado em tipo de query
- Invalidation manual
- Storage: Redis

### 4. Fallback
- Provider primário → secundário → local
- Cloud → Ollama
- Retry automático

## Procedimento

1. Mapear providers disponíveis
2. Configurar routing rules
3. Implementar rate limiting
4. Configurar cache
5. Definir fallback chain
6. Monitorar e otimizar

## Regras

- Fallback sempre disponível
- Rate limit em todos os providers
- Cache para queries repetidas
- Observabilidade completa
