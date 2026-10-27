---
name: benchmarks
description: Use when comparing prompts, models, providers, workflows, cost, latency, accuracy, or quality for a task.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# benchmarks

## Objetivo

Comparar opções de forma objetiva e reproduzível.

## Tipos de Benchmark

| Tipo | Métrica Principal | Ferramenta |
|------|-------------------|------------|
| **Modelo** | Qualidade da resposta | Avaliação humana + rubric |
| **Provider** | Latência + custo | Cron + billing |
| **Workflow** | Tempo total + steps | Cron + logs |
| **Código** | Performance | BenchmarkDotNet |
| **RAG** | Recall + precision | Queries de teste |

## Procedimento

### 1. Definir Critérios
- O que está sendo comparado?
- Quais métricas importam?
- Qual o baseline?

### 2. Executar
- Rodar cada opção com mesmas entradas
- Registrar: tempo, custo, qualidade, erros
- Mínimo 3 rodadas por opção

### 3. Analisar
- Calcular médias e desvios
- Identificar trade-offs
- Ranking por métrica

### 4. Reportar

```json
{
  "benchmark": "model-comparison",
  "criteria": ["quality", "cost", "latency"],
  "results": [
    {"option": "claude-sonnet", "quality": 9, "cost": 0.03, "latency": 2.1},
    {"option": "gpt-4o", "quality": 8, "cost": 0.02, "latency": 1.8}
  ],
  "recommendation": "claude-sonnet for quality, gpt-4o for cost"
}
```
