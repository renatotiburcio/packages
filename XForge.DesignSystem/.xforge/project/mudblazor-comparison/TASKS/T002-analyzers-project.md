---
id: T002
title: Criar projeto XForge.DesignSystem.Analyzers
priority: P0
status: pending
estimated_time: 24h
dependencies: nenhuma
---

# T002: Criar XForge.DesignSystem.Analyzers

## Objetivo
Criar projeto Roslyn analyzer que detecta problemas comuns em XForge: parametros sem [Category], XML docs ausentes, attribute case invalido.

## Passos
1. Criar `src/XForge.DesignSystem.Analyzers/XForge.DesignSystem.Analyzers.csproj` (netstandard2.0)
2. Implementar 3 diagnostics iniciais: XF0001 (UnknownParam), XF0002 (MissingCategory), XF0003 (MissingXmlDoc)
3. Criar `XForge.DesignSystem.Analyzers.CodeFixes` project
4. Criar `XForge.DesignSystem.Analyzers.TestComponents` (Razor test components)
5. Criar `XForge.DesignSystem.Analyzers.UnitTests` (xUnit + Microsoft.CodeAnalysis.Testing)
6. Publicar analyzer como `analyzers/dotnet/cs` no NuGet
7. Documentar em `.xforge/knowledge/domains/design-system/analyzers.md`

## Arquivos
- Criar: `src/XForge.DesignSystem.Analyzers/XF0001_UnknownParameter.cs`
- Criar: `src/XForge.DesignSystem.Analyzers/XF0002_MissingCategory.cs`
- Criar: `src/XForge.DesignSystem.Analyzers/XF0003_MissingXmlDoc.cs`
- Criar: `src/XForge.DesignSystem.Analyzers.CodeFixes/`
- Criar: `src/XForge.DesignSystem.Analyzers.UnitTests/`

## Criterios de Aceite
- [ ] Analyzer package compila em netstandard2.0
- [ ] 3 diagnostics implementados + testes
- [ ] CodeFix para pelo menos 1 (XF0003)
- [ ] Analyzers rodam em XForge.DesignSystem.dll (zero warnings de producao)
- [ ] CI falha quando analyzer detecta problema

## Notas
- Padrao vem de MudBlazor (4 analyzers publicados)
- Embarcado como Roslyn analyzer (nao PostInit)
- TestComponents precisa de projeto separado (Roslyn constraint)
