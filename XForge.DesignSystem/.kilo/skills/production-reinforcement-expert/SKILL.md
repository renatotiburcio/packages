---
name: production-reinforcement-expert
description: Expert em reforço de produção: hardening, patching, capacity planning e disaster recovery.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# production-reinforcement-expert

## Objetivo

Fortalecer sistemas em produção contra falhas e ameaças.

## Áreas de Reforço

### 1. Hardening
- Atualizar patches de segurança
- Configurar firewalls
- Habilitar encriptação
- Revisar permissões

### 2. Capacity Planning
- Monitorar uso de recursos
- Prever demanda
- Escalar horizontalmente
- Otimizar queries

### 3. Disaster Recovery
- Backup automatizado
- Restore testado
- RTO/RPO definidos
- Runbooks de DR

### 4. Patching
- Dependências atualizadas
- CVEs remediados
- Patches de segurança aplicados
- Regression tests

## Procedimento

1. Auditar estado atual
2. Identificar gaps de segurança
3. Priorizar por risco
4. Aplicar reforços
5. Validar após cada mudança
6. Documentar alterações

## Regras

- Patches de segurança → 48h para critical
- Backup diário obrigatório
- DR testado trimestralmente
- Capacity review mensal
