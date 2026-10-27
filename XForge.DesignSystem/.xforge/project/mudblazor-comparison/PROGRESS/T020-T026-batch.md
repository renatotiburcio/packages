# T020-T026 - MudBlazor P2 batch (Desejavel: maturidade)

## T020 - XForge.DesignSystem.UnitTests + UnitTests.Viewer pattern
Status: SCAFFOLDED
- New project scaffold: `tests/XForge.DesignSystem.UnitTests.Viewer/`
- Blazor Server app for visual test hosting
- Defer full implementation to T022 (Microsoft.Testing.Platform)

## T021 - Docs Compiler (Roslyn generator) for auto-generate tests
File: `src\XForge.DesignSystem.Generators.DocsCompiler\`
- `XFFComponentSymbol` record (Name, Namespace, Category, Parameters, Events, PublicMethods, Accessibility)
- `XFFDocsCompileResult` record (Components, TotalParameters, TotalEvents)
- `IXFDocsCompiler.Compile(ImmutableArray<INamedTypeSymbol>)`
- Filter: TypeKind==Class && !IsAbstract && inherits ComponentBase

## T022 - Microsoft.Testing.Platform migration
Status: SCAFFOLDED
- Requires xunit.v3 (Blazorise T052)
- Migration path documented but not yet executed

## T023 - IsTrimmable=true + TrimMode=link in csproj
Status: COMPLETE (already in T016 csproj)
- IsTrimmable=true
- TrimMode=link
- Supports NativeAOT

## T024 - Symbols of debug (snupkg) + EmbedUntrackedSources
Status: COMPLETE (already in T016 csproj)
- IncludeSymbols=true
- SymbolPackageFormat=snupkg
- EmbedUntrackedSources=true
- Deterministic=true

## T025 - Publish 153-style release cadence (releases/runner)
Status: DOCUMENTED
- Weekly patch
- Monthly minor
- Quarterly major
- Use git-flow or release-please pattern
- .github/workflows/release.yml (deferred)

## T026 - XForge.DesignSystem.Benchmarks (BenchmarkDotNet)
File: `D:\dev\XForge\components\src\XForge.DesignSystem.Benchmarks\`
- Created during Blazorise T055 scope
- 3 benchmark suites: ClassBuilder, ParameterState, StringBuilderCache
- [MemoryDiagnoser] + [Orderer(FastestToSlowest)] + [GroupBenchmarksBy(ByCategory)]
- Status: COMPLETE (inherited from Blazorise T055)

## Criteria
- [x] All scaffolded or completed
- [x] T023+T024 covered by T016 csproj
- [x] T026 covered by Blazorise T055