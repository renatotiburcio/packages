---
name: queue
description: Visualiza e gerencia a fila de operacoes offline. Permite reprocessar, remover e limpar itens da fila.
category: xforge-core
agent: code
---

# /fila

## Descricao

Comando para gerenciar a fila de operacoes offline. Quando o XForge esta offline, tarefas sao enfileiradas para execucao posterior.

## Uso

```
/fila                          # Mostra status da fila
/fila --listar                 # Lista todas as tarefas pendentes
/fila --reprocessar            # Reprocessa todas as tarefas pendentes
/fila --remover <id>           # Remove uma tarefa especifica da fila
/fila --limpar                 # Remove todas as tarefas ja processadas
/fila --adicionar <json>       # Adiciona tarefa manualmente (JSON)
/fila --exportar <caminho>     # Exporta fila para arquivo JSON
/fila --importar <caminho>     # Importa tarefas de arquivo JSON
```

## Parametros

| Parametro | Descricao |
|-----------|-----------|
| --listar | Lista todas as tarefas da fila com status |
| --reprocessar | Executa replay de todas as tarefas pendentes |
| --remover | Remove tarefa pelo ID |
| --limpar | Limpa tarefas ja processadas (completed/failed) |
| --adicionar | Adiciona tarefa via JSON string |
| --exportar | Exporta fila para arquivo |
| --importar | Importa tarefas de arquivo |

## Fluxo de Execucao

1. Verificar se a fila existe em ~/.xforge/queue/offline-queue.json
2. Se nao existir: informar que fila esta vazia
3. Executar acao solicitada
4. Mostrar resumo apos operacao

## Formato da Fila

```json
[
  {
    "id": "Q-20260610-001",
    "title": "Criar endpoint GET /api/produtos",
    "type": "criar",
    "complexity": "M",
    "request": "Criar endpoint GET /api/produtos com paginacao",
    "status": "pending",
    "retries": 0,
    "createdAt": "2026-06-10T01:00:00Z",
    "lastAttemptAt": null,
    "completedAt": null,
    "failedAt": null,
    "failureReason": null,
    "result": null
  }
]
```

## Status Possiveis

| Status | Descricao |
|--------|-----------|
| pending | Aguardando processamento |
| completed | Executado com sucesso |
| failed | Falhou apos max retries |

## Exemplos

### Ver status da fila

**Input**: /fila

**Output**:
```
[Fila Offline]
  Pendentes: 3
  Processados: 5
  Falhas: 1
```

### Reprocessar fila

**Input**: /fila --reprocessar

**Output**:
```
[Replay iniciado]
  Processando: Q-001 - Criar endpoint... [OK]
  Processando: Q-002 - Configurar banco... [OK]
  Processando: Q-003 - Testar API... [FAIL] Max retries
[Resumo] 2/3 processados, 1 falha
```

## Integracoes

- **B-045 (offline-manager)**: usa o mesmo arquivo de fila
- **B-046 (replay-queue)**: executa o replay das tarefas
- **B-029 (LDV loop)**: tarefas da fila viram solicitacoes LDV
