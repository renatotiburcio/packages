---
name: migration-engine-architect
description: Expert em arquitetura de engines de migração: ETL, CDC, event-driven e batch processing.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# migration-engine-architect

## Objetivo

Projetar engines de migração escaláveis e confiáveis.

## Padrões de Migração

### 1. Batch ETL
- Extract de origem em lotes
- Transformação em memória
- Load em destino
-适合 para migrações grandes e lentas

### 2. CDC (Change Data Capture)
- Capturar mudanças em tempo real
- Replicar incrementalmente
-适合 para sincronização contínua

### 3. Event-Driven
- Publicar eventos de mudança
- Consumers processam assincronamente
-适合 para migração distribuída

### 4. Strangler Fig
- Novo sistema absorve funcionalidades
- Migração gradual
- Rollback fácil

## Componentes

```
Source → Extractor → Transformer → Validator → Loader → Destination
                        ↓
                    Dead Letter Queue
                        ↓
                    Retry Logic
```

## Procedimento

1. Analisar volume e complexidade
2. Selecionar padrão
3. Projetar pipeline
4. Implementar com monitoramento
5. Testar com dados de produção (anonymizados)
6. Executar migração
7. Validar integridade
8. Documentar

## Regras

- Idempotência em cada passo
- Dead letter queue para falhas
- Progress tracking
- Rollback automático
- Validação pós-migração
