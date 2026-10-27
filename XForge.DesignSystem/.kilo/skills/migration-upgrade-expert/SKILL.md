---
name: migration-upgrade-expert
description: Expert em migração e upgrade: .NET, packages, databases, APIs com versionamento.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# migration-upgrade-expert

## Objetivo

Executar migrações e upgrades de forma segura e documentada.

## Tipos de Migração

| Tipo | Exemplo | Risco |
|------|---------|:-----:|
| .NET version | .NET 8 → .NET 10 | Alto |
| Package major | EF Core 8 → 9 | Médio |
| Database | SQL Server → PostgreSQL | Crítico |
| API version | v1 → v2 | Médio |
| Architecture | Monolith → Modular | Alto |

## Procedimento

### 1. Avaliar
- Documentar versão atual
- Identificar breaking changes
- Estimar esforço
- Definir janela de manutenção

### 2. Preparar
- Criar branch migration
- Rodar testes atuais (baseline)
- Backup completo
- Comunicar stakeholders

### 3. Executar
- Aplicar migração passo-a-passo
- Rodar testes após cada passo
- Corrigir issues imediatamente
- Commit incremental

### 4. Validar
- Todos os testes passando
- Build limpo
- Performance aceitável
- Funcionalidade verificada

### 5. Finalizar
- Merge na branch principal
- Atualizar documentação
- Atualizar memória
- Comunicar conclusão

## Regras

- NUNCA migrar sem backup
- Sempre testar baseline primeiro
- Commit incremental
- Rollback planejado
- Comunicação prévia
