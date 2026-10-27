# Design System Knowledge — Consolidated Patterns, Rules & Architecture
# Source: info/ folder analysis
# Generated: 2026-06-13

## 1. PROJECT IDENTITY
# Name: XForge DesignSystem & Docs Platform
# Mission: Criar o melhor Design System Blazor/Tailwind enterprise possivel
# Framework: Blazor WebAssembly + Razor Class Library
# Target: net10.0 | CSS: Tailwind CSS v4 | Version: 1.0.0
# License: MIT (Core) / Commercial (AI)

## 2. WORKSPACE STRUCTURE
# src/XForge.DesignSystem         → Componentes Core (~100)
# src/XForge.DesignSystem.AI      → Componentes AI (~52, Commercial)
# src/XForge.DesignSystem.AI.Tests → Testes AI
# src/XForge.DesignSystem.Docs    → Manual, showcase, guideline, catalogo
# src/XForge.DesignSystem.Docs.AI → Docs AI
# src/XForge.DesignSystem.Tests   → Testes unitarios, render, snapshot, a11y
# info/uteis/                     → 3698 SVG icons (Font Awesome Pro 7.1.0)
# info/tests/                     → XForge.DesignSystem.Docs.QualityTests

## 3. GOLDEN RULES
# 1. SRP: Um arquivo = uma responsabilidade
# 2. NUNCA setar [Parameter] diretamente: usar CurrentValue + ValueChanged.InvokeAsync()
# 3. Tailwind-first: sem CSS isolado sem justificativa
# 4. WCAG AAA: aria-checked, aria-describedby, touch 44px, focus 40%
# 5. Dark/light + data-theme em todos os componentes
# 6. Docs com exemplos renderizados e codigo copiavel
# 7. Exemplos reais com o proprio componente (NUNCA HTML generico, NUNCA ellipsis)
# 8. Testes >= 85% cobertura minima

## 4. COMPONENT PIPELINE
# Memory → Inventory → References → Capability Matrix → API Contract → UX Contract
# → A11y Contract → Performance Contract → Implementation → Docs → Tests → Review → Memory

## 5. DOCS PAGE STANDARD
# - XForgeDocsShell com Items= XForgeDocsCatalog.CoreItems
# - Hero: lg:grid-cols-[minmax(0,1fr)_22rem]
# - QuickFacts em XFStack Vertical
# - 26+ @Section calls
# - Helpers locais: Section, Example, Tile, TileGrid, WarningGrid, NameValueTable, ApiTable
# - Records: DocTile, NameValueRow, ApiRow
# - Todos textos com @T(PT-BR, EN-US)
# - Minimo 6 exemplos renderizados (ideal 8-11)
# - Codigo SEM ellipsis, via XFCode Copyable=true
# - sections: Overview, LivePreview, CodePreview, API, Changelog, RelatedComponents
# - Recommended: ProblemSolved, WhenToUse, WhenNotToUse, Anatomy, Variants, States, Events, Methods, Accessibility, Keyboard, Responsiveness, Performance, EnterpriseExamples, AntiPatterns, Tokens

## 6. DOCS FORBIDDEN HTML
# <table → XFTable | <button → XFButton | <input → XFInput/XFTextInput/XFNumberInput
# <select → XFSelect | <textarea → XFTextarea | <details → XFCollapse/XFAccordion

## 7. BLAZOR SERVER vs WASM COMPATIBILITY
# Every docs page MUST have a section covering:
# - InteractiveServer, InteractiveWebAssembly, InteractiveAuto, Static SSR
# - JS interop files and prerender safety
# - Server latency via SignalR considerations
# - @rendermode examples for each supported mode

## 8. SHELL/RAIL/SIDEBAR
# AppShell → Rail, Sidebar, AppBar, ContentArea, Footer
# Rail item = um app | Sidebar = pertence ao app ativo
# ERP apps: Financeiro, Fiscal, Estoque, Contabil, Comercial, RH, Folha, eSocial, Relatorios, Configuracoes

## 9. SVG ICON PIPELINE
# Origem: info/uteis/ (3698 Font Awesome Pro 7.1.0 SVGs)
# Regras: usar XFSvgIcons.MarkupWithSize(nome, 16), NUNCA SVG inline, kebab-case

## 10. DOCS QUALITY TEST PROJECT (info/tests/)
# Project: XForge.DesignSystem.Docs.QualityTests (net10.0, xUnit)
# Tests: PageExistence, RequiredSections, Guardrail (snapshots), AccessibilityCoverage, ChangelogPattern, DesignSystemUsage, Examples, KeyboardCoverage, OverviewPattern, PerformanceCoverage, PublicApiCoverage
# Infrastructure: DocumentationContract (6 core + 16 recommended sections, forbidden HTML, allowed structural HTML), ComponentApiCatalog, DocsFileCatalog, AllowedDocsExceptions, PathResolver

## 11. DOCUMENTATION CONTRACT
# Required sections (enforced):
#   Overview (aliases: Visao geral, Overview, Resumo)
#   LivePreview (aliases: Preview ao vivo, Exemplo renderizado, Preview, Exemplo)
#   CodePreview (aliases: Codigo, Code preview, XFCode)
#   API (aliases: API, Parametros, Parameters)
#   Changelog (aliases: Changelog, Registro de alteracoes)
#   RelatedComponents (aliases: Componentes relacionados, Related components)

## 12. KNOWN ISSUES
# P0 (16 broken): XFAnimate, XFCollapse, XFDataGrid, XFTooltip, XFDragDrop, XFCaptcha, XFSignaturePad, XFSparkline, XFMap, XFScheduler, XFSearch, XFTimeSpan, XFSpeechToText, XFTransferList, XFColorInput, XFColorPicker
# P1 (16 layout): XFGrid, XFRail, XFSidebar, XFSplitter, XFStack + 12 docs with mx-auto max-w-6xl
# P2 (ellipsis): AutocompleteDocs(34), CardDocs(20), ComboboxDocs(7), DatePickerDocs(7), DataGridDocs(5), ListDocs(1), OneTimeInputDocs(2)
# P3 (examples): XFPivotDataGrid, XFCalendar, XFValidation, XFPdfViewer
# XFHighlighter: 0 tests (has docs but no test coverage)
# 48 AI components: docs OK, tests unverified

## 13. COMPONENT INVENTORY SUMMARY
# Total: ~172 components (119 Core + 53 AI)
# Premium: ~70 | Need Review: ~30 | Need Fix: ~16 | Not Verified: ~56
# Test Methods: ~350+ | Icon SVGs: 3698
# Top tested: XFChip(134), XFDialog(98+), XFDrawer(98), XFAvatar(81), XFDivider(81), XFImage(81), XFMenu(79), XFButton(72), XFRadio(72), XFDropdownMenu(73), XFCheckbox(61)

## 14. ARCHITECTURE LAYERS
# 1. Tokens | 2. Primitives | 3. Foundations | 4. Components | 5. Compositions
# 6. Enterprise Patterns | 7. Shell | 8. Docs Integration | 9. Test Harness

## 15. MULTI-AGENT AGENTS
# Primary: xforge-designsystem-orchestrator, xforge-docs-orchestrator, xforge-memory-steward, xforge-reference-intelligence
# Execution: api-explorer-builder, component-creator, component-maintainer, component-refactor, docs-page-builder, example-builder, reference-analyzer, shell-layout-builder, svg-icon-engineer, svg-icon-pipeline, tests-builder
# Review (read-only): accessibility, component-api, designsystem-consistency, docs-quality, memory-quality, performance, reference-fidelity, solid-srp, tests-quality

## 16. SERVICES
# BrandMetadataService (261 LoC) — Brand CSS variables
# NavigationStateService (485 LoC) — Rail state, localStorage
# XFDrawerService — Drawer programmatic API
# XFToastService — Toast programmatic API

## 17. KEY DECISIONS
# D-124: Generic templates (24-section docs template, 7-category test template, 9 anti-patterns, helpers are local)
# D-123: XFTOC markers (Dot/Number/Icon/None), @onclick:preventDefault, PT-BR accents, 6→16 tests
