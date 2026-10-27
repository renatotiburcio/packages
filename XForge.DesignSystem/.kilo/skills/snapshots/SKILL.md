---
name: snapshots
description: Use when creating, validating, restoring, or comparing snapshots of memory, prompts, workflows, project state, or Engineer configuration.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# snapshots

## Objetivo

Salvar e restaurar estado do projeto de forma consistente.

## O que Snapshot Captura

| Componente | Caminho | Obrigatório |
|------------|---------|:-----------:|
| Memória | .xforge/memory/ | Sim |
| Knowledge | .xforge/knowledge/ | Sim |
| Config | kilo.jsonc + xforge-engineer.config.json | Sim |
| Rules | .kilo/rules/ | Sim |
| Agents | .kilo/agents/ | Não |
| Skills | .kilo/skills/ | Não |
| Decisions | .xforge/decisions/ | Sim |

## Comandos

### Criar Snapshot
```powershell
.kilo/automation/scripts/snapshot-create.ps1 -Name "pre-release-v2"
```

### Listar Snapshots
```powershell
.kilo/automation/scripts/snapshot-list.ps1
```

### Restaurar Snapshot
```powershell
.kilo/automation/scripts/snapshot-restore.ps1 -Name "pre-release-v2"
```

## Regras

- Snapshot antes de migration
- Snapshot antes de release
- Snapshot antes de refactor grande
- Nomear com data + motivo: `2024-01-15-pre-auth-refactor`
- Manter últimos 10 snapshots
- NUNCA deletar snapshot manualmente (usar política de retenção)

## Saída

```json
{
  "snapshot": "2024-01-15-pre-auth-refactor",
  "timestamp": "ISO-8601",
  "components": ["memory", "knowledge", "config", "rules", "decisions"],
  "size": "2.3MB",
  "restorable": true
}
```
