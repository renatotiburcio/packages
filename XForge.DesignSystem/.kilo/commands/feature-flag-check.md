---
description: XForge workflow: feature flag check
agent: code
---

# feature-flag-check

## Objetivo

Validar se um recurso experimental, workflow beta, agente novo, skill nova ou provider alternativo pode ser usado.

## Etapas

1. Identificar feature flag solicitada.
2. Verificar status: disabled, experimental, beta, approved ou deprecated.
3. Verificar escopo: global, projeto, cliente, módulo ou usuário.
4. Verificar RBAC.
5. Verificar riscos.
6. Permitir, bloquear ou exigir aprovação humana.

## Saída obrigatória

- decisão;
- motivo;
- riscos;
- quem pode aprovar;
- audit trail.

## Exemplos

```
/feature-flag-check
/feature-flag-check com parametros especificos do projeto
```
