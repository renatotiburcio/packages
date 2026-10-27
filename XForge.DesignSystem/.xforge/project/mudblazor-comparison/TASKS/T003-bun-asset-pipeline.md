---
id: T003
title: Migrar asset pipeline para Bun (TS + SCSS)
priority: P0
status: pending
estimated_time: 8h
dependencies: nenhuma
---

# T003: Migrar asset pipeline para Bun

## Objetivo
Substituir MSBuild tasks customizadas por Bun para bundling de TypeScript e SCSS, alinhando com MudBlazor e ganhando 10-100x speedup.

## Passos
1. Criar `.config/dotnet-tools.json` com `bundotnet.cli` (Bun wrapper)
2. Criar `src/XForge.DesignSystem/build.mjs` (Bun script: TS + SCSS -> min)
3. Adicionar `<JsAssetInputs>` e `<ScssAssetInputs>` no csproj
4. Adicionar targets `BuildWebAssets` (outer) + `BuildWebAssetsInner` (inner, first TFM)
5. Adicionar property `SkipBunCompile` para C#-only builds
6. Mover 25 ES modules de `TScripts/` para Bun pipeline
7. Testar build local com e sem SkipBunCompile

## Arquivos
- Criar: `.config/dotnet-tools.json`
- Criar: `src/XForge.DesignSystem/build.mjs`
- Modificar: `src/XForge.DesignSystem/XForge.DesignSystem.csproj`
- Mover: `src/XForge.DesignSystem/TScripts/*.ts`

## Criterios de Aceite
- [ ] Build de assets via Bun (sem MSBuild tasks)
- [ ] SkipBunCompile=true pula assets (C# only)
- [ ] Output `XForge.min.js` e `XForge.min.css`
- [ ] Build time 50% menor

## Notas
- MudBlazor migrou para Bun em PR #12518 (Jan 2026)
- Bun lockfile = `bun.lock` (commit)
- Cuidado: Bun nao roda em Windows ARM64 sem update (issue #12746)
