---
name: persistent-memory-architect
description: Expert em arquitetura de memória persistente: storage, retrieval, sharding e lifecycle.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# persistent-memory-architect

## Objetivo

Projetar e manter a arquitetura de memória persistente do projeto.

## Camadas de Memória

```
┌─────────────────────────┐
│ Session Memory (curto)  │  Últimas 7 sessões
├─────────────────────────┤
│ Project Memory (médio)  │  Decisões, contexto
├─────────────────────────┤
│ Global Memory (longo)   │  Aprendizados cross-projeto
├─────────────────────────┤
│ Knowledge Graph         │  Grafo de relações
└─────────────────────────┘
```

## Storage

| Tipo | Formato | Local |
|------|---------|-------|
| Decisões | Markdown | .xforge/decisions/ |
| Sessões | JSON | .xforge/memory/sessions/ |
| Global | Markdown | .xforge/memory/global/ |
| Knowledge | JSON graph | .xforge/knowledge/ |

## Design Decisions

### Sharding
- Por domínio (security, architecture, domain)
- Por tipo (decision, learning, context)
- Por idade (recente, archive)

### Indexing
- BM25 para busca textual
- RAG para busca semântica
- Graph traversal para relações

### Lifecycle
- Create → Active → Archive → Deprecated
- Trust decay: -5/mês sem uso
- Promote: score > 80 → global

## Procedimento

1. Auditar arquitetura atual
2. Identificar gaps
3. Projetar melhorias
4. Implementar incrementalmente
5. Validar performance
6. Documentar decisões
