---
id: T080
title: XF Icons Generator (CLI .NET tools/AssetsGenerator)
priority: P0
status: pending
estimated_time: 16h
dependencies: T078
---

# T080: XF Icons Generator (CLI .NET tools/AssetsGenerator)

## Objetivo
Criar CLI .NET tool que parseia arquivos SVG e gera classes C# tipadas (1 SVG = 1 class). Inspirado em FluentUI.AssetsGenerator.

## Passos
1. Criar projeto `tools/XForge.AssetsGenerator/XForge.AssetsGenerator.csproj` (net10.0 console)
2. Copiar/adaptar Configuration.cs (Microsoft.Extensions.Configuration)
3. Copiar/adaptar IconsCodeGenerator.cs (gera {Variant}{Size}.cs)
4. Copiar/adaptar EmojisCodeGenerator.cs (gera {Group}-{Style}-{Size}.cs)
5. Criar XFModel/Icon.cs (parse filename: `{name}_{size}_{variant}.svg`)
6. Criar XFModel/Emoji.cs (parse filename: `{name}_{style}_{skintone}.svg`)
7. Implementar CLI args: --assets, --target, --namespace, --sizes, --names, --library (icon|emoji), --help
8. Implementar GenerateClass() → 1 class C# tipada por SVG
9. Adicionar cross-link para XFIconCatalog (registro estatico)
10. Documentar uso no README.md do tool
11. Adicionar como dotnet tool local: `dotnet tool install --add-source ./tools`

## Arquivos
- Criar: `tools/XForge.AssetsGenerator/XForge.AssetsGenerator.csproj`
- Criar: `tools/XForge.AssetsGenerator/Program.cs`
- Criar: `tools/XForge.AssetsGenerator/Configuration.cs`
- Criar: `tools/XForge.AssetsGenerator/IconsCodeGenerator.cs`
- Criar: `tools/XForge.AssetsGenerator/EmojisCodeGenerator.cs`
- Criar: `tools/XForge.AssetsGenerator/Model/Icon.cs`
- Criar: `tools/XForge.AssetsGenerator/Model/Emoji.cs`
- Criar: `tools/XForge.AssetsGenerator/README.md`
- Criar: `tools/XForge.AssetsGenerator/Icon.svgs/Regular/Size20/a.svg` (sample 5 SVG para test)

## Criterios de Aceite
- [ ] CLI: `dotnet run --project tools/XForge.AssetsGenerator -- --help` mostra help
- [ ] Parse 1 SVG → gera 1 class C# em --target folder
- [ ] Namespace customizavel (default: XForge.DesignSystem.Components.Icon)
- [ ] --library icon funciona (Fluent pattern: {Variant}{Size}.cs)
- [ ] --library emoji funciona (Fluent pattern: {Group}-{Style}.cs)
- [ ] --sizes 12,24,32 filtra apenas esses sizes
- [ ] --names "add,home" filtra apenas esses icones
- [ ] Gera codigo compativel com XFIconCatalog (T078)
- [ ] Documentado no README (exemplos de uso)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Fork do FluentUI.AssetsGenerator (MIT-licensed, permissivel)
- Adaptar para namespace XForge.DesignSystem
- Usar Microsoft.Extensions.Configuration (ja no ecossistema .NET)
- Output: 1 arquivo .cs por (variant, size) com N classes internas
