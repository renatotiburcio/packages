# ADR-0002: RAG Lexical Local como Mecanismo Primário de Conhecimento

## Status
accepted

## Data
2026-06-08

## Contexto
O XForge precisa de um mecanismo de recuperação de conhecimento que funcione localmente, sem dependências externas, e que seja eficiente para buscar em documentação, comandos, regras e conhecimento de domínio.

## Decisão
Usar **RAG lexical local** (TF-IDF em Python puro) como mecanismo primário. Embeddings semânticos são opcionais para o futuro.

## Alternativas Consideradas

### Alternativa 1: Embeddings semânticos (OpenAI, Azure OpenAI)
**Prós:** Busca semântica poderosa, entende intenção
**Contras:** Requer API key, custo, dependência externa, não funciona offline
**Por que não foi escolhida:** Viola o princípio "funciona sem server externo"

### Alternativa 2: Busca full-text do banco de dados
**Prós:** Integrado com dados existentes
**Contras:** Limitado a dados estruturados, não busca em documentação
**Por que não foi escolhida:** Não cobre arquivos markdown, comandos, skills

### Alternativa 3: RAG híbrido (lexical + embeddings)
**Prós:** Melhor dos dois mundos
**Convers:** Complexidade adicional, embeddings ainda opcionais
**Por que não foi escolhida agora:** Pode ser adicionado no futuro. Começar simples.

## Consequências

### Positivas
- Zero dependências externas
- Funciona offline
- 788 documentos e 1.590 chunks indexados
- Source-types com pesos diferenciados
- Secret scanning automático
- Rápido para indexar e consultar

### Negativas
- Busca semântica limitada (não entende sinônimos)
- Qualidade depende da estrutura dos arquivos
- Não escala para milhões de documentos (suficiente para o caso de uso)

## Implementação
- Motor: `rag_local.py` (368 linhas, Python puro)
- Configuração: `.xforge/rag/config.json`
- Scripts: `index-local.ps1`, `query-local.ps1`, `status-index.ps1`, `clear-index.ps1`
- Próximo passo: embeddings opcionais quando necessário

## Fontes
- [TF-IDF](https://en.wikipedia.org/wiki/Tf%E2%80%93idf)
- [RAG Pattern](https://learn.microsoft.com/en-us/azure/architecture/architecture-styles/rag)
