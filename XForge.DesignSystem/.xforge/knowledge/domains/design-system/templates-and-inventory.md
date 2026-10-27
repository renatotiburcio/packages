# Templates & Component Inventory — from info/.kilo/templates/ and info/ComponentTaskList.md

## Templates

### component-doc-template.md
24 mandatory sections for every component documentation page:
1. Overview | 2. Problem Solved | 3. When To Use | 4. When Not To Use
5. Anatomy | 6. Live Preview | 7. Code Preview | 8. Variants
9. States | 10. API (table) | 11. Events | 12. Methods
13. Accessibility | 14. Keyboard Support | 15. Responsiveness | 16. Performance
17. Enterprise Examples | 18. Anti-patterns | 19. Design Tokens | 20. Related Components
21. Changelog

### DESIGNSYSTEM-BACKLOG-TEMPLATE.md
Item format: DS-BL-0001 with fields: Component, Category, Gap, Type, Origin, Reference, Impact, Priority, Risk, Acceptance criteria, Impacted files, Required tests, Required docs, Review gates, Suggested sprint, Status

### Other Templates
- api-contract-template.md — Bilingual (PT-BR/EN-US) API contract
- capability-matrix-template.md — Feature comparison matrix
- CHANGELOG-TEMPLATE.md — Changelog format
- GAP-ANALYSIS-TEMPLATE.md — Gap analysis format
- MEMORY-UPDATE-GAP-TEMPLATE.md — Memory update format
- reference-inventory-template.md — Reference inventory
- review-report-template.md — Review report
- ROADMAP-TEMPLATE.md — Roadmap format
- SPRINT-PLAN-TEMPLATE.md — Sprint plan
- CATEGORY-OVERVIEW-TEMPLATE.md — Category overview page

## Component Inventory Summary (from ComponentTaskList.md)

### Status Distribution
| Status | Count |
|---|---|
| Premium (docs + tests adequate) | ~70 |
| Needs Review | ~30 |
| Needs Fix | ~16 |
| Not Verified | ~56 |
| Total | ~172 |

### P0 — Broken Components (16)
XFAnimate (wrong animation names), XFCollapse (15 sections, ellipsis), XFDataGrid (non-standard), XFTooltip (wrong examples), XFDragDrop (upload-only examples), XFCaptcha (broken JS), XFSignaturePad (JSException), XFSparkline (3 bugs fixed), XFMap (4 bugs fixed), XFScheduler (weak examples), XFSearch (needs justification), XFTimeSpan (abnormal buttons), XFSpeechToText (huge icons), XFTransferList (large icons), XFColorInput (exaggerated palette), XFColorPicker (exaggerated palette)

### P1 — Layout/Pattern Issues (16 docs pages)
XFGrid, XFRail, XFSidebar, XFSplitter, XFStack + 12 docs pages with " mx-auto max-w-6xl\ anti-pattern

### P2 — Ellipsis in Code (7 docs pages)
AutocompleteDocs (34 occurrences), CardDocs (20), ComboboxDocs (7), DatePickerDocs (7), DataGridDocs (5), ListDocs (1), OneTimeInputDocs (2)

### P3 — Example Improvements (4)
XFPivotDataGrid, XFCalendar, XFValidation, XFPdfViewer

### P4 — Premium Core (64 components)
XFButton(72 tests), XFAvatar(81), XFMenu(79), XFToolbar(48), XFDrawer(98), XFContextMenu(54), XFDropdownMenu(73), XFTypography(52), XFDivider(81), XFImage(81), XFList(40), XFPasswordStrength(38), XFNumberInput(21+), XFCheckbox(61), XFRadio(72), XFSwitch(55), XFSlider(25+), XFRangeSlider(21+), XFRating(21+), XFDatePicker(24+), XFDateInput(21+), XFTimeInput(21+), XFCurrencyInput(27+), XFInputMask(34+), XFFileInput(34+), XFAutocomplete(33+), XFCombobox(25+), XFMultiSelect(26+), XFEditForm(26+), XFDropDownDataGrid(54), XFChart(27+), XFCarousel(19+), XFSankey(19+), XFGantt(20+), XFAppShell(44), XFAccordion(49), XFTabs(38), XFSteps(38), XFTOC(17), XFDialog(98+), XFChip(134), XFBadge(26+), XFTable(26+), XFTreeView(26+), XFProgress(26+), XFProgressRing(26+), XFSkeleton(26+), XFToast(20+), XFAlert(26+), XFSplashScreen(26+), XFAnimate(26+), XFChat(26+), XFRichTextEditor(22), XFLottie(20+), XFVirtualList(22), XFSortableList(7), XFNavigationGuard(21), XFBreakpointObserver(7), XFFocusTrap(21), XFKeyCode(21)

### P5 — AI Components (48, docs OK, tests unverified)
XFAiChat, XFAiSqlQueryBuilder, XFAiDataGridAssistant, XFAiReportBuilder, XFAiDocumentReader, XFAiDashboardInsight, XFAiNFeAnalyzer, XFAiAgentConsole, XFAiPromptBox, XFAiCommandPalette, XFAiPerformanceMonitor, XFAiMultiModalChat, XFAiWorkflowDesigner, XFAiWorkflowExecutor, XFAiScheduler, XFAiAutomationRule, XFAiBatchProcessor, XFAiStockAdvisor, XFAiSalesGoalAdvisor, XFAiFinancialConciliator, XFAiFormAssistant, XFAiAccountingAssistant, XFAiAvatar, XFAiAgentPersonality, XFAiProactiveInsight, XFAiConversationContext, XFAiVisionPanel, XFAiVideoAnalyzer, XFAiVoiceAssistant, XFAiOnboardingWizard, XFAiHumanApprovalCard, XFAiAuditTimeline, XFAuditLogger, XFCompliancePanel, XFAuthProvider, XFRbacComponent, XFMicroFrontend, XFComponentGenerator, XFThemeEditor, XFFigmaSync, XFDesignTokenExplorer, XFStorybook, XFAiVirtualScroller, XFAiLazyComponent, XFAiMemoryProfiler, XFAiBundleAnalyzer, XFAiRiskBadge, XFAiSourceCitationList, XFAiValidationSummary, XFAiExplainButton, XFAiRewriteButton, XFAiMemoryViewer, XFAiProviderConfig

### Components Without Tests
- XFHighlighter: 0 tests (docs present)
