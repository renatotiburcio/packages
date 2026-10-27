---
id: T104
title: XForge Playground (REPL online, browser-based)
priority: P0
status: pending
estimated_time: 24h
dependencies: nenhuma
---

# T104: XForge Playground (REPL)

## Objetivo
Browser-based playground (estilo blazorrepl.telerik.com) para devs testarem componentes XForge sem criar projeto.

## Passos
1. Criar projeto `XForge.Playground` (Blazor WASM standalone)
2. UI: split-pane (editor C# + preview live + console output)
3. Monaco editor (CDN) para syntax highlighting C#/Razor
4. Compile in-browser via `Microsoft.CodeAnalysis.CSharp.Scripting` (Roslyn scripting)
5. Render preview usando XForge components
6. Share via permalink (gerar ID, salvar snippet em SQLite local)
7. Pre-built snippets: 20 cenarios comuns (form, grid, dashboard, etc.)
8. Embed mode: `<iframe src="playground.xforge.dev/?snippet=abc123">`
9. Deploy em `playground.xforge.dev` (Vercel ou self-hosted)

## Arquivos
- Criar: `src/XForge.Playground/Program.cs`
- Criar: `src/XForge.Playground/Pages/Index.razor`
- Criar: `src/XForge.Playground/Components/CodeEditor.razor` (Monaco wrapper)
- Criar: `src/XForge.Playground/Components/Preview.razor`
- Criar: `src/XForge.Playground/Snippets/` (20 pre-built)
- Criar: `playground.xforge.dev/` (deploy config)

## Criterios de Aceitacao
- [ ] Editor C# com syntax highlighting (Monaco)
- [ ] Compile in-browser <2s para snippets <100 linhas
- [ ] Preview renderiza em <1s apos compile
- [ ] Share permalink funciona (gerar + carregar)
- [ ] 20 pre-built snippets cobrem 80% use cases
- [ ] Embed via iframe funciona
- [ ] SEO: OpenGraph meta tags para share social
- [ ] Viral: botao "Tweet your snippet" integrado

## Notas
- Inspirado em Telerik REPL (https://blazorrepl.telerik.com) + StackBlitz
- Marketing/SEO value altissimo (viral potential)
- Compile in-browser: ~30MB download inicial (Roslyn scripting)
- Agentes podem usar para validar codigo gerado