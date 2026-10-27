---
name: search-engine
description: Use when searching across code, memory, documentation, knowledge graph, semantic index, lexical index, GitHub history, or multiple repositories.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# search-engine

## Objetivo

Buscar informação em múltiplas fontes de forma eficiente e abrangente.

## Fontes de Busca

| Fonte | Método | Quando Usar |
|-------|--------|-------------|
| Código | grep/glob no workspace | Encontrar funções, classes, padrões |
| Memória | .xforge/memory/ + BM25 | Decisões passadas, contexto |
| Knowledge | .xforge/knowledge/ + RAG | Conhecimento acumulado |
| Documentação | .kilo/docs-html/ | Manuais, guias |
| GitHub | gh api / webfetch | Histórico, issues, PRs |
| Web | webfetch | Documentação externa, APIs |

## Estratégia de Busca

### 1. Busca por Código
```
grep "pattern" --include="*.cs" --include="*.ts"
glob "**/*Service*.cs"
```

### 2. Busca por Memória
```
memory search "keyword" scope=project
memory search "keyword" scope=global
```

### 3. Busca por Conhecimento
```
python .kilo/automation/scripts/rag/query-local.ps1 "query"
```

### 4. Busca por Documentação
```
webfetch url="https://docs.example.com/api"
```

## Regras

- Buscar local primeiro, depois cloud
- Combinar resultados de múltiplas fontes
- Deduplicar por relevância
- Registrar busca no audit trail quando envolver dados sensíveis
