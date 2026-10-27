---
name: memory-intelligence
description: Use when analyzing memory patterns, learning from past decisions, or optimizing memory retrieval.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# memory-intelligence

## Objetivo

Aprender com padrões de memória para melhorar decisões futuras.

## Análises

### Padrões de Sucesso
- Quais decisões levaram a bons resultados?
- Quais abordagens funcionam melhor?
- Quais erros se repetem?

### Padrões de Custo
- Quais tarefas consomem mais tokens?
- Onde a poda de contexto ajuda?
- Quais modelos performam melhor por tipo?

### Padrões de Qualidade
- Quais skills são mais usadas?
- Quais commands geram melhores resultados?
- Onde human review é sempre necessário?

## Métricas

| Métrica | Fórmula | Target |
|---------|---------|:------:|
| Recall | hits / queries | > 80% |
| Precision | relevant / returned | > 70% |
| Latency | time to retrieve | < 500ms |
| Freshness | avg age of used memory | < 30 dias |

## Procedimento

1. Coletar logs de uso de memória
2. Analisar padrões de query
3. Identificar gaps (queries sem resultado)
4. Identificar ruído (resultados irrelevantes)
5. Sugerir melhorias em indexação
6. Atualizar estratégias de sharding
