---
id: T110
title: XFDiagram (nodes/edges visual workflow editor)
priority: P1
status: pending
estimated_time: 40h
dependencies: nenhuma
---

# T110: XFDiagram

## Objetivo
Componente Diagram com nodes/edges para modelagem visual de workflows e processos.

## Passos
1. Criar `XForge.DesignSystem/Components/Diagram/XFDiagram.razor`
2. SVG-based renderer (pan, zoom, fit-to-screen)
3. Nodes: shape (rectangle, circle, diamond, hexagon), texto, ports (in/out)
4. Edges: tipos (straight, orthogonal, curved), arrows, labels
5. Drag/drop nodes, criar edges clicando em ports
6. Layout algorithms: hierarchical, force-directed, circular
7. Serializacao JSON (save/load diagram)
8. Templates pre-definidos: flowchart, BPMN, ER diagram, network
9. Export para SVG, PNG, PDF (via T028)
10. Integracao com AI: gerar diagrama a partir de texto (via T060)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Diagram/XFDiagram.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Diagram/Engine/LayoutEngine.cs`
- Criar: `src/XForge.DesignSystem/Components/Diagram/Models/Node.cs` + `Edge.cs`
- Criar: `tests/XForge.DesignSystem.Tests/DiagramTests.cs` (20+ tests)
- Criar: `docs/diagram/getting-started.md`

## Criterios de Aceitacao
- [ ] Pan, zoom, fit-to-screen funcionais
- [ ] 6+ shape types
- [ ] 3 edge types (straight, orthogonal, curved)
- [ ] Drag/drop nodes
- [ ] 3 layout algorithms (hierarchical, force, circular)
- [ ] Serializacao JSON
- [ ] 4 templates pre-definidos
- [ ] Export SVG/PNG/PDF
- [ ] AI integration via T060
- [ ] Acessibilidade WCAG 2.2

## Notas
- Inspirado em Telerik Diagram (Updated)
- Diferencial: open source + AI integration
- Layout algorithms: usar biblioteca Sugiyama (hierarchical) ou custom