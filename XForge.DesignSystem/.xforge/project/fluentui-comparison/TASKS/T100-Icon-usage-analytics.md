---
id: T100
title: Icon usage analytics in demo
priority: P3
status: pending
estimated_time: 2h
dependencies: T085
---

# T100: Icon usage analytics in demo

## Objetivo
Tracking de quais icones sao mais usados no demo (page views, clicks, picks). Insights para priorizar icones pre-curados.

## Passos
1. Criar `Components/Icon/XFIconAnalytics.cs` (in-memory ring buffer)
2. Instrumentar XFIconPicker.razor: track icon pick events
3. Analytics: name, variant, size, timestamp, page
4. API: GetTop(20) → Dictionary<string, int>
5. UI: <XFIconUsageStats /> mostra top 20 icones
6. Persistir em localStorage entre sessoes
7. Export: JSON/CSV download
8. Privacy: sem PII, sem network calls (in-memory only)
9. Tests: 8+ (ring buffer, top, export, persistence)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconAnalytics.cs`
- Criar: `examples/Demo/Pages/IconUsageStats.razor`
- Modificar: `examples/Demo/Pages/IconInspectorDemo.razor` (track picks)
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFIconPicker.razor` (instrument)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconAnalyticsTests.cs` (8+ tests)

## Criterios de Aceite
- [ ] Ring buffer (max 1000 events, FIFO eviction)
- [ ] GetTop(20) retorna top 20 icones
- [ ] UI mostra stats com chart
- [ ] localStorage persistence
- [ ] Export JSON/CSV
- [ ] Privacy: in-memory, sem PII, sem network
- [ ] Tests: 8+ (ring buffer, top, export, persistence)
- [ ] Build: 0 erros, 0 warnings

## Notas
- In-memory apenas (LGPD/GDPR safe)
- Util para priorizar icones pre-curados
- Insights para marketing (quais marcas/icones mais populares)
