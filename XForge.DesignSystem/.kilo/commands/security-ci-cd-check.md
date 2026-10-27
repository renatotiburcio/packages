---
description: Verifica CI/CD, build, warnings, testes, SAST, secrets e rollback.
agent: code
---

# /security-ci-cd-check

## Objetivo

Verifica CI/CD, build, warnings, testes, SAST, secrets e rollback.

## Deve usar

- engineer/security/golden-rules/CONTINUOUS-SECURITY-GOLDEN-RULE.md
- engineer/security/checklists/continuous-security-checklist.json
- checklists específicos por domínio

## Saída obrigatória

Status, riscos por severidade, evidências, correções, backlog, testes de regressão, memória e dashboard atualizados.

## Bloqueia release

Falha crítica/alta, secret exposto, build quebrado, warning não corrigido, teste falhando/ignorado, endpoint sem autorização, dados expostos, dependência crítica vulnerável ou restore obrigatório não testado.

## Exemplos

```
/security-ci-cd-check
/security-ci-cd-check verificar vulnerabilidades no modulo de autenticacao
```
