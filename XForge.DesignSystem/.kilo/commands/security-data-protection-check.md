---
description: Verifica proteção de dados, LGPD, criptografia, logs seguros e exposição.
agent: code
---

# /security-data-protection-check

## Objetivo

Verifica proteção de dados, LGPD, criptografia, logs seguros e exposição.

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
/security-data-protection-check
/security-data-protection-check verificar vulnerabilidades no modulo de autenticacao
```
