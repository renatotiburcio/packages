---
name: event-bus
description: Use when a workflow must emit, document, consume, or react to Engineer events such as MEMORY_UPDATED, PROJECT_RECOGNIZED, QUALITY_FAILED, or INCIDENT_RECORDED.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# event-bus

## Objetivo

Emitir e consumir eventos para comunicação desacoplada entre componentes.

## Eventos Obrigatórios

| Evento | Quando Emitir | Dados Mínimos |
|--------|---------------|---------------|
| `MEMORY_UPDATED` | Após salvar memória | key, scope, action |
| `PROJECT_RECOGNIZED` | Após reconhecimento completo | project_id, summary |
| `QUALITY_FAILED` | Quando gate falha | gate, reason, severity |
| `INCIDENT_RECORDED` | Após incidente | severity, impact, resolution |
| `KNOWLEDGE_PROMOTED` | Após promoção | source, target, trust_score |
| `RELEASE_READY` | Todos gates passam | version, changelog |

## Estrutura do Evento

```json
{
  "id": "uuid",
  "type": "EVENT_TYPE",
  "timestamp": "ISO-8601",
  "source": "component-name",
  "data": {},
  "metadata": {
    "version": "1.0",
    "correlationId": "uuid"
  }
}
```

## Procedimento

1. Identificar quando uma ação gera evento
2. Montar payload com dados mínimos
3. Salvar em .xforge/queue/events/
4. Verificar se há handlers registrados
5. Executar handlers se houver
6. Registrar no audit trail
