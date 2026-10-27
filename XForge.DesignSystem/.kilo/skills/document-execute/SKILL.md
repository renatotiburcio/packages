---
name: document-execute
description: FASE 2 — Executa tarefas do BACKLOG.md com COMPACTAÇÃO POR TAREFA. Cada tarefa: executa → salva → resume em 1 linha → próxima. 15+ tarefas por sessão. Checkpoint automático. Nunca para.
---

# document-execute — Fase 2: Execução Compacta

## OBJETIVO

Executar TODAS as tarefas P0 e P1 do BACKLOG.md. Meta: **15+ tarefas por sessão** via compactação por tarefa.

## REgra #1

**NUNCA PARAR** até completar P0+P1. A única exceção: contexto em 95%+ com checkpoint salvo.

## FLUXO POR TAREFA (repetir para cada tarefa)

### 1. CARREGAR (mínimo de tokens)
```
Ler APENAS:
- .xforge/checkpoints/document-execute.json (saber onde parou)
- .xforge/project/BACKLOG.md (saber qual tarefa fazer)
- .xforge/project/TASKS/TXXX-nome.md (saber o que fazer)
NÃO ler: outros arquivos, conhecimento, documentação
```

### 2. EXECUTAR (sem enrolação)
```
Para cada passo da tarefa:
  → Criar/editar arquivo
  → Validar (existe? compila?)
  → Se falhar: retry 3x
NÃO documentar o que fazendo. FAZER e pronto.
```

### 3. VALIDAR (rápido)
```
Checklist mínimo:
- [ ] Critérios de aceite: OK
- [ ] Build: OK
- [ ] Testes: OK (se existirem)
```

### 4. COMPACTAR (CRÍTICO — Após CADA tarefa)
```
APÓS completar a tarefa, o agente DEVE:

1. Salvar resultado em ARQUIVO (não no chat):
   - Atualizar status no BACKLOG.md → "done"
   - Atualizar .xforge/project/STATUS.md

2. Salvar checkpoint em .xforge/checkpoints/document-execute.json:
   {
     "last": "TXXX",
     "done": ["T001",...,"TXXX"],
     "next": "TYYY",
     "total": "X/Y",
     "ts": "ISO"
   }

3. RESUMIR em 1 LINHA no chat:
   "T001 ✅ Entity.cs criado (150 linhas)"

4. LIMPAR contexto mental:
   - NÃO reler arquivos já processados
   - NÃO manter código no chat
   - APENAS: checkpoint + próxima tarefa
```

### 5. REpetir (volta ao passo 1)

## QUANTAS TAREFAS POR SESSÃO

| Estratégia | Tarefas/sessão | Como |
|-----------|---------------|------|
| SEM compactação | 3-5 | Contexto enche rápido |
| COM compactação | **15-20** | Cada tarefa = 1 linha de resumo |

**A chave**: após cada tarefa, o agente só mantém no contexto:
- 1 linha de resumo ("T001 ✅ arquivo criado")
- Checkpoint (1 JSON pequeno)
- Próxima tarefa (1 arquivo)

Total: ~200 tokens por tarefa → 15+ tarefas em 32k context.

## REGRA DE OURO: 1 LINHA POR TAREFA

```
❌ ERRADO (gasta contexto):
"T001 foi uma tarefa onde criei a entidade FiscalEntry.cs com 
150 linhas de código, incluindo propriedades Id, Value, Date, 
Status, implementando IAuditable, usando xunit para testes..."

✅ CORRETO (economiza contexto):
"T001 ✅ FiscalEntry.cs (150 linhas)"
```

## CHECKPOINT FORMAT

Salve em `.xforge/checkpoints/document-execute.json`:
```json
{
  "last": "T005",
  "done": ["T001","T002","T003","T004","T005"],
  "next": "T006",
  "blocked": [],
  "total": "5/20",
  "ts": "2026-06-12T10:00:00Z"
}
```

## GESTÃO DE CONTEXTO

```
A cada tarefa completa:
  → Contexto cresce ~200 tokens (1 linha + checkpoint update)
  → 15 tarefas = ~3000 tokens de overhead
  → Cabem 15+ tarefas em 32k context

Se contexto > 80%:
  1. Salvar checkpoint
  2. Resumir: "T001-T015 feitos, 15/20, próximo T016"
  3. Carregar: checkpoint + T016
  4. CONTINUAR

Se contexto > 95%:
  1. Salvar checkpoint
  2. "Checkpoint salvo. Envie msg para retomar."
  3. PARAR
```

## RETOMADA

```
Se .xforge/checkpoints/document-execute.json existe:
  1. Ler checkpoint
  2. Ler BACKLOG.md
  3. Ler TXXX (próxima tarefa)
  4. CONTINUAR (não recomeçar)
```

## COMANDOS

```
/document-execute                 → Iniciar loop completo
/document-execute retomar         → Retomar de checkpoint
/document-execute tarefa T001     → Executar tarefa específica
/document-execute listar          → Listar pendentes
/document-execute status          → Progresso atual
```

## EXEMPLO REAL (15 tarefas em 1 sessão)

```
Usuário: /document-execute

Agente: Lendo backlog... 20 tarefas P0/P1.

T001 ✅ Entity.cs (150 linhas)
T002 ✅ Repository.cs (200 linhas)
T003 ✅ Service.cs (180 linhas)
T004 ✅ Controller.cs (120 linhas)
T005 ✅ Validator.cs (90 linhas)
T006 ✅ Tests.cs (250 linhas)
T007 ✅ Migrations (3 arquivos)
T008 ✅ Config.cs (40 linhas)
T009 ✅ DTOs.cs (80 linhas)
T010 ✅ Mapping.cs (60 linhas)
T011 ✅ Middleware.cs (100 linhas)
T012 ✅ Extensions.cs (70 linhas)
T013 ✅ README.md (atualizado)
T014 ✅ Program.cs (modificado)
T015 ✅ Tests passando

[15/20 tarefas — 15 linhas no chat]

Checkpoint: T001-T015 feitos. Próximo: T016.
```
