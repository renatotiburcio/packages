---
description: Verifica APIs, CORS, rate limiting, Swagger, erros e payloads.
agent: code
---

# /security-api-check

## Objetivo

Verifica APIs, CORS, rate limiting, Swagger, erros e payloads.

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
/security-api-check
/security-api-check verificar vulnerabilidades no modulo de autenticacao
```
