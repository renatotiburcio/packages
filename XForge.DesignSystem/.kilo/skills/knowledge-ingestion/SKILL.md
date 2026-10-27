---
name: knowledge-ingestion
description: Use when ingesting new knowledge from documents, web pages, code reviews, incidents, or external sources into the knowledge graph.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# knowledge-ingestion

## Objetivo

Ingerir e indexar novo conhecimento de forma confiável.

## Fontes de Conhecimento

| Fonte | Processo | Destino |
|-------|----------|---------|
| Documentos (.md, .pdf) | Extração + chunking | .xforge/knowledge/ |
| Web pages | webfetch + parse | .xforge/knowledge/ |
| Code reviews | Extração de decisões | .xforge/decisions/ |
| Incidentes | Post-mortem | .xforge/operations/ |
| Conversas | Lesson extraction | .xforge/memory/ |

## Pipeline de Ingestão

1. **Capturar**: Fonte bruta (arquivo, URL, conversa)
2. **Parsear**: Extrair texto e estrutura
3. **Chunkar**: Dividir em pedaços de 40 linhas
4. **Indexar**: Adicionar ao RAG index
5. **Classificar**: Origem, confiança, domínio
6. **Validar**: Verificar qualidade do chunk
7. **Armazenar**: Salvar em .xforge/knowledge/

## Regras

- Sempre registrar fonte original
- Nunca ingerir sem classificar confiança
- Dados sensíveis → pseudonimizar antes de ingerir
- Reindexar após ingestão
- Manter log de ingestão
