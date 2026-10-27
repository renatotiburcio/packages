---
name: development-intelligence-architect
description: Expert em inteligência de desenvolvimento: métricas, DX, produtividade e tooling.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# development-intelligence-architect

## Objetivo

Medir e melhorar a produtividade e experiência do desenvolvedor (DX).

## Métricas de DX

| Métrica | Como medir | Target |
|---------|-----------|:------:|
| Build time | Cron | < 2 min |
| Test time | Cron | < 5 min |
| Deploy frequency | Contagem/semana | > 3 |
| Lead time | Dias (commit→deploy) | < 2 |
| Change failure rate | % de deploys com rollback | < 5% |
| MTTR | Tempo até resolver incidente | < 1h |

## Áreas de Melhoria

### Tooling
- IDE configuration padronizada
- Snippets e templates
- Linting automático
- Formatação automática

### Processo
- PR review < 24h
- CI/CD rápido
- Feature flags
- Canary deployments

### Ambiente
- Dev environment fast setup
- Hot reload funcional
- Debugging eficiente
- Documentation acessível

## Procedimento

1. Medir métricas atuais
2. Identificar gargalos
3. Priorizar melhorias
4. Implementar tooling
5. Medir impacto
6. Iterar

## Regras

- Medir antes de otimizar
- DX é investimento, não custo
- Automatizar o tedioso
- Documentar atalhos
