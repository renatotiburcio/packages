---
id: T088
title: Icon cache + LRU por assembly
priority: P1
status: pending
estimated_time: 4h
dependencies: T078
---

# T088: Icon cache + LRU por assembly

## Objetivo
LRU cache para SVG path strings (evita re-parse do Dictionary). Memory-bounded, thread-safe, scope-aware (per assembly).

## Passos
1. Criar interface IXFIconCache { GetOrAdd(key, factory) }
2. Criar XFIconLruCache : IXFIconCache (MemoryCache + SizeLimit)
3. Implementar LRU eviction (SizeLimit=10MB, eviction 25%)
4. Thread-safe: ConcurrentDictionary + lock
5. Suportar per-assembly scopes (para testes que descartam icones)
6. Metrics: hits, misses, evictions, memory usage
7. Tests: 20+ (LRU eviction, concurrent access, memory bound, scope disposal)

## Arquivos
- Criar: `src/XForge.DesignSystem/Components/Icon/IXFIconCache.cs`
- Criar: `src/XForge.DesignSystem/Components/Icon/XFIconLruCache.cs`
- Modificar: `src/XForge.DesignSystem/Components/Icon/XFIconRegistry.cs` (use cache)
- Criar: `tests/XForge.DesignSystem.Tests/Icon/IconLruCacheTests.cs` (20+ tests)

## Criterios de Aceite
- [ ] Interface IXFIconCache
- [ ] LRU eviction (SizeLimit configurable)
- [ ] Thread-safe (ConcurrentDictionary + lock)
- [ ] Memory bounded (10 MB default)
- [ ] Per-assembly scopes
- [ ] Metrics (hits, misses, evictions, memory)
- [ ] Tests: 20+ (LRU, concurrency, scope, memory)
- [ ] Build: 0 erros, 0 warnings

## Notas
- Necessario para apps com 1000+ icones renderizados
- MemoryCache ja no BCL (Microsoft.Extensions.Caching.Memory)
- LRU pattern: Dictionary + LinkedList (custom ou BCL)
