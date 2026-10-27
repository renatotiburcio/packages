---
name: cost-aware-intelligence
description: Use when tracking, optimizing, or making cost-aware decisions about AI provider usage, token consumption, or infrastructure costs.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# cost-aware-intelligence

## Objetivo

Tomar decisões conscientes de custo ao usar IA e infraestrutura.

## Matriz de Decisão por Custo

| Custo/Tarefa | Modelo Recomendado | Provider |
|:------------:|-------------------|----------|
| $0 (local) | ollama/llama3 128k | Ollama |
| < $0.01 | openrouter/auto | OpenRouter |
| $0.01-0.05 | claude-sonnet-4 | Anthropic |
| $0.05-0.20 | gpt-4o | OpenAI |
| > $0.20 | claude-opus-4 | Anthropic |

## Regras de Economia

1. **Local primeiro**: Ollama para 80% das tarefas diárias
2. **Context mínimo**: Enviar apenas o necessário no prompt
3. **Cache**: Reusar respostas de queries similares
4. **Batch**: Agrupar tarefas pequenas
5. **Downgrade automático**: Se task < score 5, usar economy

## Tracking

### Por Tarefa
```json
{
  "task": "refactor-auth",
  "provider": "anthropic",
  "model": "claude-sonnet-4",
  "tokensIn": 12000,
  "tokensOut": 3000,
  "cost": 0.081
}
```

### Diário
- Total de tokens consumidos
- Custo por provider
- Custo por tipo de tarefa
- Economia vs usar sempre cloud

## Procedimento

1. Antes de cada tarefa → estimar custo
2. Se custo > threshold → sugerir alternativa
3. Registrar consumo
4. Reportar semanalmente
