---
name: ldv-validation-checklist
description: Checklist de validacao do Loop de Decomposicao com Validacao. Garante que o loop so termina com 100% OK.
version: 1.0.0
---

# LDV Validation Checklist

## Objetivo

Garantir que o Loop de Decomposicao com Validacao (LDV) so e considerado concluido quando todas as tarefas passam nos criterios de aceite. Nenhuma tarefa fica para tras.

## Fase 1: Validacao da Analise

- [ ] AnalysisResult gerado com requestId unico
- [ ] AnalysisResult.timestamp e valido (ISO-8601)
- [ ] intent.primary classificado corretamente
- [ ] complexity.level justificado com fatores
- [ ] Pelo menos 1 criterio de aceite definido
- [ ] Riscos identificados (mesmo que vazio)
- [ ] requiresHumanReview avaliado
- [ ] Se requiresHumanReview=true, revisao humana concluida

## Fase 2: Validacao da Decomposicao

- [ ] DecompositionResult gerado com decompositionId unico
- [ ] DecompositionResult.requestId referencia AnalysisResult correto
- [ ] totalTasks >= 1
- [ ] Todas as tarefas tem id unico (T-NNN)
- [ ] Todas as tarefas tem title nao-vazio
- [ ] Todas as tarefas tem description clara
- [ ] Todas as tarefas tem agent atribuido
- [ ] Todas as tarefas tem effort estimado (S/M/L)
- [ ] Todas as tarefas tem pelo menos 1 criterio de aceite
- [ ] Todas as tarefas tem maxRetries definido
- [ ] DAG nao tem ciclos (validado algoritmicamente)
- [ ] DAG e conectado (todas as tarefas alcancaveis)
- [ ] executionOrder[] produzido e valido
- [ ] executionOrder[] respeita dependencias do DAG

## Fase 3: Validacao da Execucao (por tarefa)

- [ ] Tarefa tem status pending antes de executar
- [ ] Todas as dependencias estao completed
- [ ] executor-tarefas foi invocado com TaskExecutionRequest valido
- [ ] TaskExecutionResult recebido
- [ ] TaskExecutionResult.taskId corresponde a tarefa
- [ ] TaskExecutionResult.status e valido (completed/failed/skipped)
- [ ] Se completed: todos os criteriaResults tem passed=true
- [ ] Se failed: attemptNumber <= maxRetries
- [ ] Se failed: nextAction definido (retry/skip/escalate)
- [ ] Se failed e maxRetries esgotado: escalacao ou skip executado
- [ ] artifacts[] lista arquivos criados/modificados
- [ ] startedAt e finishedAt preenchidos
- [ ] durationMs calculado corretamente

## Fase 4: Validacao da Consolidacao

- [ ] Todas as tarefas do DAG foram processadas
- [ ] Nenhuma tarefa ficou com status pending
- [ ] Relatorio final gerado com loopId
- [ ] summary.totalTasks == total de tarefas no DAG
- [ ] summary.completed + summary.failed + summary.skipped == summary.totalTasks
- [ ] durationMs total e razoavel
- [ ] Sessao registrada em .xforge/memory/sessions/
- [ ] Licoes aprendidas registradas (se aplicavel)
- [ ] Backlog atualizado (se aplicavel)

## Fase 5: Validacao de Qualidade

- [ ] Nenhum arquivo foi modificado fora do escopo
- [ ] Build passa sem erros
- [ ] Testes existentes nao quebraram
- [ ] Nenhum segredo ou credencial exposto
- [ ] Encoding UTF-8 sem BOM em todos os arquivos criados
- [ ] Nenhum mojibake detectado

## Fase 6: Gate Final

- [ ] Todas as fases acima marcadas
- [ ] 0 erros criticos
- [ ] 0 tarefas pendentes
- [ ] Usuario informado do resultado
- [ ] Status final: completed, partial, failed, ou cancelled

## Nao conformidades

Se qualquer item acima nao for satisfeito:

| Severidade | Acao |
|------------|------|
| CRITICA | Abortar loop, reportar ao usuario, preservar estado |
| ALTA | Tentar corrigir automaticamente, re-validar |
| MEDIA | Registrar warning, continuar com atencao |
| BAIXA | Registrar para proxima iteracao |

## Metricas

Ao final do loop, coletar:

- Taxa de sucesso: completed / totalTasks * 100
- Taxa de retry: total retries / totalTasks
- Taxa de escalacao: escalated / totalTasks
- Tempo medio por tarefa: durationMs / totalTasks
- Licoes aprendidas: count(lessons)
