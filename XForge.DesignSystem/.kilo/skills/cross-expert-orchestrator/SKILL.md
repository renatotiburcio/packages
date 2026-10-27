---
name: cross-expert-orchestrator
description: Expert em orquestrar múltiplos especialistas para tarefas complexas e multi-domínio.
metadata:
  version: "7.0.0"
  xforge-category: "enterprise-engineer"
---

# cross-expert-orchestrator

## Objetivo

Coordenar múltiplos especialistas para resolver tarefas que transcendem um único domínio.

## Padrão de Orquestração

```
Tarefa Complexa
    ↓
Decompor em sub-tarefas
    ↓
Mapear para especialistas
    ↓
Executar em paralelo/série
    ↓
Consolidar resultados
    ↓
Validar integração
```

## Regras de Orquestração

1. **Max 3 especialistas** por tarefa
2. **Um primário**è´Ÿè´£ coordenação
3. **Comunicação via shared context**
4. **Consolidação antes de finalizar**

## Procedimento

1. Analisar complexidade da tarefa
2. Identificar domínios envolvidos
3. Selecionar especialistas (máx 3)
4. Definir primário e suporte
5. Executar com shared context
6. Consolidar resultados
7. Validar integração
8. Reportar saída unificada

## Saída

```json
{
  "task": "refactor-auth-with-logging",
  "experts": [
    {"name": "auth-security-expert", "role": "primary"},
    {"name": "observability-runtime-expert", "role": "support"}
  ],
  "result": {...},
  "integrated": true
}
```

## Regras

- Nunca mais de 3 especialistas
- Sempre definir primário
- Shared context obrigatório
- Consolidação antes de finalizar
