---
description: Verifica NuGet/npm, vulnerabilidades, licenças e supply chain.
agent: code
---

# /security-dependencies-check

## Objetivo

Verifica NuGet/npm, vulnerabilidades, licenças e supply chain.

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
/security-dependencies-check
/security-dependencies-check verificar vulnerabilidades no modulo de autenticacao
```
