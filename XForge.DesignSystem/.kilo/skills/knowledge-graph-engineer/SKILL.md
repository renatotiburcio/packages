---
name: knowledge-graph-engineer
description: Expert em construir, consultar e manter o knowledge graph do projeto.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# knowledge-graph-engineer

## Objetivo

Construir e manter o grafo de conhecimento estruturado do projeto.

## Nós do Grafo

| Tipo | Exemplo | Arestas |
|------|---------|---------|
| **Decision** | "Usar Minimal API" | afeta, rejeita, substitui |
| **Component** | "AuthService" | depende_de, usa, testa |
| **Rule** | "Golden Rule 1" | aplica_a, bloqueia |
| **Incident** | "Auth timeout" | causou, afetou |
| **Knowledge** | "EF Core patterns" | valida, documenta |

## Operações

### Criar Nó
1. Definir tipo e título
2. Adicionar conteúdo e fonte
3. Calcular confiança inicial
4. Criar arestas para nós existentes
5. Atualizar índice

### Consultar
- Por tipo: "decisões de segurança"
- Por aresta: "o que afeta AuthService?"
- Por confiança: "score > 80"
- Por data: "últimos 30 dias"

### Atualizar
- Incrementar confiança quando validado
- Decrementar quando contradito
- Marcar deprecated quando obsoleto

## Armazenamento

- `.xforge/knowledge/graph.json` (nós + arestas)
- `.xforge/knowledge/index.json` (índice)

## Procedimento

1. Identificar nova informação
2. Verificar se nó similar existe
3. Criar ou atualizar nó
4. Criar/atualizar arestas
5. Reindexar
6. Verificar integridade

## Regras

- Sempre registrar fonte
- Nunca duplicar nós
- Manter arestas bidirecionais
- Reindexar após mudanças
