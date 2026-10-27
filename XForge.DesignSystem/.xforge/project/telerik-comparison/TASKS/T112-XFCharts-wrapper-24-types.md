---
id: T112
title: XFCharts wrapper (24+ types via ECharts 6.0)
priority: P1
status: pending
estimated_time: 32h
dependencies: nenhuma
---

# T112: XFCharts wrapper

## Objetivo
Wrapper component XForgeBranded sobre ECharts 6.0 com 24+ tipos de grafico (Telerik parity).

## Passos
1. Criar `XForge.DesignSystem/Components/Charts/XFChart.razor` (generic)
2. Parametros: `TChartType` enum (24+ tipos: Line, Bar, Area, Pie, Donut, Scatter, Radar, Heatmap, Sankey, Tree, Sunburst, Gauge, Funnel, Candlestick, OHLC, Waterfall, Boxplot, Heatmap, Parallel, Graph, Treemap, etc.)
3. Data binding: `IEnumerable<TItem>` + selector functions
4. Theme integration via T103 (CSS variables)
5. Tooltip custom (T062)
6. Legend configuravel
7. Animation on data change
8. Export: PNG, SVG, PDF
9. Live update (SignalR ou polling) para real-time dashboards
10. Documentacao com 24 demos (1 por tipo)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Charts/XFChart.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Charts/ChartType.cs`
- Criar: `src/XForge.DesignSystem/Components/Charts/Engine/EChartsConfigBuilder.cs`
- Criar: `src/XForge.DesignSystem/Components/Charts/Models/Series.cs`
- Criar: `tests/XForge.DesignSystem.Tests/ChartsTests.cs` (30+ tests)
- Criar: `docs/charts/24-types.md`

## Criterios de Aceitacao
- [ ] 24+ chart types (Telerik parity)
- [ ] Data binding via IEnumerable<TItem>
- [ ] Tooltip custom
- [ ] Theme integration via T103
- [ ] Export PNG/SVG/PDF
- [ ] Live update via SignalR
- [ ] 24 demos
- [ ] Acessibilidade WCAG 2.2
- [ ] Testes: 30+ (1 por tipo + edge cases)

## Notas
- Wrapper sobre ECharts 6.0 (ja usado em XForge)
- Telerik tem 24+ chart types (Area/Bar/Bubble/Candlestick/Column/Donut/Heatmap/Line/OHLC/Pie/Radar Area/Column/Line/Range Area/Bar/Column/Sankey/Scatter/Scatter Line/Stock/Trendline/Waterfall)
- XForge ja usa ECharts; criar wrapper component branded