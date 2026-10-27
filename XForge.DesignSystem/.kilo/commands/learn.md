---
name: learn
description: Registra manualmente um erro/solucao no knowledge graph. Permite ao usuario ensinar o XForge com base em experiencia propria.
category: xforge-core
agent: code
---

# /aprender

## Descricao

Comando para registro manual de aprendizado. Permite ao usuario ensinar o XForge com base em experiencia propria: erros encontrados, solucoes aplicadas, anti-patterns descobertos.

## Uso

`
/aprender --erro <descricao> --solucao <descricao> [--tipo <taskType>] [--tags <tag1,tag2>]
/aprender --anti-pattern <descricao> [--tipo <taskType>] [--tags <tag1,tag2>]
/aprender --regra-ouro <descricao> [--tipo <taskType>] [--tags <tag1,tag2>]
/aprender --listar [--tipo <taskType>] [--min-trust <0-100>]
/aprender --busca <termo>
/aprender --remover <lessonId>
/aprender --export <caminho>
`

## Parametros

| Parametro | Descricao |
|-----------|-----------|
| --erro | Descricao do erro encontrado |
| --solucao | Descricao da solucao aplicada |
| --tipo | Tipo de tarefa: criar, modificar, analisar, corrigir, migrar, documentar, testar, revisar |
| --tags | Tags separadas por virgula |
| --anti-pattern | Descricao do anti-pattern |
| --regra-ouro | Descricao da regra de ouro |
| --listar | Lista todas as licoes registradas |
| --busca | Busca licoes por termo |
| --min-trust | Filtra por trust score minimo |
| --remover | Remove uma licao pelo ID |
| --export | Exporta licoes para arquivo JSON |

## Fluxo de Execucao

1. Validar parametros de entrada.
2. Criar Lesson object com trust score inicial (50 para manual).
3. Adicionar ao knowledge graph (errors-solutions-graph.json).
4. Registrar em feedback-log.jsonl como feedback manual.
5. Confirmar registro ao usuario.

## Exemplos

### Exemplo 1: Registrar erro/solucao

**Input**: /aprender --erro "Migration falha com table already exists" --solucao "Rodar dotnet ef database update 0 antes de recriar" --tipo migrar --tags database,ef-core

**Output**:
`
[Licao registrada]
  ID: LESSON-20260610-M001
  Tipo: solucao-validada
  TaskType: migrar
  Trust Score: 50 percent (manual)
  Tags: database, ef-core
`

### Exemplo 2: Registrar anti-pattern

**Input**: /aprender --anti-pattern "Nao usar ToList() antes de filtrar com LINQ to Entities" --tipo modificar --tags performance,database

**Output**:
`
[Licao registrada]
  ID: LESSON-20260610-M002
  Tipo: anti-pattern
  TaskType: modificar
  Trust Score: 50 percent (manual)
  Tags: performance, database
`

### Exemplo 3: Listar licoes

**Input**: /aprender --listar --tipo migrar

**Output**:
`
[Licoes para migrar]
  1. LESSON-20260610-M001 [solucao-validada] Trust: 50 percent
     Migration falha com table already exists
     Solucao: Rodar dotnet ef database update 0 antes de recriar
`

## Trust Score Inicial

- Registro manual: 50 (media, precisa de confirmacao automatica para subir).
- Registro automatico (feedback-capture): calculado pela frequencia.
- Trust sobe quando a licao e aplicada com sucesso.
- Trust cai quando a licao e ignorada e o erro persiste.

## Integracoes

- **B-036 (knowledge graph)**: adiciona/atualiza licoes.
- **B-034 (feedback-capture)**: registra como feedback manual.
- **B-037 (lesson-applier)**: usa licoes registradas.
