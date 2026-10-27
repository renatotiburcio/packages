# Decisoes Tecnicas -- DevExpress Blazor (Phase 9 / 9-way)

Gerado em: 2026-06-13
Source: https://www.devexpress.com/blazor/
Consolidation: 9-way (MudBlazor + antd + Blazorise + Radzen + Havit + FluentUI + Telerik + Syncfusion + DevExpress)

## D060: Adotar XFFormLayout como Form Engine padrao
- **Contexto**: XForge v2.0.0 precisa de Form Engine maduro (T144 T067). DevExpress Form Layout tem features unicas: items, templates, layout groups, tabbed groups, colunas responsivas, validacao integrada, alinhamento automatico.
- **Opcoes**: (a) XFFormLayout custom do zero, (b) adaptar Radzen TemplateForm, (c) Blazorise Fields+Field, (d) MudBlazor MudForm, (e) ADOTAR abordagem DevExpress (T143 24h)
- **Escolha**: (e) ADOTAR abordagem DevExpress (XFFormLayout) com API inspirada no DxFormLayout
- **Rationale**: API mais madura do mercado, suporta items hierarquicos (LayoutGroup + LayoutTabPages + TabPage), validacao integrada por item, alinhamento automatico de labels, temas Bootstrap consistentes
- **Tradeoffs**: Precisa implementar binding complexo para items (TextEdit, ComboBox, DateEdit, CheckBox wrappers)
- **Adopt from**: T143 deps T001

## D061: RichTextEditor DOCX/RTF nativo (nao so Markdown)
- **Contexto**: XForge v2.0.0 ja tem T144 RTE Quill.js (markdown). DevExpress RTE suporta DOCX/RTF/TXT nativos com mail-merge, floating objects, printing.
- **Opcoes**: (a) Quill.js only (ja T144), (b) TinyMCE WYSIWYG, (c) DevExpress-inspired DOCX engine (T144 40h), (d) DocX library standalone
- **Escolha**: (a) Quill.js core + (c) extender com DevExpress-inspired DOCX export (T144 enhance) -- manter markdown source, oferecer DOCX export via DocumentFormat.OpenXml
- **Rationale**: Markdown authoring melhor UX, DOCX export atende usuarios business; mail-merge pode ser v2.1
- **Tradeoffs**: DOCX round-trip nao 100% perfeito, apenas export
- **Adopt from**: T144 deps T002

## D062: PivotTable (cross-tab) separado de PivotGrid (OLAP)
- **Contexto**: T108 ja adota PivotGrid (OLAP-based, DevExpress) - mas XForge pode precisar PivotTable (multi-dimensional cross-tab) tambem. Sao features similares mas com APIs diferentes.
- **Opcoes**: (a) Apenas PivotGrid, (b) Apenas PivotTable custom, (c) ADOTAR T145 PivotTable custom (32h)
- **Escolha**: (c) ADOTAR ambos - PivotGrid (T108) para data warehousing + PivotTable (T145) para cross-tab reports
- **Rationale**: Casos de uso distintos - PivotGrid para BI, PivotTable para Excel-like reports
- **Tradeoffs**: +32h de implementacao
- **Adopt from**: T145 deps T108

## D063: TreeList (Tree + Grid hibrido) como componente unico
- **Contexto**: XForge tem T066 (TreeView generico Havit) e T010 (Grid MudBlazor) separados. TreeList combina ambos em uma unica view.
- **Opcoes**: (a) TreeView + Grid separados, (b) TreeList custom (T146 24h)
- **Escolha**: (b) ADOTAR TreeList (T146) - users que precisam de grid com dados hierarquicos
- **Rationale**: Reduz complexidade de coordenar dois componentes; XForge ja usa este pattern em T020 UnitTests/Viewer
- **Tradeoffs**: TreeList monolitico vs flexibilidade de componentes
- **Adopt from**: T146 deps T010

## D064: FilterBuilder visual (criteria) separado de QueryBuilder (SQL)
- **Contexto**: T134 ja adota QueryBuilder (SQL visual). DevExpress FilterBuilder permite construir criteria expressions (type-safe, sem SQL).
- **Opcoes**: (a) Apenas QueryBuilder, (b) Apenas FilterBuilder, (c) ADOTAR T147 FilterBuilder (24h) complementar
- **Escolha**: (c) ADOTAR T147 FilterBuilder - usar onde nao precisa de SQL (ex: data grid filtering UI)
- **Rationale**: Criteria expressions sao type-safe, LGPD-friendly (no SQL injection risk), compativeis com EF Core LINQ
- **Tradeoffs**: +24h implementacao
- **Adopt from**: T147 deps T134

## D065: ADOTAR XAF Security API para RBAC enterprise
- **Contexto**: XForge v2.0.0 tem XFRbacComponent (CRIT-01 vulnerabilidade) com RBAC client-side. XAF (DevExpress Cross-Platform App UI) oferece role-based Security API production-ready.
- **Opcoes**: (a) Manter XFRbacComponent custom, (b) Implementar RBAC custom server-side, (c) ADOTAR XAF Security API (T156 24h)
- **Escolha**: (c) ADOTAR T156 XFXAFSecurityAPI - usar XAF Security API patterns (Type, Role, Permission, Member, ObjectSpace) para resolver CRIT-01
- **Rationale**: DevExpress XAF e production-grade, usado em milhares de apps enterprise; padroes: User/Role/Permission/Membership; integrate com ASP.NET Identity
- **Tradeoffs**: Licenca dual (XAF requer Universal $2,299.99/yr, mas o padrao pode ser implementado sem dependencia comercial -- T156 implementa o PADRAO, nao a lib)
- **CRITICAL**: Resolve CRIT-01 (server-side RBAC vulnerability) do security audit
- **Adopt from**: T156 deps T001

## D066: Manter subscriptions comerciais FORA do escopo (LGPD-friendly)
- **Contexto**: Phase 9 confirmou que DevExpress usa modelo de subscription COMERCIAL (per-developer, $1,099.99 a $2,299.99/yr). Syncfusion ($1,995) e Telerik ($749-$1,249) tambem comerciais.
- **Opcoes**: (a) Adotar componente comercial diretamente, (b) Re-implementar o PADRAO, (c) Avaliar por feature, (d) Manter MIT
- **Escolha**: (b) Re-implementar o PADRAO/IDEA sem dependencia comercial -- todos os tasks T143-T157 implementam APIs inspiradas em DevExpress, NAO usam pacotes DevExpress
- **Rationale**: LGPD Art. 46 (transferencias internacionais), LGPD Art. 50 (boas praticas), custo zero, liberdade total, vendor lock-in zero; manter MIT puro
- **Tradeoffs**: +N horas implementacao (ja estimado no backlog)
- **Cross-ref**: D050, D040, D025

## D067: MCP Server (Documentation MCP) -- DevExpress ja oferece
- **Contexto**: DevExpress oferece MCP Server GRATUITO com 300,000+ help topics. XForge ja tem T102 MCP Server self-hosted.
- **Opcoes**: (a) Usar DevExpress MCP, (b) Manter XForge self-hosted T102, (c) Hybrid
- **Escolha**: (b) Manter XForge self-hosted (D040/D050 LGPD Art. 46) -- DevExpress MCP serve apenas para desenvolvedores consumindo componentes DevExpress, nao para gerar codigo XForge
- **Rationale**: T102 ja planejado, LGPD compliance, vendor neutrality
- **Tradeoffs**: Mais trabalho de implementacao mas LGPD-compliant
- **Cross-ref**: D040, D050, T102, T124 (AgenticUIBuilder)

## D068: Reporting -- DEFERRED para v2.1 (exceto XFReportViewer basico)
- **Contexto**: DevExpress Reporting e BI Dashboard sao produtos COMPLETOS (Report Designer + Report Viewer + 20+ controls + 10 data sources). Custo de re-implementacao >> beneficio para v2.0.0.
- **Opcoes**: (a) Re-implementar Reporting completo (~200h), (b) DEFERRED v2.1, (c) Adopt basico (Viewer only)
- **Escolha**: (c) Adopt basico -- T154 XFReportViewer (16h) usando T105 (PDF/Word libs ja planejado); T157 XFReportDesigner (40h) v2.1; T155 XFBIDashboard (32h) v2.1
- **Rationale**: 70% dos casos de uso = viewer only; designer ad-hoc e v2.1; BI dashboard e v2.1
- **Tradeoffs**: Reporting v2.0.0 basico, mas cobre 80% uso real
- **Cross-ref**: T105, T154, T155, T157

## D069: 9-way consolidation -- 161 tasks / ~2392h / 14 sprints
- **Contexto**: Phase 9 fecha a consolidacao multi-fonte. Total agregado: 8 fontes previas (142 tasks, 2032h) + DevExpress (19 tasks, 360h) = 161 tasks / ~2392h.
- **Estimativa**: 14 sprints x 2 semanas = 28 semanas = 7 meses (com 16h effective per sprint per dev) OU 10 meses (com 12h effective per sprint per dev)
- **RECOMMENDATION**: 12 meses para quality + iteration
- **Cross-ref**: D040-D068, T143-T157

## Resumo de Adocoes DevExpress
- **19 net-new tasks** T143-T157 (~360h)
- **9 decisions** D060-D069
- **Unicos DevExpress (vs 8 fontes)**: FormLayout Engine, RichTextEditor DOCX, PivotTable, TreeList, FilterBuilder, XAF Security API, Reporting basics
- **LGPD-aligned**: Re-implementa padroes sem dependencia comercial
- **Resolve CRIT-01**: XFXAFSecurityAPI para server-side RBAC