---
id: T076
title: BootstrapIconsGenerator SG IIncremental
priority: P2
status: pending
estimated_time: 4h
dependencies: nenhuma
origin: Havit
---

# T076: BootstrapIconsGenerator SG IIncremental

## Objetivo
Implementar XFIconGenerator : IIncrementalGenerator que parseia Icons/bootstrap-icons.json em build-time e gera classe partial XFIcon com 2000+ propriedades estaticas type-safe. Pattern BootstrapIconsGenerator Havit.

## Passos
1. Criar projeto XForge.DesignSystem.SourceGenerators/ (separate assembly)
2. Adicionar pacote Microsoft.CodeAnalysis 5.3.0 (central managed)
3. Criar XFIconGenerator : IIncrementalGenerator
4. Initialize: AdditionalTextsProvider.Where(path ends with Icons/icon-list.json).Select(GetText).Collect()
5. RegisterSourceOutput: GenerateSourceCode(SourceText, SourceProductionContext)
6. Generate: namespace XForge.DesignSystem { public partial class XFIcon { ... } }
7. Para cada icon name: property XFIcon ICON_NAME => new XFIcon('icon-name')
8. Adicionar XFIcons.json file com 2000+ Bootstrap icons (subset 200+ essenciais)
9. Configurar OutputItemType=Analyzer, ReferenceOutputAssembly=false no csproj
10. Adicionar SourceGenerator no .csproj principal

## Arquivos
- Criar: src/XForge.DesignSystem.SourceGenerators/XFIconGenerator.cs
- Criar: src/XForge.DesignSystem.SourceGenerators/XForge.DesignSystem.SourceGenerators.csproj
- Criar: src/XForge.DesignSystem/Icons/icon-list.json (200+ icons)
- Modificar: src/XForge.DesignSystem/XForge.DesignSystem.csproj (ProjectReference OutputItemType=Analyzer)

## Criterios de Aceite
- [ ] XFIconGenerator compila, IIncrementalGenerator valido
- [ ] Build gera XFIcon.generated.cs com 200+ properties
- [ ] XFIcon.Alarm, XFIcon.Palette, XFIcon.House accessiveis em codigo
- [ ] Build incremental nao recompila quando .json nao muda
- [ ] 0 warnings, 0 errors
