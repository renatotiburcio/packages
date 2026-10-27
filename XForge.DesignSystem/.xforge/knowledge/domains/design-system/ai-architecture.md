# XForge DesignSystem — AI Architecture Knowledge

## AI Services (4 interfaces)
- `IAiProvider` — Provider abstraction
- `IAiEmbeddingService` — Semantic embeddings (RAG, search, similarity)
- `IAiMemoryService` — Persistent memory (semantic, long-term, session, contextual)
- `IAiAuditService` — Audit trail (prompt/completion/token/cost/risk logging)

## AI Providers (11)
OpenAI, Ollama, Gemini, Anthropic, Azure OpenAI, DeepSeek, OpenRouter, Local LLM, InMemory, Generic

## AI Component Prefix
All AI components use `XFAi*` prefix

## Key AI Components (29 total, all ⭐ XForge-first)

### AI Services Foundation (S-AI001) — 4 components
- IAiProvider, IAiEmbeddingService, IAiMemoryService, IAiAuditService

### AI Core Interaction (S-AI002) — 4 components
- XFAiChat — Enterprise AI chat copilot (streaming, multi-model, RAG, tool calling, 28 params)
- XFAiCommandPalette — VSCode/Copilot-style command palette (fuzzy search, slash commands)
- XFAiPromptBox — Advanced prompt editor (templates, variables, versioning)
- XFAiSuggestionPanel — Contextual suggestions (next actions, predictions)

### AI Data & Analysis (S-AI003) — 5 components
- XFAiDataGridAssistant — Grid AI assistant (NL filtering, explain data, smart grouping)
- XFAiReportBuilder — NL report generation (PDF/Excel/CSV/Word, SQL generation)
- XFAiSqlQueryBuilder — Safe SQL generator (injection protection, read-only enforcement)
- XFAiDashboardInsight — Automatic dashboard insights (trend/outlier detection, forecasting)
- XFAiDocumentReader — Intelligent document reader (OCR, PDF/XML/image/table extraction)

### AI Domain-Specific (S-AI004) — 6 components
- XFAiNFeAnalyzer — Fiscal analysis (XML parsing, CFOP/CST/NCM validation, SPED)
- XFAiStockAdvisor — Stock management AI (forecasting, rupture detection, ABC curve)
- XFAiSalesGoalAdvisor — Sales goal management (ranking, gamification, commission analysis)
- XFAiFinancialConciliator — Financial reconciliation (bank/card, AI matching, fraud detection)
- XFAiAccountingAssistant — Accounting AI (suggested entries, smart DRE, auto-classification)
- XFAiFormAssistant — Form AI assistant (autofill, OCR, smart validation)

### AI Utility (S-AI005) — 8 components
- XFAiRiskBadge — Visual risk indicator (Low/Medium/High/Critical)
- XFAiExplainButton — Explain any element
- XFAiRewriteButton — Intelligent text rewrite (formalize, summarize, translate)
- XFAiValidationSummary — Smart validation (explain, fix suggestions)
- XFAiAuditTimeline — Audit timeline (immutable, hashing, digital signature)
- XFAiHumanApprovalCard — Human approval (approve/reject/edit-before-apply)
- XFAiSourceCitationList — AI source citations (confidence score, source links)
- XFAiMemoryViewer — AI memory visualization

### AI Orchestration (S-AI006) — 2 components
- XFAiAgentConsole — Agent monitoring console (multi-agent orchestration, CEO/Director/Specialist/Worker hierarchy)
- XFAiWorkflowDesigner — Visual workflow builder (drag-drop, AI nodes, approval nodes)

## AI Global Requirements
All AI components must have: Loading states, Skeletons, Error boundaries, Retry, Streaming, Cancelamento, Virtualização, Telemetria, Logging, Observabilidade, Keyboard nav, Screen reader, Dark mode, Mobile responsiveness, Debounce, Throttle, State persistence, AutoSave, Undo/Redo, Audit trail, Explainability, Permission integration

## AI Security Requirements
Anti prompt injection, Rate limiting, Tool restrictions, Role permissions, Context isolation, PII masking, Secure logs, Encryption

## AI Observability
Prompt logs, Completion logs, Agent logs, Tool logs, Performance logs, Cost logs, Token logs

## AI Testing Requirements
Unit tests, Snapshot tests, Accessibility tests, Visual regression, Performance tests, AI hallucination tests, Prompt injection tests

## Pending AI Expansion (BG-081 to BG-100) — 20 items
- Error Diagnostics: XFAiErrorDiagnosticsPanel, XFAiErrorBanner, IAiErrorDiagnosticProvider
- Memory: XFAiMemoryCenter, IAiMemoryService Profile + Operational scopes
- Proactive: XFAiProactiveBriefing, Suggestion Feedback Loop
- Trust: XFAiTrustLevel enum, IAiTrustService, Fact/Inference/Opinion classification
- RAG: IAiRagPipeline service
- Evaluation: IAiEvaluationService, Prompt Regression Tests
- Routine: XFAiRoutineInsightPanel
- Chat: XFAiChat Diagnostic Mode
