# Project DNA — XForge Design System

## Purpose
Enterprise-grade Blazor component library with AI extensions. Open-source core (MIT) + commercial AI.

## Status
Recognized (2026-06-12) | Deep Analysis Complete (2026-06-13)

## Project Name
XForge Design System

## Business Domain
UI Component Library — Design System for enterprise Blazor applications. AI-native ERP integration.

## Stack
| Layer | Technology |
|---|---|
| Runtime | .NET 10.0 |
| UI | Blazor WebAssembly + Razor Components |
| CSS | Tailwind CSS v4 |
| JS | 25 ES modules (JS interop: xf-*.js) |
| Charts | ECharts 6.0 |
| Rich Text | Quill.js |
| Tests | xUnit 2.9.2 + bUnit 2.3.4 + Stryker |
| AI | 11 providers (OpenAI, Ollama, Gemini, Anthropic, Azure, DeepSeek, OpenRouter, Local) |
| Icons | Font Awesome Pro 7.1.0 (3,692 SVGs) |

## Architecture
- **Pattern**: Razor Class Library (RCL) + Blazor WebAssembly apps
- **Hierarchy**: `XForgeComponentBase` → `XFInputComponentBase` → Components
- **CSS**: Tailwind + CSS custom properties via `XFFormCss` centralizer
- **Design Tokens**: 15 strongly-typed enums
- **AI**: Provider pattern with `IAiProvider` interface + 11 implementations
- **File Convention**: `.razor` + `.razor.cs` + `.types.cs` (3-file pattern)

## Modules
| Module | Type | License | Components |
|---|---|---|---|
| `XForge.DesignSystem` | RCL — Core | MIT | ~100 |
| `XForge.DesignSystem.Shared` | RCL — Boundaries | MIT | Enums |
| `XForge.DesignSystem.AI` | RCL — AI | Commercial | ~52 |
| `XForge.DesignSystem.Docs` | Blazor WASM — Core docs | MIT | 99 pages |
| `XForge.DesignSystem.Docs.AI` | Blazor WASM — AI docs | Commercial | 64 pages |
| `XForge.DesignSystem.Docs.Shared` | RCL — Docs shell | MIT | Shared |
| `XForge.DesignSystem.Tests` | Test project — Core | MIT | ~200 files |
| `XForge.DesignSystem.AI.Tests` | Test project — AI | Commercial | ~350 methods |

## Component Categories (11)
Layout, Navigation, Forms, Data Display, Feedback, Overlays, Typography, Utilities, Icons, Enterprise ERP, AI

## AI Component Categories (21 sub-domains)
Chat, Prompting, Analysis, Insights, Forms, DataGrid, Workflows, Financial, Accounting, Fiscal, Documents, Agents, Audit, Security, Common, MultiModal, Vision, Audio, Utility, Service DTOs, Common DTOs

## Data Stores
N/A (Frontend component library). AI features use `IAiMemoryService` and `IAiAuditService` with in-memory implementations.

## Frontend
- Blazor WebAssembly
- Tailwind CSS v4 with dark/light themes
- Multi-brand support via `data-brand` attribute
- WCAG 2.1 AA accessibility compliance
- i18n (pt-BR / en-US)
- Rail/Sidebar shell architecture

## Tests
- **Framework**: xUnit + bUnit (component testing)
- **Coverage**: 71.5% line, 58.1% branch, 81.5% method (target: 85%)
- **Mutation Testing**: Stryker (blocked on .NET 10.0 preview)
- **Architecture Tests**: Ecosystem boundary validation
- **Docs Quality Tests**: 12 test classes validating documentation completeness

## CI/CD
GitHub Actions configured (BG-040 complete).

## Security Constraints
- API keys stored in config (use env vars/secrets in production)
- Prompt injection detection enabled by default
- Rate limiting configured (60 req/min)
- RBAC is client-side only (KNOWN LIMITATION)
- Regex ReDoS vulnerability in XFRegexValidator
- Validators bypassable (default patterns)

## Coding Conventions
- Component prefix: `XF` (e.g., `XFButton`, `XFDataGrid`)
- AI component prefix: `XFAi` (e.g., `XFAiChat`)
- File pattern: `.razor` + `.razor.cs` + `.types.cs`
- Namespace root: `XForge.DesignSystem`
- CSS: No hardcoded Tailwind — all via `XFFormCss`
- JS: Isolated ES modules in `wwwroot/js/xf-*.js`
- Event naming: `On{Action}` convention
- No versioned naming (no V2/V3 suffixes)

## Known Risks
| Severity | Risk | Mitigation |
|---|---|---|
| CRITICAL | RBAC client-side only | Move to server |
| CRITICAL | Regex ReDoS | Add timeout |
| HIGH | Validators bypassable | Remove defaults |
| HIGH | JS interop crashes in Server mode | Add try/catch guards |
| MEDIUM | PII in audit logs | Implement redaction |
| MEDIUM | Coverage below target (71.5% vs 85%) | Add tests for AI providers, CrossPlatform |

## Key Metrics
| Metric | Value |
|---|---|
| Total Components | ~152 (100 Core + 52 AI) |
| Doc Pages | 163 (99 Core + 64 AI) |
| Test Methods | ~3,500+ |
| AI Providers | 11 |
| Icon SVGs | 3,692 |
| Design Token Enums | 15 |
| JS Interop Modules | 25 |
| Quality Score | 97/100 |
| Backlog Items | 132 (82 complete, 50 pending) |

## Documentation
- `.xforge/project/PROJECT-DNA.md` — Project identity
- `.xforge/project/ARCHITECTURE.md` — Full architecture
- `.xforge/project/COMPONENT-INVENTORY.md` — ~152 components catalog
- `.xforge/project/SECURITY-AUDIT.md` — 11 vulnerabilities found
- `.xforge/project/DECISIONS.md` — 8+ ADRs
- `.xforge/project/BACKLOG.md` — 132 improvement items
- `.xforge/knowledge/domains/design-system/` — 10 knowledge files

## Preferred Commands
```powershell
# Build
dotnet build

# Run docs
dotnet run --project src/XForge.DesignSystem.Docs
dotnet run --project src/XForge.DesignSystem.Docs.AI

# Test
dotnet test
dotnet test src/XForge.DesignSystem.Tests
dotnet test src/XForge.DesignSystem.AI.Tests

# Validate system
.\.kilo\automation\scripts\doctor.ps1
.\.kilo\automation\scripts\knowledge-validation.ps1
```

## Pipelines

### Component Creation
Memory Retrieve → Current Inventory → Reference Analysis (Blazorise, FluentUI, Radzen) → Capability Matrix → API Contract → UX/UI Contract → Accessibility Contract → Performance Contract → Implementation → Docs → Examples → Tests → Review → Memory Update

### Docs Creation
Memory Retrieve → Docs Architecture Check → Page Contract → Live Preview → Code Renderer → API Explorer → Playground → Accessibility Section → Performance Section → Examples → Review → Memory Update

### Gap Analysis
/designsystem-analyze-current → /reference-analyze-all → /designsystem-gap-analysis → /designsystem-backlog → /designsystem-sprint-plan → /component-roadmap → /designsystem-changelog-update → /designsystem-memory-update → /review-designsystem
