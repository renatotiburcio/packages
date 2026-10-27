---
id: known-decisions
type: memoria
tags: [memoria, decisoes, arquitetura, escolhas]
trust: high
updated: 2026-06-09
---

# Decisoes Conhecidas

## Arquiteturais

| ID | Decisao | Contexto | Data |
|----|---------|----------|------|
| ADR-0001 | Terminal como template Kilo | Projeto XForge-Development usa o diretorio terminal/ como template | 2026-06-08 |
| DEC-001 | .kilo = operacional, .xforge = persistente | Separar camadas para permitir updates sem perder conhecimento | 2026-06-09 |
| DEC-002 | Agentes em portugues | 36 agentes renomeados para nomes consistentes em PT (kebab-case) | 2026-06-09 |
| DEC-003 | 5 providers no routing | openrouter, openai, anthropic, google, xAI configurados no kilo.jsonc | 2026-06-09 |
| DEC-004 | RAG cache incremental | rag_cache.py com SHA-256 hashing para detectar mudancas | 2026-06-09 |
| DEC-005 | Doctor com 9 secoes | Validacao completa: estrutura, jsonc, registros, cmds, agentes, skills, encoding, connectivity, quality gates | 2026-06-09 |
| DEC-006 | PowerShell 5.1 encoding | Usar [IO.File]::WriteAllText(UTF8) + StreamReader(UTF8) + [regex]::IsMatch() | 2026-06-09 |
| DEC-007 | VS Code Extension excluida | B-020 removido do escopo — usuario confirmou | 2026-06-09 |

## Tecnologicas

| Decisao | Escolha | Alternativas |
|---------|---------|-------------|
| RAG | Python puro (rag_local.py) | LangChain, LlamaIndex |
| CI/CD | GitHub Actions + PowerShell | Azure DevOps, Jenkins |
| Hooks | .githooks/ com core.hooksPath | Husky, lefthook |
| Multi-tenant | JSON configs por tenant | Database-backed |
| Offline | Ollama fallback + cache | Modelo local dedicado |
| Dashboard | Next.js + shadcn/ui | React puro, Vue |
| SDK | TypeScript + npm | Python, Go |
| gRPC | Protocol Buffers | REST, GraphQL |
