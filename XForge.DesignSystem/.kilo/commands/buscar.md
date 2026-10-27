---
description: Busca informacoes na web e no knowledge base local. Combina DuckDuckGo, Firecrawl e RAG.
agent: code
---

# /buscar

## Objetivo

Buscar informacoes em fontes externas (web) e internas (knowledge base). Combina resultados de web search com RAG local para fornecer respostas completas.

## Subcomandos

### /buscar web \<termo\>
Busca na web usando DuckDuckGo API.

**Exemplos:**
```
/buscar web .NET 8 minimal API best practices
/buscar web Blazor Server vs WebAssembly differences
/buscar web PostgreSQL partitioning strategies
```

**Comportamento:**
1. Faz request para DuckDuckGo Instant Answer API
2. Retorna resumo + links relevantes
3. Sugere buscas relacionadas

### /buscar scrape \<url\>
Faz scraping de uma URL usando Firecrawl.

**Exemplos:**
```
/buscar scrape https://docs.microsoft.com/ef/core/
/buscar scrape https://learn.blazor.com/
```

**Comportamento:**
1. Faz request para Firecrawl API
2. Extrai conteudo principal da pagina
3. Converte para markdown limpo
4. Opcionalmente indexa no RAG local

### /buscar local \<termo\>
Busca no knowledge base local (RAG).

**Exemplos:**
```
/buscar local padroes de arquitetura
/buscar local decisoes sobre banco de dados
/buscar local configuracao CI/CD
```

**Comportamento:**
1. Faz query no indice RAG local
2. Retorna documentos mais relevantes
3. Inclui score de relevancia

### /buscar \<termo\>
Busca combinada: web + local.

**Exemplos:**
```
/buscar Entity Framework migrations strategies
/buscar RabbitMQ dead letter queue configuration
```

**Comportamento:**
1. Busca no RAG local primeiro
2. Se resultados insuficientes, busca na web
3. Combina e deduplica resultados
4. Apresenta fontes (local vs web)

## APIs Utilizadas

| API | Env Var | Uso |
|-----|---------|-----|
| DuckDuckGo | (sem chave) | Web search |
| Firecrawl | `env:FIRECRAWL_API_KEY` | Page scraping |
| RAG Local | (nenhuma) | Knowledge base |

## Config

Definido em `~/.xforge/config.json`:
```json
{
  "search": {
    "defaultEngine": "combined",
    "maxWebResults": 5,
    "maxLocalResults": 10,
    "autoIndex": true
  }
}
```

## Validacoes

- Se FIRECRAWL_API_KEY nao configurada -> busca web sem scraping
- Se RAG nao indexado -> sugere rodar `/conhecimento` primeiro
- Se termo vazio -> erro com instrucoes de uso

## Exemplos

```
/buscar
/buscar com parametros especificos do projeto
```
