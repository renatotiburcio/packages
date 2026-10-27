---
name: backup-restore-expert
description: Expert in backup, restore, migration safety and knowledge preservation.
metadata:
  version: "35.0.0"
  xforge-category: "operations"
---

# backup-restore-engineer-expert

## Objetivo

Garantir backup e restore confiável de dados e configurações.

## O que Backup

| Componente | Frequência | Retenção |
|------------|:----------:|:--------:|
| .xforge/memory/ | Diário | 30 dias |
| .xforge/knowledge/ | Diário | 30 dias |
| .xforge/decisions/ | Sob demanda | Permanente |
| kilo.jsonc | Sob demanda | Permanente |
| .kilo/rules/ | Sob demanda | Permanente |
| .kilo/agents/ | Sob demanda | Permanente |
| Banco de dados | Configurável | Configurável |

## Comandos

### Backup
```powershell
.kilo/automation/scripts/backup-engineer.ps1 -Name "pre-release"
```

### Restore
```powershell
.kilo/automation/scripts/restore/restore-engineer.ps1 -Name "pre-release"
```

### Verificar Integridade
```powershell
.kilo/automation/scripts/backup/verify-backup.ps1 -Name "pre-release"
```

## Procedimento

1. Criar snapshot antes de operação de risco
2. Verificar integridade do backup
3. Testar restore em ambiente seguro
4. Documentar procedure de restore
5. Registrar no audit trail

## Regras

- NUNCA fazer backup de secrets (só vault)
- Sempre testar restore antes de precisar
- Backup antes de migration é obrigatório
- Manter pelo menos 3 backups recentes
