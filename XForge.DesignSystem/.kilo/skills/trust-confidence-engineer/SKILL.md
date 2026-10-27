---
name: trust-confidence-engineer
description: Expert em calcular e gerenciar scores de confiança para conhecimento, fontes e decisões.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# trust-confidence-engineer

## Objetivo

Implementar e gerenciar sistema de confiança para conhecimento.

## Fórmula

```
trust = (source * 0.3) + (recency * 0.2) + (corroboration * 0.3) + (criticality * 0.2)
```

### Componentes

| Componente | 0-30 | 31-60 | 61-80 | 81-100 |
|------------|------|-------|-------|--------|
| **Source** | AI-generated | Blog | Internal docs | Official docs |
| **Recency** | > 2 anos | 6-24 meses | 1-6 meses | < 30 dias |
| **Corroboration** | 1 fonte | 2 fontes | 3 fontes | 4+ fontes |
| **Criticality** | Info | Feature | Security | Fiscal/Legal |

### Faixas

| Score | Nível | Ação |
|:-----:|-------|------|
| 90-100 | Verified | Auto-promote |
| 70-89 | Trusted | Usar com registro |
| 50-69 | Provisional | Marcar incerto |
| 30-49 | Unverified | Cautela |
| 0-29 | Deprecated | Não usar |

## Decay

- -5 pontos/mês sem uso
- -10 pontos quando contradito
- Reset quando fonte atualizada

## Procedimento

1. Avaliar componente individual
2. Calcular score composto
3. Aplicar decay
4. Classificar nível
5. Ação correspondente
6. Registrar no knowledge graph
