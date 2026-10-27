---
name: criar-agent
description: Cria novo agente no sistema XForge. Analisa o projeto para definir responsabilidades, permissoes e integracao com agentes existentes.
category: xforge-public
agent: code
---

# /criar-agent

## Objetivo

Criar um novo agente no sistema XForge Enterprise Development OS, definindo suas responsabilidades, permissoes, skills associadas e integracao com o ecossistema existente.

## Quando usar

- Usuario precisa de um novo agente especializado
- Ex: /criar-agent --nome fiscal-expert --dominio fiscal
- Ex: /criar-agent --nome blazor-ui --dominio frontend
- Ex: /criar-agent --nome api-gateway --dominio backend

## Roteamento interno

- /analisar-projeto - reconhece agentes e padroes existentes
- /criar-skill - se o agente precisar de skills novas

## Uso

`
/criar-agent --nome <nome-agente> --dominio <dominio>
/criar-agent --nome <nome-agente> --dominio <dominio> --tipo primario
/criar-agent --nome <nome-agente> --dominio <dominio> --tipo suporte
/criar-agent --nome <nome-agente> --dominio <dominio> --pai <agente-pai>
`

## Procedimento

### Fase 1: Analise do Ecossistema Existente

1. Ler .kilo/core/agent-router/CANONICAL-AGENTS.md para entender agentes canonicos
2. Ler .kilo/core/registries/expert-registry.json para agentes registrados
3. Identificar gaps: qual dominio nao tem agente dedicado?
4. Verificar se ja existe agente similar (evitar duplicacao)

### Fase 2: Definicao do Agente

1. Definir nome seguindo convencao: lowercase-with-hyphens, descritivo
2. Definir dominio de atuacao (fiscal, frontend, backend, devops, etc.)
3. Definir tipo: primario, suporte ou subagente
4. Definir permissores: read, edit (extensoes permitidas), bash
5. Definir skills associadas (existentes ou novas)
6. Definir agentes com quem colabora

### Fase 3: Criacao do Arquivo

1. Criar arquivo em .kilo/agents/<nome-agente>.md
2. Usar frontmatter padrao com name, description, color, mode, permission
3. Incluir secoes obrigatorias:
   - Quando Usar - condicoes de ativacao
   - Quando Nao Usar - limites de responsabilidade
   - Missao - proposito do agente
   - Responsabilidades - lista numerada de 5-10 itens
   - Saida Esperada - o que produzir
   - Nunca Fazer - limites e restricoes

### Fase 4: Registro

1. Adicionar entrada em .kilo/core/registries/expert-registry.json
2. Se for canonico, adicionar em CANONICAL-AGENTS.md
3. Se for subagente, atualizar arquivo do agente pai com referencia

### Fase 5: Validacao

1. Verificar sintaxe do frontmatter
2. Verificar se nome segue convencao
3. Verificar se permissoes sao coerentes
4. Executar doctor.ps1 para validar estrutura

## Saida esperada

- Arquivo .kilo/agents/<nome-agente>.md criado
- Registro atualizado em expert-registry.json
- Se canonico: CANONICAL-AGENTS.md atualizado
- Relatorio: nome, dominio, tipo, permissoes, skills

## Modelos Pequenos (64k context)

1. Ler arquivos de referencia em chunks
2. Criar agente em uma unica escrita
3. Atualizar registros incrementalmente

## Regras

- **Nunca duplique** - verificar se agente similar ja existe
- **Sempre defina permissoes restritivas** - principio do menor privilegio
- **Sempre registre** - todo agente deve estar no registry
- **Sempre documente** - Quando Usar, Quando Nao Usar, Nunca Fazer obrigatorios
