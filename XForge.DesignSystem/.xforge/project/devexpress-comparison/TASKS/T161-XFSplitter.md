---
id: T161
title: XFSplitter
priority: P1
status: pending
estimated_time: 8h
dependencies: T001
---

# T161: XFSplitter

## Objetivo
Resizable split panels horizontal/vertical com min/max sizes.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress SEM dependencia comercial

## Descricao
XFSplitter divide a tela em 2 paineis (H/V) com divisor arrastavel, suporte a min/max sizes, collapse/expand, snap positions (25/33/50/66/75/100%), keyboard resizable (Arrow keys quando focado) e server-side state persistido.

## Passos
1. Estudar API DevExpress Splitter no docs.devexpress.com
2. Definir modelo XForge (XFSplitter, SplitterOrientation, SplitterState)
3. Implementar drag handlers com pointer events
4. Persistir state em localStorage (client) ou session (server)
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T161/XFSplitter.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T161/SplitterOrientation.cs
- Criar: src/XForge.DesignSystem/Components/T161/SplitterState.cs
- Criar: src/XForge.DesignSystem/Components/T161/StatePersistenceService.cs
- Criar: src/XForge.DesignSystem.Tests/T161SplitterTests.cs
- Criar: docs/components/t161-splitter/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA (ARIA separator)
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap