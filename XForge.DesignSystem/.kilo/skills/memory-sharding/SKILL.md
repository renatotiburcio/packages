---
name: memory-sharding
description: Use when organizing memory into shards for efficient retrieval, context window optimization, or multi-project management.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# memory-sharding

## Objetivo

Organizar memória em shards para busca eficiente e contexto otimizado.

## Estratégia de Sharding

| Shard | Conteúdo | Tamanho Alvo |
|-------|----------|:------------:|
| **project-dna** | Identidade do projeto | < 2KB |
| **architecture** | Decisões de arquitetura | < 5KB |
| **security** | Regras e decisões de segurança | < 3KB |
| **domain** | Conhecimento de domínio | < 5KB |
| **incidents** | Histórico de incidentes | < 3KB |
| **decisions** | ADRs e decisões | < 5KB |

## Procedimento

1. Classificar informação por domínio
2. Atribuir ao shard correto
3. Manter tamanho do shard < limite
4. Criar cross-references entre shards
5. Indexar cada shard separadamente
6. No query: identificar shards relevantes, não carregar todos

## Regras

- Nunca carregar todos os shards de uma vez
- Carregar project-dna sempre (contexto base)
- shards de segurança → sempre incluir em tasks de código
- shards de domínio → incluir quando task é de domínio
