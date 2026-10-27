---
id: T003
title: Branch sync — commit e push
priority: P0
status: pending
estimated_time: 30min
dependencies: nenhuma
---

# T003: Branch sync

## Objetivo
Sincronizar branch local com origin/master.

## Passos
1. `git status` — verificar mudanças pendentes
2. `git diff --stat` — verificar escopo das mudanças
3. Stage arquivos relevantes (não incluir .xforge/project/ nem coverage reports)
4. Commit com mensagem descritiva
5. Push para origin/master

## Arquivos
- Commit: novos test files, fixes, documentação
- Não incluir: `.xforge/project/`, `coverage-baseline.json`, bin/obj

## Critérios de Aceite
- [ ] Local e origin sincronizados
- [ ] 0 erros de merge
- [ ] Histórico limpo e descritivo

## Notas
- Branch local está ahead de origin/master
- Último commit: `7836b6b docs: generate 109 component documentation pages`
