---
name: xforge-init
description: Golden Command — inicializa e valida TODO o sistema XForge. Detecta automaticamente modelos Ollama, valida knowledge, scripts, rules e manual. Não fixa modelos — adapta ao que está instalado. Use na primeira vez ou sempre que precisar validar.
---

# xforge-init

## Objetivo
Validar TODO o sistema XForge e mostrar status. Detecta automaticamente os modelos Ollama disponíveis.

## Regra Fundamental

**NÃO fixar modelos no config.** O usuário seleciona o modelo no momento do prompt. O sistema detecta e adapta.

## Procedimento

### Passo 1: Doctor
```powershell
.\.kilo\automation\scripts\doctor.ps1
```

### Passo 2: Ollama (Detecção Automática)
```powershell
try { $r = Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method Get -TimeoutSec 5; $r.models | ForEach-Object { "$($_.name) ($([Math]::Round($_.size/1GB, 1))GB)" } } catch { "Offline" }
```
Mostrar: modelos instalados e capacidades detectadas.

### Passo 3: Knowledge
- `.xforge/knowledge/INDEX.json` existe
- Knowledge files >= 170
- `errors-solutions-graph.json` existe

### Passo 4: Scripts
Verificar scripts principais existem.

### Passo 5: Rules
`.kilo/rules/` com >= 28 arquivos .md

### Passo 6: Manual
`docs/index.html` existe

### Passo 7: Knowledge Validation
```powershell
.\.kilo\automation\scripts\knowledge-validation.ps1
```

### Passo 8: Score
`score = (checks_passing / 8) * 100`

## Saída Esperada
```
[1/8] ✅ Doctor — 24 OK, 0 errors
[2/8] ✅ Ollama — 4 modelos (Embeddings, Large models)
[3/8] ✅ Knowledge — 173 files + INDEX
[4/8] ✅ Scripts — 7 scripts
[5/8] ✅ Rules — 30 rules
[6/8] ✅ Manual — 1753 lines
[7/8] ✅ Validation — 6 areas
[8/8] ✅ Score — 100%
```

## Se Modelo Específico Necessário
Se uma tarefa precisa de modelo grande (ex: 70B+), o agente deve:
1. Detectar capacidade do modelo atual
2. Se insuficiente, pedir ao usuário para selecionar modelo maior
3. Usar o modelo selecionado para a tarefa

## Uso
```
Use the xforge-init skill
```
