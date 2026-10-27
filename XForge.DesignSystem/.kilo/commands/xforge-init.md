---
name: xforge-init
description: Golden Command — ativa e valida TODO o sistema XForge de uma vez. Roda doctor, Ollama, knowledge, error graph, scripts, rules, manual e validation. Mostra score de prontidão.
category: xforge-public
agent: code
---

# /xforge-init

## Objetivo
Ativar e validar TODO o sistema XForge de uma vez. Mostra score de prontidão (0-100%).

## Procedimento

### 1. Doctor Check
Execute:
```powershell
.\.kilo\automation\scripts\doctor.ps1
```
Verifique: 0 errors, todos os [OK] presentes.

### 2. Ollama Status
Execute:
```powershell
try { Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method Get -TimeoutSec 5 } catch { "Ollama not running" }
```
Verifique: modelos instalados (gemma4-26b, qwen3-coder-30b, qwen3.6-27b, nomic-embed-text).

### 3. Knowledge Base
Verifique:
- `.xforge/knowledge/INDEX.json` existe
- 173 knowledge files presentes
- `errors-solutions-graph.json` com 20 patterns

### 4. Scripts
Verifique que estes scripts existem:
- `doctor.ps1`
- `xforge-init.ps1`
- `score.ps1`
- `pre-commit.ps1`
- `live-dashboard.ps1`
- `knowledge-validation.ps1`
- `dependency-check.ps1`
- `proactive-intelligence.ps1`

### 5. Rules
Verifique que `.kilo/rules/` tem 30 arquivos .md.

### 6. Manual
Verifique que `docs/index.html` existe com ~1700 linhas.

### 7. Knowledge Validation
Execute:
```powershell
.\.kilo\automation\scripts\knowledge-validation.ps1
```
Verifique: 6 áreas validadas (INSS, IRRF, eSocial, NF-e, Reforma, CNPJ).

### 8. Score Final
Calcule:
```
score = (checks_passing / 8) * 100
```

## Saída Esperada

```
╔══════════════════════════════════════════════════════════╗
║              XFORGE INIT — RESULTADO                   ║
╠══════════════════════════════════════════════════════════╣
║  Doctor:      PASS (24 OK, 0 errors)                   ║
║  Ollama:      PASS (4 models)                          ║
║  Knowledge:   PASS (173 files)                         ║
║  Error Graph: PASS (20 patterns)                       ║
║  Scripts:     PASS (31 available)                      ║
║  Rules:       PASS (30 loaded)                         ║
║  Manual:      PASS (1741 lines)                        ║
║  Validation:  PASS (6 areas)                           ║
║                                                        ║
║  SCORE: 100% (8/8 checks passed)                       ║
╚══════════════════════════════════════════════════════════╝
```

## Se Algo Falhar

1. Rode `doctor.ps1` para detalhes
2. Corrija os issues encontrados
3. Rode `/xforge-init` novamente

## Uso
```
/xforge-init
```
