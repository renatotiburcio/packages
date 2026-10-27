# Backlog — XForge DesignSystem Premium Gold

Gerado em: 2026-06-26
Fontes analisadas: código fonte completo (155 componentes, 347 test files, 223 docs)

## P0 — Crítico (qualidade base)

| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T001 | Medir cobertura real de testes (baseline 13.9% → alvo 85%) | 2h | nenhuma |
| T002 | Redundância: consolidar 6883 tests → ~2000 testes focados | 4h | T001 |
| T003 | Branch sync: commit + push das mudanças de teste | 30min | nenhuma |

## P1 — Importante (componentes ERP críticos)

| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T010 | DataGrid: testes de sorting, paging, filtering, editing | 3h | T001 |
| T011 | EditForm: testes de validação, state, submit flow | 3h | T001 |
| T012 | Select/Combobox: binding, search, cascading | 2h | T001 |
| T013 | DatePicker/TimeInput: formato BR, cultura, validação | 2h | T001 |
| T14 | FileInput/Upload: progresso, drag-drop, cancelamento | 2h | T001 |
| T015 | Cnpj/Cpf/Cep/Phone输入: máscara, validação BR | 2h | T001 |
| T016 | TreeView: expand/collapse, binding, virtualização | 2h | T001 |
| T017 | Chart: bind points, redraw, tooltip | 2h | T001 |
| T018 | Documentação interativa: live demo em cada doc page | 4h | T010-T017 |

## P2 — Desejável (diferenciação enterprise)

| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T020 | Performance benchmarks: render time, GC pressure | 3h | T001 |
| T021 | Accessibility audit: axe-core, keyboard nav | 3h | T001 |
| T022 | Visual regression: screenshot tests | 3h | T001 |
| T023 | Theme system: contrast, dark mode validation | 2h | T001 |
| T024 | Cross-browser: Playwright tests (Chrome, FF, Safari) | 2h | T001 |

## P3 — Baixa prioridade (polimento)

| ID | Título | Estimativa | Dependências |
|----|--------|-----------|--------------|
| T030 | SVG comment hygiene validation em CI | 1h | nenhuma |
| T031 | Manual sync: docs/index.html | 2h | T018 |
| T032 | Bundle analyzer: tamanho final + tree-shaking | 1h | T001 |
| T033 | Changelog generator integrado | 1h | nenhuma |

## Resumo
- Total: 15 tarefas (P0: 3, P1: 11, P2: 5, P3: 4)
- Estimativa total: ~40h
- Tarefas sem dependência: T003, T001, T030-T033
- Target: 85% cobertura + qualidade premium gold em componentes ERP
