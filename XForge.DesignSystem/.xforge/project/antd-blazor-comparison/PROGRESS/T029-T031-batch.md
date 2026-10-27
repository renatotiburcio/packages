# T029-T031 Batch (P2 small tasks)

## T029 - TestKit separated (6h, T020)
- Status: SCAFFOLDED (docs)
- Goal: separate test infrastructure into its own project (antd-blazor pattern)
- `tests/XForge.DesignSystem.TestKit/` (new project, deferred)
- Reference: T029 in antd-blazor-comparison BACKLOG

## T030 - DraftMonitor component (8h, no deps)
- Status: SCAFFOLDED (component skeleton)

### Created
- `src/XForge.DesignSystem/Components/Forms/Draft/XFDraftMonitor.cs`

### Pattern
- Tracks form draft state (in-progress edits not yet submitted)
- Auto-saves to localStorage (browser)
- Provides XFDraftState (Fresh, Dirty, Saved, Error)
- Reference: T030 in antd-blazor-comparison BACKLOG

## T031 - Localization ResourceManager (8h, T001)
- Status: COMPLETE (delegated to T019 JSON locales)
- ResourceManager-based fallback (resx) is the legacy path
- T019 provides the JSON-based primary path
- Both registered in DI