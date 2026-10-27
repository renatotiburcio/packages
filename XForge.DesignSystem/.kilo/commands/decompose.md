---
name: decompose
description: Interface publica do LDV. Permite ao usuario decompor qualquer solicitacao em tarefas atomicas e executar com validacao.
category: xforge-core
agent: code
---

# /decompor

## Descricao

Comando principal do Loop de Decomposicao com Validacao (LDV). Recebe qualquer solicitacao e executa o loop completo: analise -> decomposicao -> execucao -> consolidacao.

## Uso

`
/decompor <solicitacao>
/decompor --analise <solicitacao>
/decompor --dag <solicitacao>
/decompor --executar <decomposicao-id>
/decompor --status <loop-id>
/decompor --retry <loop-id>
`

## Parametros

| Parametro | Descricao |
|-----------|-----------|
| solicitacao | Texto livre descrevendo o que fazer |
| --analise | Executa apenas a analise (passo 1), sem decompor |
| --dag | Executa analise + decomposicao (passos 1-3), sem executar |
| --executar | Executa uma decomposicao ja existente |
| --status | Mostra status de um loop em execucao |
| --retry | Re-executa tarefas falhadas de um loop |
| --humano | Forca revisao humana antes de executar |
| --dry-run | Simula sem executar nenhuma acao |

## Fluxo de Execucao

1. O comando invoca o workflow loop-decomposicao-validacao (B-029)
2. O workflow coordena todos os agentes do LDV
3. O resultado e apresentado ao usuario

## Exemplos

### Exemplo 1: Decomposicao completa

**Input**: /decompor Crie um CRUD de clientes com API REST, PostgreSQL e testes

**Output esperado**:
`
[deep-request-analyzer] Analisando solicitacao...
  Intencao: criar
  Complexidade: M (6 tarefas estimadas)
  Areas: backend, database, testes

[task-decomposer] Decompondo...
  T-001: Criar tabela clientes no PostgreSQL
  T-002: Criar entidade Cliente no .NET
  T-003: Criar DTOs ClienteRequest e ClienteResponse
  T-004: Criar repositorio IClienteRepository
  T-005: Criar endpoints CRUD
  T-006: Criar testes de integracao

[executor-tarefas] Executando T-001... [OK]
[executor-tarefas] Executando T-002... [OK]
[executor-tarefas] Executando T-003... [OK]
[executor-tarefas] Executando T-004... [OK]
[executor-tarefas] Executando T-005... [OK]
[executor-tarefas] Executando T-006... [OK]

[consolidacao] Loop concluido!
  Tarefas: 6/6 concluidas
  Duracao: 2m 34s
  Arquivos criados: 12
`

### Exemplo 2: Apenas analise

**Input**: /decompor --analise Preciso migrar o modulo fiscal para .NET 8

**Output esperado**:
`
[deep-request-analyzer] Analisando solicitacao...
  Intencao: migrar
  Complexidade: CRITICA (25+ tarefas estimadas)
  Riscos:
    - ALTO: Regras tributarias complexas
    - ALTO: Dependencia de dados legados
  Requer revisao humana: SIM
  Motivo: Migracao fiscal requer validacao de especialista

[--analise] Decomposicao nao executada (modo analise).
  Use /decompor sem --analise para executar o loop completo.
`

### Exemplo 3: Ver DAG sem executar

**Input**: /decompor --dag Crie autententicacao JWT

**Output esperado**:
`
[deep-request-analyzer] Intencao: criar | Complexidade: M

[DAG de Tarefas]
T-001 (raiz)  -> T-002, T-003
T-002         -> T-004
T-003         -> T-004
T-004 (folha)

Execucao: T-001 -> (T-002 || T-003) -> T-004

[--dag] Execucao nao iniciada.
  Use /decompor --executar <id> para executar.
`

## Nao fazer

- Nao executar o loop se requiresHumanReview=true sem confirmacao
- Nao mostrar output tecnico demais (a menos que --verbose)
- Nao permitir /decompor aninhado (dentro de outro /decompor)

## Integracoes

- **B-029 (workflow)**: loop-decomposicao-validacao
- **B-026 (agente)**: deep-request-analyzer
- **B-027 (agente)**: task-decomposer
- **B-031 (agente)**: executor-tarefas
