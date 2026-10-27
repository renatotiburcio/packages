---
id: T097
title: SVG optimizer (svgo integration) + Comment Strip
priority: P3
status: pending
estimated_time: 4h
dependencies: T080
---

# T097: SVG optimizer (svgo integration) + Comment Strip

## Objetivo
Integrar svgo (SVG Optimizer) no AssetsGenerator para reduzir tamanho dos SVG files. Tipicamente 30-50% reducao. **CRITICO: garantir que TODOS os comentarios SVG (`<!--...-->`) sao removidos antes de gerar classes C# — license hygiene (D039).**

## Passos
1. Adicionar NuGet: SVGO (Node.js) ou usar biblioteca C# equivalente (e.g. SvgOptimization)
2. Criar wrapper SvgOptimizer static class
3. No IconsCodeGenerator, chamar SvgOptimizer.Optimize() **+ SvgContentSanitizer.Sanitize()** antes de gerar class
4. Preservar viewBox, xmlns, fill, `<title>`, `<desc>`
5. **Minify: remove comments, whitespace, precision reduction** (ja cobre o comment strip do D039)
6. **License hygiene: explicit `Assert_No_External_License_Traces()` test** apos optimize
7. Adicionar flag --no-optimize para manter SVG original (debug)
8. Tests: 10+ (otimizacao, edge cases, preservation, **license trace**)

## Arquivos
- Criar: `tools/XForge.AssetsGenerator/SvgOptimizer.cs`
- Modificar: `tools/XForge.AssetsGenerator/IconsCodeGenerator.cs` (call Optimizer + Sanitizer)
- Modificar: `tools/XForge.AssetsGenerator/EmojisCodeGenerator.cs` (call Optimizer + Sanitizer)
- Modificar: `tools/XForge.AssetsGenerator/Configuration.cs` (add --no-optimize)
- Criar: `tests/XForge.AssetsGenerator.Tests/SvgOptimizerTests.cs` (10+ tests)
- Criar: `tests/XForge.AssetsGenerator.Tests/LicenseTraceTests.cs` (5+ tests)

## Criterios de Aceite
- [ ] SVG files otimizados 30-50%
- [ ] viewBox/xmlns preservados
- [ ] Fill attributes preservados
- [ ] `<title>`/`<desc>` preservados como elementos (NAO em comentarios)
- [ ] Flag --no-optimize funciona
- [ ] **ZERO comentarios SVG** apos optimize (regex `<!--.*?-->` retorna 0 matches)
- [ ] **ZERO referencias Font Awesome / @fontawesome / Fonticons / fontawesome.com** apos optimize
- [ ] Tests: 10+ (otimizacao, edge cases, preservation) + 5+ LicenseTrace
- [ ] Build: 0 erros, 0 warnings

## Notas
- Alternativas: SvgOptimization (C#), SVGO (Node), NPSPEC
- Para AOT: C# lib (sem Node)
- Reduz bundle size do icon system
- **License compliance**: alinhado com D039 (DECISIONS.md) e SH-013 (self-healing-rules.md)
- Font Awesome Pro 7.1.0 e Commercial; XForge e MIT; NAO pode conter FA metadata no bundle distribuido
- Coordenacao com T078: SvgContentSanitizer e a implementacao primaria; T097 e a implementacao secundaria (svgo pipeline)
