---
description: Ingerir, auditar e pesquisar knowledge packs e RAG index.
agent: code
---

# /conhecimento

## Categoria

gestão

## Objetivo

Gerenciar o conhecimento acumulado do projeto: ingestão, busca, auditoria.

## Sub-comandos

| Sub-comando | Ação |
|-------------|------|
| `/conhecimento ingerir <fonte>` | Adicionar nova fonte |
| `/conhecimento buscar <query>` | Buscar no knowledge base |
| `/conhecimento auditar` | Verificar qualidade do index |
| `/conhecimento reindexar` | Reconstruir índice RAG |
| `/conhecimento status` | Status do knowledge graph |

## Procedimento

### Ingerir
1. Identificar fonte (URL, arquivo, conversa)
2. Chunkar em pedaços de 40 linhas
3. Classificar por domínio
4. Avaliar confiança
5. Indexar no RAG
6. Atualizar knowledge graph

### Buscar
1. Usar RAG query local
2. Combinar com BM25
3. Filtrar por domínio e confiança
4. Retornar top-5 com fonte

### Auditar
1. Verificar integridade do índice
2. Identificar chunks órfãos
3. Verificar confiança mínima
4. Remover duplicatas

### Reindexar
```powershell
python .kilo/automation/scripts/rag/rag_local.py index
```

## Fontes Suportadas

| Fonte | Processo |
|-------|----------|
| .md files | Parse markdown |
| .pdf files | Extract text |
| Web pages | webfetch + parse |
| Code files | AST parse |
| Conversas | Lesson extraction |

## Regras

- Sempre registrar fonte original
- Nunca ingerir sem classificar confiança
- Dados sensíveis → pseudonimizar
- Reindexar após ingestão

## Exemplos

```
/conhecimento
/conhecimento ingerir documentação da API externa
/conhecimento buscar padrões de CQRS
/conhecimento auditar qualidade do index
/conhecimento reindexar todo o projeto
```
