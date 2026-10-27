# T022-T026 Batch (P2 small tasks)

## T022 - Microsoft.Testing.Platform migration (8h)
- Status: SCAFFOLDED (docs)
- Goal: migrate from VSTest to Microsoft.Testing.Platform (MTP)
- Reference: T022 in antd-blazor-comparison BACKLOG
- Note: requires T052 (xunit.v3) in Blazorise BACKLOG

## T023 - IsTrimmable + TrimMode=link (2h, T016)
- Status: COMPLETE (in csproj)
- Already applied in T016 csproj
- `<IsTrimmable>true</IsTrimmable>`, `<TrimMode>link</TrimMode>`

## T024 - Snupkg + EmbedUntrackedSources (2h)
- Status: COMPLETE (in csproj)
- Already applied in T016 csproj
- `<IncludeSymbols>true</IncludeSymbols>`, `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`, `<EmbedUntrackedSources>true</EmbedUntrackedSources>`

## T025 - Release cadence (4h, T020)
- Status: SCAFFOLDED (docs)
- Weekly patch (bug fixes)
- Monthly minor (new features)
- Quarterly major (breaking changes)
- Reference: antd-blazor release cadence

## T026 - BenchmarkDotNet (8h, T020)
- Status: SCAFFOLDED (deferred - already in Blazorise T055)
- The Benchmarks project from T055 (XForge.DesignSystem.Benchmarks) covers this.
- Reference: T055 in Blazorise BACKLOG