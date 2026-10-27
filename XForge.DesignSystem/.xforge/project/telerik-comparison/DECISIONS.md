# Decisoes Tecnicas — Telerik (7-way)

## D040: MCP Server como prioridade P0
- **Contexto**: Telerik posiciona MCP Servers como feature #1 ("AI Tools for Telerik UI for Blazor"). Concorrencia tambem tem (Blz T053, Havit 4 tools, Radzen cloud). LATAM precisa de MCP local por LGPD.
- **Opcoes**: (a) Deferred, (b) Cloud-only (Telerik-style), (c) Self-hosted first (Havit-style)
- **Escolha**: (c) self-hosted (10 tools, HTTP transport, OpenTelemetry)
- **Rationale**: LGPD, dados locais, mesmo pattern do Havit (4 tools), mas XForge expande para 10
- **Tradeoffs**: Mais setup local vs dependencia de cloud third-party

## D041: ThemeBuilder web app + Figma sync
- **Contexto**: Telerik oferece online + Figma sync. XForge tem 25 design token enums, sem UI para gerar temas.
- **Opcoes**: (a) Manual (atual), (b) CLI tool, (c) Web app + CLI, (d) Web app + CLI + Figma sync
- **Escolha**: (d) Full stack
- **Rationale**: Telerik provou ROI, Figma sync e diferencial, LATAM designers precisam
- **Tradeoffs**: 32h upfront mas habilita 4 temas profissionais + designer integration

## D042: XForge Playground (REPL)
- **Contexto**: Telerik REPL (blazorrepl.telerik.com) e viral. XForge nao tem nada similar.
- **Opcoes**: (a) Nao fazer, (b) Blazor REPL clone, (c) StackBlitz-style para C# (Monaco + Roslyn in-browser)
- **Escolha**: (b)+(c) — Monaco editor + Roslyn compile via Blazor + share permalink
- **Rationale**: Marketing/SEO, viral potential, dev productivity
- **Tradeoffs**: 24h + hosting de compile in-browser (~30MB)

## D043: Document Processing category
- **Contexto**: Telerik oferece 5 libraries separadas (PDF/Word/Spread/Zip/Streaming). XForge ja tem T028 (QuestPDF generate).
- **Opcoes**: (a) Nao fazer, (b) PDF only (read+edit), (c) PDF + Word, (d) Full (PDF + Word + Spread + Zip)
- **Escolha**: (c) PDF + Word libs (T105) + SpreadProcessing + Zip (T107)
- **Rationale**: Telerik provou valor para ERP LATAM (contratos em PDF, relatorios em Word)
- **Tradeoffs**: 64h para libs completas, mas evita dependencia de MS Office

## D044: Spreadsheet component
- **Contexto**: Telerik Spreadsheet = Excel-like com formulas. Nenhum outro concorrente tem no nivel enterprise.
- **Opcoes**: (a) Nao fazer, (b) Read-only grid, (c) Full Excel-like (formulas, charts, formatacao)
- **Escolha**: (c) full Excel-like
- **Rationale**: Diferencial grande, viral, 48h para MVP (engine de formulas custom)
- **Tradeoffs**: Complexidade alta mas viabilidade comprovada (Telerik)

## D045: Charts wrapper (T112)
- **Contexto**: XForge ja usa ECharts 6.0. Telerik tem 24+ chart types. Criar wrapper component XForgeBranded.
- **Opcoes**: (a) ECharts direto (atual), (b) Wrapper component, (c) Wrapper + custom Telerik-like options
- **Escolha**: (b)+(c) — XFChart<TChartType> generic com Telerik-like API
- **Rationale**: Produtividade, IntelliSense, theme integration
- **Tradeoffs**: 32h para wrapper sobre 24 types

## D046: WebMCP deferred
- **Contexto**: Telerik menciona WebMCP (browser API para agents). E spec W3C emergente.
- **Decisao**: DEFERRED — aguardar spec amadurecer
- **Rationale**: Risco alto, ecossistema ainda nao tem suporte amplo
- **Tradeoffs**: Telerik ja esta investindo; reavaliar em v2.1

## D047: Figma Tokens Sync (T122)
- **Contexto**: Telerik Figma Kits + sync. XForge 25 token enums.
- **Opcoes**: (a) Nao fazer, (b) Export JSON only, (c) Full sync (push + pull) com Figma Variables API
- **Escolha**: (c) Full sync via Figma Variables API
- **Rationale**: Diferencial para designers LATAM, integracao bidirecional
- **Tradeoffs**: 16h + dependencia da Figma API

## D048: Reporting Integration DEFERRED
- **Contexto**: Telerik Reporting e add-on. Demanda LATAM = baixa.
- **Decisao**: DEFERRED para v2.1
- **Rationale**: Nao core, custo de licenca alto, alternativas open source (QuestPDF + SSRS)
- **Tradeoffs**: Perde feature enterprise, mas foca em 80% use cases

## D049: SmartPasteButton (T077b - NEW)
- **Contexto**: Telerik SmartPasteButton = clipboard → form auto-fill com AI.
- **Escolha**: Implementar T077b (4h) dentro de S2 (Havit AI ja planejado)
- **Rationale**: Baixo custo, alto valor, complementa AI components
- **Tradeoffs**: 4h de effort

## D050: Self-hosted MCP (LGPD compliance)
- **Contexto**: Telerik posiciona cloud-first, Radzen cloud. LATAM/LGPD exige local-first.
- **Escolha**: Self-hosted OBRIGATORIO (rejeitar cloud-only)
- **Rationale**: LGPD Art. 46, dados brasileiros, RFB integracao
- **Tradeoffs**: Setup complexity