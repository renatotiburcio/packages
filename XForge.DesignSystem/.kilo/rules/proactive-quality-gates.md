# Proactive Quality Gates — XForge Engineer

## Visão Geral

Quality gates automáticos que rodam silenciosamente a cada mudança de código, sem intervenção manual. Divididos em 4 estágios.

## Gate 1: Pré-Commit (instantâneo, <2s)

Rodam automaticamente antes de cada commit.

### Checks
| Check | Comando | Critério | Auto-fix |
|-------|---------|----------|----------|
| **Lint C#** | `dotnet format --verify-no-changes` | Sem formatação incorreta | `dotnet format` |
| **Usings** | Verificação de using não utilizados | Zero unused usings | Remoção automática |
| **Null safety** | Análise de FirstOrDefault sem null check | Zero riscos de NullRef | Adição de null check |
| **Secrets** | Grep de padrões de secret | Zero secrets no código | Mover para .env |
| **Self-healing** | SH rules (SH-001 a SH-012) | Zero erros detectáveis | Correção automática |

### Integração Git

```powershell
# Adicionar como pre-commit hook (via .kilo/automation/scripts/setup-hooks.ps1)
#!/bin/sh
powershell -ExecutionPolicy Bypass -File .kilo/automation/scripts/pre-commit.ps1
```

## Gate 2: Pré-Build (<30s)

Rodam antes de `dotnet build`.

### Checks
| Check | Comando | Critério | Auto-fix |
|-------|---------|----------|----------|
| **Restore** | `dotnet restore` | Zero erros de restore | Re-tentar |
| **Build** | `dotnet build --no-restore` | Zero erros, zero warnings | Correção automática |
| **Warnings** | `dotnet build -warnaserror` | Zero warnings | Correção automática |
| **Async safety** | Análise de sync-over-async | Zero .Result/.Wait() | Substituição por await |
| **Docker** | `docker build --check` | Dockerfile válido | Correção de stage |

### Configuração

```json
{
  "preBuild": {
    "enabled": true,
    "failOn": "error",
    "warnAs": "warning",
    "autoFix": true,
    "checks": ["restore", "build", "warnings", "async-safety"]
  }
}
```

## Gate 3: Pré-Push (<60s)

Rodam antes de push para remote.

### Checks
| Check | Comando | Critério | Auto-fix |
|-------|---------|----------|----------|
| **Unit tests** | `dotnet test --no-build` | 100% pass | Re-tentar com isolamento |
| **Coverage** | `dotnet test --collect:"XPlat Code Coverage"` | ≥ 85% | Sugerir testes faltantes |
| **Security scan** | `dotnet list package --vulnerable` | Zero CVEs críticos | Atualizar pacote |
| **NuGet audit** | `dotnet nuget audit` | Zero vulnerabilidades | Atualizar |
| **Build clean** | `dotnet build -warnaserror` | Zero warnings | Correção |

### Configuração

```json
{
  "prePush": {
    "enabled": true,
    "failOn": "error",
    "coverage": {
      "minimum": 85,
      "tool": "coverlet"
    },
    "security": {
      "failOnCritical": true,
      "failOnHigh": false
    }
  }
}
```

## Gate 4: Pós-Merge (<5min)

Rodam após merge na branch principal.

### Checks
| Check | Comando | Critério | Auto-fix |
|-------|---------|----------|----------|
| **Full build** | `dotnet build` | Build limpo | Correção |
| **Full tests** | `dotnet test` | 100% pass | Re-tentar |
| **Coverage** | Coverage report | ≥ 85% | Alerta |
| **Security** | `dotnet list package --vulnerable` | Zero CVEs | Atualizar |
| **Docker build** | `docker build` | Build OK | Correção |
| **Integration tests** | Testcontainers | 100% pass | Re-tentar |

### Configuração

```json
{
  "postMerge": {
    "enabled": true,
    "branch": "main",
    "failOn": "error",
    "notify": true,
    "autoCreateIssue": true
  }
}
```

## Dashboard de Quality Gates

O `/qualidade` command mostra o status de todos os gates:

```
┌─────────────────────────────────────────────┐
│           QUALITY GATES STATUS              │
├─────────────────────────────────────────────┤
│ Pré-Commit  ✅ 12/12 checks pass           │
│ Pré-Build   ✅  5/5 checks pass            │
│ Pré-Push    ⚠️  4/5 checks pass (coverage) │
│ Pós-Merge   ✅  6/6 checks pass            │
├─────────────────────────────────────────────┤
│ Coverage: 87% ✅ (meta: 85%)               │
│ Security: 0 CVEs ✅                        │
│ Warnings: 0 ✅                             │
│ Self-Heals: 3 auto-fixed ✅                │
└─────────────────────────────────────────────┘
```

## Integração com Split Architecture

O **Router** (qwen2.5:7b) detecta a intenção de commit/push e aciona automaticamente o gate apropriado. O **Worker** (qwen2.5:72b) é consultado apenas quando o gate encontra erro que precisa de correção complexa.

```
git commit
  → Router detecta "commit"
  → Pré-commit gate roda (<2s)
  → Se OK: commit prossegue
  → Se erro: Worker corrige automaticamente
  → Re-try commit
```
