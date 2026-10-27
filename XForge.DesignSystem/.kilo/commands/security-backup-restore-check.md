---
description: Verifica backup, restore real, RPO/RTO e continuidade.
agent: code
---

# /security-backup-restore-check

## Objetivo

Verifica backup, restore real, RPO/RTO e continuidade.

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
/security-backup-restore-check
/security-backup-restore-check verificar vulnerabilidades no modulo de autenticacao
```
