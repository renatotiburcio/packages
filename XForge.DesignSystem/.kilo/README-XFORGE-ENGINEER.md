# .kilo — XForge Engineer

Camada operacional do Engineer compatível com KiloCode.

Conteúdos:

```text
.kilo/
├── commands/
├── skills/
├── agents/
├── rules/
├── core/
├── automation/
├── checklists/
├── docs-html/
├── mcp/
└── platform/
```

Regra: `.kilo` pode ser atualizada com backup e migração. `.xforge` deve ser preservada.

## Compatibilidade Kilo

`kilo.jsonc` contém APENAS chaves ofiais do Kilo:
- `instructions` — array de paths/globs para regras
- `agent` — definições de agents
- `skills` — paths para skills

Toda configuração XForge customizada (routing, contextWindow, tools, etc.) fica em:
- `.xforge/config/xforge-engineer.config.json`
- `.xforge/config/engineer.config.json`
