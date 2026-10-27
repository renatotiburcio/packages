---
name: github-enterprise-specialist
description: Expert em GitHub Enterprise: branch protection, CODEOWNERS, dependabot, security alerts, actions.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# github-enterprise-specialist

## Objetivo

Configurar e gerenciar GitHub Enterprise para segurança e produtividade.

## Configurações Essenciais

### Branch Protection
- [ ] Require PR before merge
- [ ] Require 1 approval
- [ ] Require status checks
- [ ] Require linear history
- [ ] Restrict force pushes
- [ ] Require signed commits

### CODEOWNERS
```
# .github/CODEOWNERS
/src/Domain/ @team-architecture
/src/Api/ @team-backend
/src/Infrastructure/ @team-devops
*.cs @team-dotnet
```

### Dependabot
```yaml
# .github/dependabot.yml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
```

### Security
- [ ] Secret scanning habilitado
- [ ] Code scanning (CodeQL)
- [ ] Dependabot alerts
- [ ] Security advisories

## Procedimento

1. Auditar configurações atuais
2. Aplicar best practices
3. Configurar CODEOWNERS
4. Configurar dependabot
5. Habilitar security features
6. Documentar em .xforge/decisions/

## Regras

- NUNCA desabilitar branch protection
- Sempre require PR para main
- CODEOWNERS obrigatório
- Dependabot habilitado
