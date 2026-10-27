---
id: T030
title: SVG comment hygiene em CI
priority: P3
status: pending
estimated_time: 1h
dependencies: nenhuma
---

# T030: SVG comment hygiene em CI

## Objetivo
Garantir que SVGs não contenham comentários de licença comercial.

## Passos
1. Adicionar pre-commit hook para scan SVG
2. Regex: `<!--.*?-->` em arquivos .svg
3. Bloquear commit se encontrar comentário
4. Registrar em `.kilo/automation/scripts/svg-hygiene.ps1`

## Arquivos
- Criar: `.kilo/automation/scripts/svg-hygiene.ps1`
- Modificar: `.githooks/pre-commit`

## Critérios de Aceite
- [ ] Hook instalado
- [ ] SVGs limpos (0 comentários)
- [ ] Commit bloqueado se SVG com comentário
