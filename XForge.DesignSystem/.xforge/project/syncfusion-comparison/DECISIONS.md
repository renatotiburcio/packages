# Decisoes Tecnicas -- Syncfusion Blazor (Phase 8) -- 8-way

## D051: Block Editor (Notion-like)
- Contexto: Necessidade de editor block-based para CMS/docs/blog
- Opcoes: (a) Quill.js wrapper, (b) TipTap JS wrapper, (c) Native C# implementation
- Escolha: (c) Native C# + JS interop para slash commands
- Rationale: AOT-friendly, MIT, sem dependencia externa, controle total
- Tradeoffs: ~24h upfront vs Quill ~8h, mas licensing clean

## D052: AI Chat Strategy
- Contexto: Multiplas opcoes de AI chat (Syncfusion AI AssistView, Telerik AI Prompt, Telerik Chat, Radzen AIChatService, Havit HxSmart*)
- Opcoes: (a) Reusar Radzen AIChatService (T060), (b) Syncfusion-style AIAssistView, (c) Microsoft.Extensions.AI wrapper
- Escolha: (c) Microsoft.Extensions.AI wrapper (T129) + AIAssistView component (T125)
- Rationale: Vendor-neutral, future-proof, 11 providers ja integrados em XForge
- Tradeoffs: Wrapper ~16h, mas desacopla de qualquer vendor

## D053: 3D Charts
- Contexto: Syncfusion 3D Charts unico entre 8 fontes
- Opcoes: (a) Three.js wrapper, (b) Babylon.js, (c) D3.js 3D
- Escolha: (a) Three.js (industry standard, MIT, 70k stars)
- Rationale: MIT, ecosistema gigante, WebGL/VR/AR ready
- Tradeoffs: Bundle 600KB gzipped; deferred load on demand

## D054: File Manager Architecture
- Contexto: Syncfusion FileManager e MS-only pattern
- Opcoes: (a) Server-side provider pattern (Blazorise style), (b) Client-side only, (c) Hybrid SignalR
- Escolha: (a) Server-side provider com IFileManagerProvider abstraction
- Rationale: Suporte a S3, Azure Blob, local FS, SFTP atraves de providers
- Tradeoffs: ~32h upfront, mas extensivel para N backends

## D055: Query Builder
- Contexto: Syncfusion QueryBuilder unico entre 8 fontes
- Opcoes: (a) Custom UI + Expression Tree compile, (b) SQL string builder, (c) LINQ dynamic
- Escolha: (a) Custom UI + Expression Tree compile
- Rationale: Type-safe, compile-time validation, sem SQL injection
- Tradeoffs: ~24h, mas muito mais seguro

## D056: Microsoft.Extensions.AI Adoption
- Contexto: 8 fontes diferentes, cada uma com seu AI wrapper
- Opcoes: (a) Manter wrappers por vendor, (b) Adotar Microsoft.Extensions.AI
- Escolha: (b) Adotar Microsoft.Extensions.AI como padrao XForge v2.0.0
- Rationale: Official Microsoft, vendor-neutral, 11 providers ja em XForge
- Tradeoffs: Migration effort, mas alinhamento com Microsoft .NET ecosystem

## D057: A11y Components (OTP, Speech)
- Contexto: Syncfusion tem OTPInput e SpeechToText que outras 7 fontes nao tem
- Opcoes: (a) Skip, (b) Implement, (c) Third-party wrapper
- Escolha: (b) Implement nativamente
- Rationale: LGPD Art. 47 a11y compliance, WCAG 2.2 AA target
- Tradeoffs: ~20h combined, mas compliance-ready

## D058: Figma Native Sync (full bidirectional)
- Contexto: T122 ja cobre basic Figma sync; Syncfusion Figma UI Kit + Figma Variables API
- Opcoes: (a) Manter T122 only, (b) Adicionar T142 full bidirectional
- Escolha: (b) Full bidirectional via Figma Variables API + Plugin
- Rationale: Designer-developer handoff seamless, LATAM design teams
- Tradeoffs: ~16h, mas ganha designer ecosystem

## D059: Community License LATAM Strategy
- Contexto: Syncfusion Community License (free <$1M revenue, 5 devs) pode ser referencia
- Opcoes: (a) Replicar Community License, (b) Manter MIT puro
- Escolha: (b) Manter MIT puro (XForge ja e MIT)
- Rationale: Mais simples, sem burocracia, LGPD-friendly
- Tradeoffs: Sem revenue stream de license, mas alinhado com open source
