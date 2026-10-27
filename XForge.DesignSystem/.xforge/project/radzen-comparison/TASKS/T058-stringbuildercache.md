---
id: T058
title: StringBuilderCache ThreadStatic para perf
priority: P1
status: pending
estimated_time: 2h
dependencies: T059
---

# T058: StringBuilderCache ThreadStatic para perf

## Objetivo
Implementar pool de StringBuilder por thread para evitar allocs no hot path de rendering.

## Passos
1. Criar `Utilities/StringBuilderCache.cs` (static class, MIT license)
2. Implementar `[ThreadStatic] StringBuilder? t_cachedInstance`
3. Implementar `Acquire(int capacity = 16)` que retorna cached ou new
4. Implementar `Release(StringBuilder sb)` que cacheia se <= 360
5. Implementar `GetStringAndRelease(StringBuilder sb)` helper
6. Refatorar `ClassList.ToString()` (T059) para usar cache
7. Benchmark antes/depois com BenchmarkDotNet

## Arquivos
- Criar: `src/XForge.DesignSystem/Utilities/StringBuilderCache.cs`
- Modificar: `src/XForge.DesignSystem/Utilities/ClassList.cs` (T059)
- Criar: `tests/XForge.DesignSystem.Tests/Utilities/StringBuilderCacheTests.cs`

## Critérios de Aceite
- [ ] Zero alloc em Acquire quando cached
- [ ] MaxBuilderSize=360 respeitado
- [ ] ThreadStatic garante thread-safety
- [ ] BenchmarkDotNet mostra ganho ≥ 30% em hot path
- [ ] Build 0 warnings
- [ ] Testes cobrem Acquire/Release/GetStringAndRelease

## Notas
- Origem: **Radzen** ⭐ (copy MIT do .NET Foundation)
- Ref: `radzen-blazor/Radzen.Blazor/Rendering/StringBuilderCache.cs:58`
- MIT-licensed: pode-se copiar diretamente do Radzen ou .NET Runtime
- Tradeoff: cache por thread consome ~360 bytes/thread