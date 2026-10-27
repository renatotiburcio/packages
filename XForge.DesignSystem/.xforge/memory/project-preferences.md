---
id: project-preferences
type: memoria
tags: [memoria, preferencias, config, padroes]
trust: high
updated: 2026-06-09
---

# Preferencias do Projeto

## Idioma
- Interface de comandos: Portugues
- Documentacao tecnica: Portugues
- Codigo: Ingles (padrao)

## Estilo de Codigo
- Agentes: YAML frontmatter (name, description, mode, permission, color)
- Skills: SKILL.md com YAML frontmatter
- Comandos: Markdown com frontmatter (description, agent)
- Scripts: PowerShell (.ps1) para automacao, Python para RAG

## Modelo de IA
- Provider padrao: openrouter
- Modelo padrao: anthropic/claude-sonnet-4-20250514
- Debug: openai/gpt-4o
- Documentacao: openai/gpt-4o (temperature: 0.2)

## Encoding
- Todos os arquivos: UTF-8 sem BOM
- Scripts PS1: WriteAllText(Encoding.UTF8)
- Leitura: StreamReader(UTF8)
- Detecao mojibake: [regex]::IsMatch() (nao usar -match)

## Fluxo de Trabalho
- Validar com doctor.ps1 antes de commit
- Usar /provider para trocar modelo
- Usar /buscar para pesquisa combinada
- Usar /memoria para gerenciar contexto

## Organizacao
- .kilo/ = operacional (pode ser substituido)
- .xforge/ = persistente (nunca deletar)
- scripts/ = utilitarios
- tests/ = testes unitarios
- scaffolding/ = WIP (gRPC, SDK, dashboard)
