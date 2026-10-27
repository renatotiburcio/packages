---
id: T021
title: Docs Compiler (Roslyn generator) para auto-gerar tests
priority: P2
status: pending
estimated_time: 40h
dependencies: T001, T020
---

# T021: Docs Compiler (Roslyn generator)

## Objetivo
Criar Roslyn source generator que descobre todos os `*Example.razor` em `Examples/` e gera testes `X_Should_Render` automaticamente, type-safe via `nameof()`.

## Passos
1. Criar `src/XForge.DesignSystem.Docs.Compiler/` (Roslyn generator)
2. Implementar `IIncrementalGenerator` que parseia `Examples/`
3. Emitir `[Generated]` test methods para xUnit
4. Configurar `GenerateDocsTests=true` como MSBuild property
5. Criar `src/XForge.DesignSystem.UnitTests.Docs/` que consome os generated tests
6. Validar que renames de Example sao compiler-checked (nameof)

## Arquivos
- Criar: `src/XForge.DesignSystem.Docs.Compiler/ExampleTestGenerator.cs`
- Criar: `src/XForge.DesignSystem.UnitTests.Docs/`
- Modificar: `XForge.DesignSystem.Docs.csproj` (Examples sao input)

## Criterios de Aceite
- [ ] Generator roda em build incremental
- [ ] Cada Example gera 1 test method
- [ ] Renomear Example quebra build (nameof)
- [ ] Testes passam sem intervencao manual

## Notas
- Inspirado em `MudBlazor.UnitTests.Docs.Generator`
- Approach incremental evita re-parsing desnecessario
- Cuidado: generators sao lentos na primeira build
