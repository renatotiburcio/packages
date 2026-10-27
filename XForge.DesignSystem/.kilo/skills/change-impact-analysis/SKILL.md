---
name: change-impact-analysis
description: Use when analyzing the impact of a proposed change across the codebase, architecture, or team.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# change-impact-analysis

## Objetivo

Mapear todos os impactos de uma mudança antes de executá-la.

## Dimensões de Impacto

| Dimensão | O que verificar |
|----------|-----------------|
| **Código** | Arquivos afetados, dependências, imports |
| **API** | Contratos públicos, breaking changes |
| **Banco** | Schema changes, migrations, dados |
| **Testes** | Testes que quebram, novos testes necessários |
| **Docs** | README, API docs, CHANGELOG |
| **Time** | Quem precisa saber, review necessário |
| **Deploy** | Pipeline, feature flags, rollback |
| **Performance** | Latência, memória, CPU |

## Procedimento

1. Identificar o escopo da mudança
2. Usar grep/glob para mapear referências
3. Verificar contratos de API (OpenAPI)
4. Verificar migrations pendentes
5. Listar testes afetados
6. Estimar esforço total
7. Classificar risco: baixo/médio/alto/crítico
8. Sugerir plano de execução

## Saída

```json
{
  "change": "descrição",
  "scope": {
    "filesAffected": 12,
    "apiBreaking": false,
    "dbMigration": true,
    "testsBroken": 5,
    "docsUpdate": true
  },
  "riskLevel": "medium",
  "estimatedHours": 6,
  "recommendation": "branch-and-test"
}
```
