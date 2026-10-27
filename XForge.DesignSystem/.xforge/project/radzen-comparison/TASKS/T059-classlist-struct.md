---
id: T059
title: ClassList struct-based fluent CSS builder
priority: P0
status: pending
estimated_time: 4h
dependencies: nenhuma
---

# T059: ClassList struct-based fluent CSS builder

## Objetivo
Substituir CssBuilder (T010) por ClassList struct-based com API Radzen-like, integrando StringBuilderCache (T058).

## Passos
1. Criar `Utilities/ClassList.cs` como `readonly struct` (não class)
2. Implementar factory `Create(string? className = null)`
3. Implementar `Add(string?, bool condition = true)` fluent
4. Implementar `Add(IReadOnlyDictionary<string, object>?)` que lê attr `class`
5. Implementar `Add(FieldIdentifier, EditContext?)` para validação
6. Implementar helpers: `AddDisabled`, `AddVariant`, `AddSize`, `AddShade`, `AddAlignment`
7. Implementar `ToString()` que usa `StringBuilderCache.GetStringAndRelease`
8. Refatorar componentes para usar `ClassList.Create().Add("base").Add("active", isActive)`
9. Benchmarks contra T010 CssBuilder

## Arquivos
- Criar: `src/XForge.DesignSystem/Utilities/ClassList.cs`
- Modificar: `src/XForge.DesignSystem/Components/Base/XForgeComponentBase.cs`
- Refatorar: 5+ componentes para usar ClassList
- Criar: `tests/XForge.DesignSystem.Tests/Utilities/ClassListTests.cs`

## Critérios de Aceite
- [ ] Zero-alloc em hot path (validado com BenchmarkDotNet)
- [ ] Fluent API ergonômica (3+ métodos encadeáveis)
- [ ] Integração com EditContext/FieldIdentifier
- [ ] ToString() usa StringBuilderCache
- [ ] Build 0 warnings
- [ ] Testes cobrem Add(condition), EditContext, helpers

## Notas
- Origem: **Radzen** ⭐
- Ref: `radzen-blazor/Radzen.Blazor/Rendering/ClassList.cs:168`
- **SUPERSEDES T010 (CssBuilder)** — consolidar
- Dep T058 (StringBuilderCache) para perf