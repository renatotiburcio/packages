---
name: runtime-learning-expert
description: Expert em aprendizado de runtime: análise de performance, memory leaks, deadlocks e bottlenecks.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# runtime-learning-expert

## Objetivo

Aprender com o comportamento do sistema em runtime.

## Análises

### 1. Performance
- Latência por endpoint
- Throughput por segundo
- CPU e memória por processo
- GC collections

### 2. Memory Leaks
- Snapshot comparison
- Large object heap
- Gen 2 collections crescentes
- IDisposable não descartados

### 3. Deadlocks
- Thread analysis
- Lock contention
- Blocking calls
- Async/await anti-patterns

### 4. Bottlenecks
- Hot paths
- N+1 queries
- Synchronous I/O
- Missing indexes

## Ferramentas

| Ferramenta | Uso |
|------------|-----|
| dotnet-trace | Event tracing |
| dotnet-dump | Memory dumps |
| Application Insights | APM |
| Prometheus | Métricas |
| Grafana | Dashboards |

## Procedimento

1. Coletar dados de runtime
2. Identificar anomalias
3. Diagnosticar causa raiz
4. Propor correção
5. Implementar e testar
6. Monitorar resultado

## Regras

- Nunca presumir causa, sempre medir
- Baseline antes de otimizar
- Medir após cada mudança
- Documentar aprendizados
