---
id: T061
title: Build-time SCSS/JS via custom MSBuild
priority: P2
status: pending
estimated_time: 8h
dependencies: T015
---

# T061: Build-time SCSS/JS via custom MSBuild

## Objetivo
Adicionar build-time SCSS compilation e JS minification via custom MSBuild Target inline (não usar Radzen.MSBuild 0.0.6 pre-alpha).

## Passos
1. Criar `build/Common.targets` com Target `CompileSass` (BeforeTargets=BeforeBuild)
2. Usar `Microsoft.Web.LibraryManager` ou `LibSassBuilder` (estável)
3. Criar Target `MinifyJs` (AfterTargets=Compile) com Terser via npm
4. Adicionar ItemGroup `<SassFile Include="themes/**/*.scss" />` condicional TFM=net10
5. Adicionar ItemGroup `<Content Remove="wwwroot/css/**/*.css" />` para evitar duplicates
6. Adicionar Target `MoveCss` que move SCSS-compiled para wwwroot/css
7. Documentar uso no README

## Arquivos
- Criar: `build/Common.targets`
- Modificar: `src/XForge.DesignSystem/XForge.DesignSystem.csproj`
- Criar: `docs/build-scss.md`

## Critérios de Aceite
- [ ] `dotnet build` compila SCSS para CSS automaticamente
- [ ] `dotnet build` minifica JS automaticamente
- [ ] Incremental build (não recompila se SCSS não mudou)
- [ ] Funciona em net8/9/10
- [ ] Build 0 warnings
- [ ] Documentação no README

## Notas
- Origem: **Radzen** ⭐ (adaptado)
- Ref: `radzen-blazor/Radzen.Blazor/Radzen.Blazor.csproj:99` (SassCompile Target)
- **NÃO usar Radzen.MSBuild 0.0.6** (pre-alpha, instável)
- Preferir LibSassBuilder (NuGet estável) + custom Target
- 8h = 4h SCSS + 4h JS minify