---
id: T111
title: XFGauges (4 types: Arc/Circular/Linear/Radial)
priority: P1
status: pending
estimated_time: 16h
dependencies: nenhuma
---

# T111: XFGauges

## Objetivo
4 tipos de gauge para dashboards: Arc, Circular, Linear, Radial.

## Passos
1. Criar `XForge.DesignSystem/Components/Gauges/XFGauge.razor`
2. Discriminator: `GaugeType` enum (Arc, Circular, Linear, Radial)
3. Renderizar via SVG (zero dependencia JS)
4. Parametros: Value, Min, Max, Color, Label, ShowValue, ShowTicks
5. Animacao smooth ao mudar value (CSS transition)
6. Compor em dashboards com T112 Charts
7. Suporte a `Range colors` (ex: 0-30 verde, 30-70 amarelo, 70-100 vermelho)
8. WCAG 2.2: aria-valuenow, aria-valuemin, aria-valuemax
9. Documentacao + 4 demos (1 por tipo)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Gauges/XFGauge.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Gauges/GaugeType.cs`
- Criar: `src/XForge.DesignSystem/Components/Gauges/Renderers/ArcRenderer.cs`
- Criar: `src/XForge.DesignSystem/Components/Gauges/Renderers/CircularRenderer.cs`
- Criar: `src/XForge.DesignSystem/Components/Gauges/Renderers/LinearRenderer.cs`
- Criar: `src/XForge.DesignSystem/Components/Gauges/Renderers/RadialRenderer.cs`
- Criar: `tests/XForge.DesignSystem.Tests/GaugeTests.cs` (15 tests)

## Criterios de Aceitacao
- [ ] 4 tipos: Arc, Circular, Linear, Radial
- [ ] Parametros: Value, Min, Max, Color, Label, ShowValue, ShowTicks
- [ ] Animacao smooth ao mudar value
- [ ] Range colors (multi-color ranges)
- [ ] WCAG 2.2: aria-valuenow, aria-valuemin, aria-valuemax
- [ ] SVG-based (zero JS)
- [ ] 4 demos (1 por tipo)
- [ ] Testes: 4 tipos + range colors + accessibility

## Notas
- Inspirado em Telerik Gauges (Arc, Circular, Linear, Radial)
- SVG-based para performance e zero dependencia