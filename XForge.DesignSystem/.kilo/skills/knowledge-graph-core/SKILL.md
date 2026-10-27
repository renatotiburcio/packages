---
name: knowledge-graph-core
description: Use when building, querying, updating, or traversing the project knowledge graph.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# knowledge-graph-core

## Objetivo

Gerenciar o grafo de conhecimento do projeto.

## Nós do Grafo

| Tipo | Exemplo | Relacionamentos |
|------|---------|-----------------|
| **Decisão** | "Usar XForge.MediatR" | afeta, rejeita, substitui |
| **Componente** | "AuthService" | depende_de, usa, testa |
| **Regra** | "Golden Rule 1" | aplica_a, bloqueia |
| **Incidente** | "Auth timeout" | causou, afetou |
| **Conhecimento** | "EF Core patterns" | valida, documenta |

## Operações

### Criar Nó
```json
{
  "type": "decision",
  "title": "Usar AutoMapper",
  "content": "Não usar Mapster",
  "source": "golden-rules",
  "trust": 95
}
```

### Consultar
- Por tipo: "todas as decisões de segurança"
- Por relacionamento: "o que afeta AuthService?"
- Por confiança: "conhecimento com score > 80"

### Atualizar
- Incrementar confiança quando validado
- Decrementar quando contradito
- Marcar como deprecated quando obsoleto

## Armazenamento

- .xforge/knowledge/graph.json (nós + arestas)
- .xforge/knowledge/index.json (índice para busca)
