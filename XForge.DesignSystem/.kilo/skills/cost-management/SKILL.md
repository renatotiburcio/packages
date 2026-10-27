---
name: cost-management
description: Use when tracking, optimizing, or reporting AI provider costs, token usage, or budget consumption.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# cost-management

## Objetivo

Monitorar e otimizar custos de uso de IA.

## Modelo de Custo por Provider

| Provider | Modelo | Custo/1M tokens (in) | Custo/1M tokens (out) |
|----------|--------|:--------------------:|:---------------------:|
| Ollama | llama3 | $0 (local) | $0 (local) |
| OpenRouter | auto | Variável | Variável |
| Anthropic | claude-sonnet-4 | $3.00 | $15.00 |
| Anthropic | claude-opus-4 | $15.00 | $75.00 |
| OpenAI | gpt-4o | $2.50 | $10.00 |
| OpenAI | o1 | $15.00 | $60.00 |

## Oportunidades de Economia

1. **Local primeiro**: Ollama para tarefas simples (economia: 100%)
2. **Economy tier**: openrouter/auto para não-reasoning (economia: ~80%)
3. **Batch**: Agrupar tarefas pequenas em uma chamada
4. **Cache**: Reusar respostas de queries similares
5. **Context minimal**: Enviar apenas o necessário no prompt

## Procedimento

1. Registrar provider e modelo usado por tarefa
2. Estimar custo da tarefa
3. Comparar com tier alternativo
4. Se custo > threshold → sugerir downgrade
5. Salvar em .xforge/operations/costs/
6. Reportar semanalmente

## Regras

- LGPD/dados sensíveis → NUNCA usar cloud
- Tarefas simples → Ollama primeiro
- Premium apenas para complexidade > 10
- Log de custo em toda task completion
