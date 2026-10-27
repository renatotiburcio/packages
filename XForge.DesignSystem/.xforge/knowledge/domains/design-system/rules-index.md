# Rules Index — from info/.kilo/rules/

## Rules Files
| File | Purpose |
|---|---|
| 00-project-context.md | Project structure (DesignSystem, Docs, Tests, References, SVG) |
| 10-memory.md | Memory lifecycle (before/after task, files to read) |
| 20-component-engineering.md | Component pipeline, mandatory checks, blockers |
| 25-utf8-ptbr-accentuation.md | PT-BR accent rules for code/docs |
| 30-docs-platform.md | XForge.Docs requirements, page sections, examples, Blazor compat |
| 40-reference-analysis.md | Reference analysis against Blazorise/fluentui/radzen |
| 50-shell-rail-sidebar.md | Shell/Rail/Sidebar structure and app architecture |
| 60-svg-icons.md | SVG icon pipeline rules and outputs |
| 70-tests-quality.md | Mandatory test categories for enterprise components |
| 80-multiagent.md | Multi-agent rules for Kilo, Codex, Antigravity |
| 90-file-safety.md | Files that must never be edited |

## Key Rules Summary

### Component Engineering (20-component-engineering.md)
- SOLID + SRP mandatory
- Tailwind-first, data-theme, dark/light mandatory
- Responsive + MAUI Blazor when applicable
- Block: business logic in component, no docs, no examples, no API contract

### Docs Platform (30-docs-platform.md)
- Every docs page needs: Overview, Problem, WhenToUse, WhenNotToUse, Anatomy, Variants, States, API, Events, Methods, A11y, Keyboard, Responsive, Performance, Examples, Enterprise, Anti-patterns, Tokens, Changelog
- Blazor Server/WASM compatibility section REQUIRED
- No generic HTML when XF component exists
- No ellipsis in example code

### Tests Quality (70-tests-quality.md)
- Per component: unit, rendering, interaction, a11y, snapshot, regression, performance
- Minimum: main params, events, states, variants, keyboard, aria, rendering, bug regression

### Multi-Agent (80-multiagent.md)
- Kilo uses .kilo/agents, .kilo/commands, .kilo/skills, .kilo/rules
- Codex uses AGENTS.md, .codex/config.toml
- Antigravity uses ANTIGRAVITY.md, .antigravity/
- Reviewers are read-only agents

### File Safety (90-file-safety.md)
- NEVER edit: .env, secrets, certs, keys, bin/obj, .git
- NEVER run destructive commands without authorization
