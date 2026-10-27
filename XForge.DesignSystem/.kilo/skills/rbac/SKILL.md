---
name: rbac
description: Use when validating who can alter global memory, promote knowledge, approve workflows, change governance, run dangerous commands, or execute critical actions.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# rbac

## Objetivo

Validar permissões antes de executar ações de risco.

## Matriz de Permissões

| Ação | Nível Mínimo | Aprovação |
|------|:------------:|-----------|
| Ler código | Qualquer | Não |
| Editar código | Developer | Não |
| Criar feature | Developer | Não |
| Alterar governance | Admin | Humana |
| Alterar segurança | Security | Humana |
| Modificar memória global | Admin | Humana |
| Promover conhecimento | Curador | Humana |
| Aprovar release | Lead | Humana |
| Executar migration | Admin | Humana |
| Acessar dados LGPD | Compliance | Humana |
| Alterar RBAC | SuperAdmin | Humana |

## Procedimento

1. Identificar a ação pretendida
2. Consultar .kilo/rules/rbac-rules.md
3. Verificar permissão do contexto atual
4. Se insuficiente → bloquear e sugerir quem pode
5. Se requer aprovação → pause e aguardar confirmação humana
6. Registrar tentativa no audit trail

## Nunca Fazer

- Bypass de permissão com --no-verify
- Executar ações de Admin sem contexto de Admin
- Modificar regras RBAC diretamente
