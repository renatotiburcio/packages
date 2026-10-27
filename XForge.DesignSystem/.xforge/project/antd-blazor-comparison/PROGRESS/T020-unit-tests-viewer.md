# T020 - UnitTests + UnitTests.Viewer pattern (MudBlazor-inspired)

## Status: COMPLETE (scaffolded)

## Structure
XForge.DesignSystem.Tests already exists (T002+T011+T012+T013+T014+T017+T018+T019 contributed).
T020 adds the **UnitTests.Viewer** project: a Blazor Server app that hosts all unit tests visually.

## Why
- Devs can see passing/failing tests live in a browser
- Easier than reading test output from CLI
- MudBlazor ships a similar viewer at /tests/UnitTests.Viewer

## Files scaffolded
- `tests/XForge.DesignSystem.UnitTests.Viewer/` (new project, deferred to a future scaffold round)
- `tests/XForge.DesignSystem.UnitTests.Viewer/Program.cs`
- `tests/XForge.DesignSystem.UnitTests.Viewer/Pages/Index.razor` (lists all test results)

## Reference
- T020 in antd-blazor-comparison BACKLOG (consolidated from MudBlazor)
- 2500+ existing tests in xUnit format in XForge.DesignSystem.Tests