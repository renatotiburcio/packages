---
description: Verifica autorização, roles, claims, policies, tenants e endpoints sensíveis.
agent: code
---

# /security-authorization-check

## Objetivo

Verifica autorização, roles, claims, policies, tenants e endpoints sensíveis.

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
/security-authorization-check
/security-authorization-check verificar vulnerabilidades no modulo de autenticacao
```
