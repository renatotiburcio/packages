# XForge DesignSystem — Task List, Backlog & Coverage

## Component Task List Summary (from ComponentTaskList.md)

| Status | Count |
|---|---|
| Premium | ~70 |
| Needs review | ~30 |
| Needs correction | ~16 |
| Not verified | ~56 |
| Total | ~172 |

## Components with Known Issues (Priority 1)
XFAnimate (wrong animation names), XFCollapse (15 sections, ellipsis), XFDataGrid (out of standard), XFTooltip (wrong examples), XFDragDrop (upload-only examples), XFCaptcha (broken), XFSignaturePad (JSException), XFSparkline (3 bugs), XFMap (4 bugs), XFScheduler (weak examples), XFSearch (needs justification), XFTimeSpan (abnormal buttons), XFSpeechToText (huge icons), XFTransferList (large icons), XFColorInput/Picker (exaggerated palette)

## Layout/Pattern Issues (Priority 2)
XFGrid, XFRail, XFSidebar, XFSplitter, XFStack — all have mx-auto max-w-6xl pattern issue

## Ellipsis in Code (Priority 3)
AutocompleteDocs (34), CardDocs (20), ComboboxDocs (7), DatePickerDocs (7), DataGridDocs (5), ListDocs (1), OneTimeInputDocs (2), LayoutOverviewDocs (5)

## Backlog Summary (132 items total)

| Priority | Count | Status |
|---|---|---|
| P0 | 3 | Complete |
| P1 | 8 | Complete |
| P2 | 30 | Complete |
| P3 | 13 | Complete (1 deferred: MAUI) |
| P4 (AI) | 29 | Complete |
| AI Expansion | 20 | Pending (S-AI007-S-AI010) |
| Stryker Mutation Testing | 13 | In Progress (4 complete, 9 pending) |
| Blazor Server/WASM | 12 | Pending |
| **Total** | **132** | **82 complete, 50 pending** |

## Coverage Baseline (D-120)
| Metric | Value | Target |
|---|---|---|
| Line Coverage | 71.5% | >= 85% |
| Branch Coverage | 58.1% | >= 70% |
| Method Coverage | 81.5% | >= 85% |
| Stryker Score | TBD | >= 80% (general), >= 95% (critical) |

## Key Coverage Gaps (0-30%)
- AI providers: GenericAiProvider(0%), OpenRouterProvider(0%), AzureOpenAiProvider(13.7%), AnthropicProvider(10.3%), GeminiProvider(9%)
- AI components: XFAiSqlQueryBuilder(11%), XFAiDataGridAssistant(11%), XFAiDocumentReader(14.1%)
- CrossPlatform: XFBiometricResult(0%), XFCameraCapture(0%), XFGeoLocation(0%)
- Forms: XFCompareValidator(48.1%), XFSpeechToText(45.9%), XFFileInput(52.6%)
- DataDisplay: XFMap(22.5%), XFChart(51.2%)

## Pending Stryker Items (BG-108 to BG-120)
- Coverlet baseline: Complete (v6.0.4)
- Stryker install: Blocked (.NET 10.0 preview incompatibility with .razor.cs partial classes)
- Vacuous tests fix: Complete
- Validator tests: Complete (22 new tests)
- Security critical >= 95%: Pending (XFCaptcha, XFPasswordStrength, XFNavigationGuard)
- Fiscal/Financial >= 95%: Pending (XFCurrencyInput, XFBarcode, XFQRCode)
- UI standard >= 80%: Pending
- CI/CD quality gates: Pending

## Pending Blazor Server/WASM (BLZ-001 to BLZ-012)
- JS interop try/catch guards (5 components): XFTooltip, XFDialog, XFDrawer, XFContextMenu, XFFocusTrap, XFPopover, XFSplitter, XFSplitButton, XFChart, XFMap, XFAnimate, XFSignaturePad
- Prerender-aware initialization (3 items): NavigationStateService, XForgeDocsMainLayout, XFMap children
- Full Server/WASM architecture (4 items): Convert Docs to Blazor Web App, render mode attributes, PersistentComponentState, prerender-safe CSS

## Capability Matrix Summary
- Blazorise overlap: 82%
- FluentUI overlap: 66%
- Radzen overlap: 86%
- XForge leads: 33 components (no reference equivalent)
- Quality score: 97/100
