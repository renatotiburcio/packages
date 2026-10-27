---
name: continuous-security-architect
description: Expert em segurança contínua: SAST, DAST, secrets scanning, security gates no CI/CD.
metadata:
  version: "37.0.0"
  xforge-category: "continuous-security"
---

# continuous-security-architect

## Objetivo

Integrar segurança em cada etapa do ciclo de desenvolvimento.

## Pipeline de Segurança

```
Code → SAST → Commit → Secret Scan → PR → DAST → Deploy → Monitor
```

### 1. SAST (Static Analysis)
- .NET: Security Code Scan, SonarQube
- JS/TS: ESLint security plugins
- Python: Bandit
- Executar: toda PR, toda build

### 2. Secret Scan
- Tools: gitleaks, truffleHog, detect-secrets
- Regras: API keys, passwords, tokens, private keys
- Executar: todo commit, todo push

### 3. DAST (Dynamic Analysis)
- Tools: OWASP ZAP, Burp Suite
- Target: endpoints de API
- Executar: pré-release, semanal

### 4. Dependency Scan
- .NET: dotnet list package --vulnerable
- Node: npm audit
- Tools: Snyk, Dependabot
- Executar: diário, toda atualização

### 5. Container Scan
- Tools: Trivy, Grype, Snyk Container
- Targets: Dockerfile, images
- Executar: toda build de imagem

## Security Gates no CI/CD

| Gate | Critério | Bloqueia? |
|------|----------|:---------:|
| SAST | 0 critical/high | Sim |
| Secret scan | 0 secrets | Sim |
| Dep scan | 0 critical | Sim |
| License | 0 GPL in commercial | Sim |
| Container | 0 critical | Sim |

## Procedimento

1. Mapear ferramentas existentes
2. Integrar no CI/CD
3. Definir thresholds
4. Configurar alerts
5. Monitorar e ajustar
