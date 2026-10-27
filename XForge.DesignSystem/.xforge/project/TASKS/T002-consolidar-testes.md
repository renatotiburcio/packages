---
id: T002
title: Consolidar testes redundantes
priority: P0
status: pending
estimated_time: 4h
dependencies: T001
---

# T002: Consolidar testes redundantes

## Objetivo
Reduzir 6883 tests para ~2000 testes focados, mantendo ou melhorando cobertura.

## Passos
1. Identificar testes duplicados (ComponentTest + ExtendedTest para mesmo componente)
2. Analisar quais testes são smoke-only (sem valor)
3. Consolidar: manter o arquivo com melhor cobertura, deletar redundantes
4. Reorganizar: um arquivo por componente, subpasta por categoria
5. Validar: rodar todos os testes, garantir 0 failures

## Arquivos
- Deletar: arquivos redundantes em subpastas (AI/, Button/, Forms/, etc.)
- Manter: arquivos em `Components/` root como canônicos
- Modificar: `ComponentCoverageTests.cs` (manter como smoke test único)

## Critérios de Aceite
- [ ] Total de tests reduzido para ~2000
- [ ] 0 failures após consolidação
- [ ] Cobertura mantida ou melhorada
- [ ] Um arquivo canônico por componente

## Notas
- Estimativa: ~50% dos testes são redundantes ou vazios
- Foco: manter testes funcionais (events, states, binding) e deletar smoke-only
