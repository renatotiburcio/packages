---
name: context-window-strategy
description: Use when the task needs context compression, token budgeting, hierarchical retrieval, summarization, or selection of relevant memory. Activate when context is above 50% capacity.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# context-window-strategy

## Objetivo

Gerenciar janela de contexto para evitar estouro de tokens.

## Capacidades por Modelo

| Tier | Contexto | Condense At | Estratégia |
|------|----------|-------------|------------|
| Local (Ollama) | 128k | 100k | summarize |
| Cloud Standard | 128k-200k | 100k-150k | summarize |
| Cloud Premium | 200k+ | 150k+ | summarize |

## Quando Ativar

- Contexto acima de 50% da capacidade
- Tarefa requer leitura de múltiplos arquivos grandes
- Conversa longa com muitas trocas

## Procedimento de Compressão

### 1. Leitura Inteligente
- Use grep/glob para encontrar linhas relevantes ANTES de ler o arquivo inteiro
- Leia apenas offset+limit (ex: linhas 10-50) quando souber a região exata
- Não leia arquivos que você já conhece
- Para 128k: pode ler arquivos maiores, mas prefira seletividade

### 2. Escrita Incremental
- Faça uma modificação por vez
- Use edit_file em vez de write_file para arquivos existentes
- Não tente escrever múltiplos arquivos em uma única resposta

### 3. Sumarização
- Se o contexto encher, resuma o que você sabe e prossiga
- Salve o estado atual em .xforge/memory/sessions/ se necessário
- Diga "Contexto limite atingido. Salvando progresso."

### 4. Por Tier de Modelo

#### 128k (Local Ollama / Cloud Standard)
- Pode carregar arquivos inteiros quando necessário
- Use indexes primeiro (.xforge/memory/index.json)
- Prefer edit_file para existentes
- 2-3 arquivos por resposta é seguro

#### 64k (Modelos menores)
- Leia APENAS linhas necessárias (use offset/limit)
- Uma modificação por resposta
- Use edit_file (mais econômico que write_file)
- Nunca carregue arquivos inteiros
