---
name: github-automation
description: Use when automating GitHub workflows: PR creation, issue management, releases, branch protection, CI/CD.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# github-automation

## Objetivo

Automatizar tarefas repetitivas no GitHub.

## Automações Disponíveis

| Comando | Ação |
|---------|------|
| `/github pr-create` | Criar PR com template |
| `/github issue-create` | Criar issue com labels |
| `/github release` | Criar release com changelog |
| `/github branch-protect` | Configurar branch protection |
| `/github dependabot` | Configurar dependabot |

## PR Template

```markdown
## Summary
- [ ] Bug fix
- [ ] New feature
- [ ] Refactor
- [ ] Documentation

## Changes
- ...

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing done

## Checklist
- [ ] SOLID/SRP followed
- [ ] No preview packages
- [ ] Docs updated
```

## Procedimento

1. Identificar tarefa a automatizar
2. Verificar se gh CLI está disponível
3. Executar comando
4. Verificar resultado
5. Reportar status

## Regras

- NUNCA push para main sem PR
- Sempre usar squash merge
- Sempre incluir tests no PR
- Branch protection em main always
