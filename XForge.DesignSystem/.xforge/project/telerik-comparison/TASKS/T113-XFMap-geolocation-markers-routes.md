---
id: T113
title: XFMap (geolocation + markers + routes)
priority: P1
status: pending
estimated_time: 32h
dependencies: nenhuma
---

# T113: XFMap

## Objetivo
Componente Map com markers, routes, clustering, geocoding (OpenStreetMap + Mapbox).

## Passos
1. Criar `XForge.DesignSystem/Components/Map/XFMap.razor`
2. Tile providers: OpenStreetMap (default, MIT), Mapbox (opcional)
3. Markers: pin custom (IconClass, Color, Title)
4. Marker clustering (proximidade)
5. Routes: polyline com waypoints, directions (OSRM/Mapbox Directions API)
6. Geolocation: browser API (navigator.geolocation)
7. Geocoding: Nominatim (OSM) ou Mapbox Geocoding
8. Layers: traffic, transit, satellite
9. Draw mode: poligonos, circulos, retangulos
10. Export: PNG, GeoJSON

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Map/XFMap.razor` + `.razor.cs`
- Criar: `src/XForge.DesignSystem/Components/Map/Models/Marker.cs` + `Route.cs`
- Criar: `src/XForge.DesignSystem/Components/Map/Services/GeocodingService.cs`
- Criar: `src/XForge.DesignSystem/Components/Map/Services/RoutingService.cs`
- Criar: `tests/XForge.DesignSystem.Tests/MapTests.cs` (15+ tests)
- Criar: `docs/map/getting-started.md`

## Criterios de Aceitacao
- [ ] OSM tile provider default (MIT, sem custo)
- [ ] Markers com IconClass, Color, Title
- [ ] Marker clustering funcional
- [ ] Routes: polyline + directions
- [ ] Geolocation via browser API
- [ ] Geocoding (OSM Nominatim)
- [ ] Layers: traffic, transit, satellite
- [ ] Draw mode: poligonos, circulos
- [ ] Export PNG, GeoJSON
- [ ] LGPD: sem tracking de usuarios
- [ ] Documentacao + 5 demos

## Notas
- Inspirado em Telerik Map
- OSM como default (MIT, sem custo, sem API key)
- Mapbox opcional para tiles premium
- LGPD: nao enviar geolocation a terceiros