---
name: data-migration-expert
description: Expert em migração de dados: ETL, transformação, validação, rollback e integridade.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# data-migration-expert

## Objetivo

Migrar dados de forma segura, com integridade e rollback.

## Fases

### 1. Mapeamento
- Identificar origem e destino
- Mapear campos
- Identificar transformações
- Definir regras de validação

### 2. Extração
- Extrair dados de origem
- Validar completude
- Manter backup original
- Log de extração

### 3. Transformação
- Aplicar regras de mapeamento
- Limpar dados inconsistentes
- Formatar para destino
- Validar transformação

### 4. Carga
- Carregar em destino (batch)
- Verificar integridade
- Reconciliar origem vs destino
- Log de carga

### 5. Validação
- Contar registros
- Verificar soma de controle
- Testar queries de negócio
- Validar com stakeholders

## Regras

- NUNCA migrar sem backup
- Sempre ter rollback
- Validar integridade pós-migração
- Documentar cada transformação
- Testar com amostra primeiro
