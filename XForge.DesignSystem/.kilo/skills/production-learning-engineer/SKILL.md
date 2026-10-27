---
name: production-learning-engineer
description: Expert em aprendizado contínuo de produção: análise de logs, métricas, incidentes e feedback.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# production-learning-engineer

## Objetivo

Extrair aprendizados de dados de produção para melhorar o sistema.

## Fontes de Aprendizado

| Fonte | Dado | Insights |
|-------|------|----------|
| Logs | Erros, warnings | Bugs recorrentes |
| Métricas | Latência, throughput | Gargalos |
| Incidentes | Root causes | Problemas estruturais |
| User feedback | Satisfação | UX gaps |
| APM | Traces | Bottlenecks |

## Procedimento

### 1. Coletar
- Logs de erro (últimos 7 dias)
- Métricas de performance
- Incidentes registrados
- Feedback de usuários

### 2. Analisar
- Classificar por frequência
- Identificar padrões
- Correlacionar com código
- Estimar impacto

### 3. Aprender
- Extrair regras preventivas
- Atualizar known errors
- Criar playbooks
- Atualizar memória

### 4. Aplicar
- Criar testes para bugs encontrados
- Adicionar monitoramento
- Atualizar alertas
- Melhorar documentação

## Métricas

| Métrica | Target |
|---------|--------|
| Bugs recorrentes | < 5/mês |
| Mean time to detect | < 5 min |
| Mean time to resolve | < 2h |
| Learning application rate | > 80% |

## Regras

- Aprender de cada incidente
- Documentar cada lição
- Aplicar lições a código/testes
- Medir efetividade do aprendizado
