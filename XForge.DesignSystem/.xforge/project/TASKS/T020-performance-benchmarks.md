---
id: T020
title: Performance benchmarks — render time, GC pressure
priority: P2
status: pending
estimated_time: 3h
dependencies: T001
---

# T020: Performance benchmarks

## Objetivo
Medir performance de renderização dos componentes críticos.

## Passos
1. Configurar BenchmarkDotNet no projeto de testes
2. Criar benchmarks para: DataGrid (1000 rows), TreeView (1000 nodes), Form (50 fields)
3. Medir: render time, memory allocations, GC pressure
4. Salvar baseline de performance

## Arquivos
- Criar: `src/XForge.DesignSystem.Benchmarks/ComponentBenchmarks.cs`
- Criar: `.xforge/project/performance-baseline.json`

## Critérios de Aceite
- [ ] Benchmarks para DataGrid, TreeView, Form
- [ ] Render time < 100ms para 1000 items
- [ ] Memory allocations documentadas
- [ ] Baseline salvo
