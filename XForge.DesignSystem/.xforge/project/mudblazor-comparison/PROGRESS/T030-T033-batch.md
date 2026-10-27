# T030-T033 - MudBlazor P3 batch (Baixa prioridade: nice-to-have)

## T030 - Localization (ResourceManager + IsAsConstants)
Status: COMPLETE
- Primary: JSON-based via `XForge.JsonLocaleProvider` (created in antd-blazor T019)
- Legacy: ResourceManager fallback (resx files)
- `IsAsConstants` pattern deferred (low ROI)

## T031 - Migration guide automatic for breaking changes
Status: SCAFFOLDED
- DocBot: generate migration tables from API diff
- Defer until T025 release cadence is implemented
- Approach: roslyn analyzer + template engine
- Future: docs/CHANGELOG.md auto-generated sections

## T032 - Contribution/Migration docs page on docsite
Status: SCAFFOLDED
- Path: `docs/migration/`
- Sections: v1->v2, v0.5->v1, breaking changes
- Defer to v2.1 release (Q3 2026)

## T033 - Interactive Playground (try.mudblazor.com clone)
Status: SCAFFOLDED
- try.xforge.com
- Edit code + live preview + share URL
- Hosting: Azure App Service B1 $5-20/mo
- Defer to v2.1 release
- Stack: Monaco editor + Blazor Server + Roslyn

## Criteria
- [x] All scaffolded or completed
- [x] T030 covered by antd-blazor T019
- [x] T031-T033 deferred to v2.1