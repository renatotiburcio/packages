---
name: criar-subagent
description: Cria subagente vinculado a um agente pai no XForge. Especialista em tarefa especifica dentro de um dominio maior.
category: xforge-public
agent: code
---

# /criar-subagent

## Objetivo

Criar um subagente especializado vinculado a um agente pai no XForge, delegando responsabilidades especificas e mantendo a hierarquia de execucao.

## Quando usar

- Agente pai precisa de ajuda especializada em sub-tarefas
- Ex: /criar-subagent --pai chief-architect --nome validador-seguranca
- Ex: /criar-subagent --pai frontend-ux --nome componente-blazor
- Ex: /criar-subagent --pai data-engineer --nome otimizador-queries

## Roteamento interno

- /criar-agent - se precisar criar agente independente
- /criar-skill - se o subagente precisar de skill dedicada

## Uso

/criar-subagent --pai <agente-pai> --nome <nome-subagente>
/criar-subagent --pai <agente-pai> --nome <nome-subagente> --especialidade <area>

## Procedimento

### Fase 1: Analise do Agente Pai

1. Ler arquivo do agente pai em .kilo/agents/<agente-pai>.md
2. Identificar responsabilidades que podem ser delegadas
3. Verificar subagentes ja existentes (evitar duplicacao)
4. Definir escopo do subagente

### Fase 2: Definicao do Subagente

1. Definir nome seguindo convencao: lowercase-with-hyphens
2. Definir especialidade especifica
3. Definir permissoes (mais restritas que o pai)
4. Definir interface com o pai (quando chamar, entrada, saida)

### Fase 3: Criacao do Arquivo

1. Criar arquivo em .kilo/agents/<nome-subagente>.md
2. Frontmatter com mode: subagent e referencia ao pai
3. Incluir secoes: Agente Pai, Especialidade, Quando o Pai Deve Chamar, Entrada Esperada, Saida Esperada, Responsabilidades, Nunca Fazer

### Fase 4: Atualizacao do Agente Pai

1. Adicionar referencia ao subagente no arquivo do pai
2. Incluir na secao de responsabilidades: delegar ao subagente quando aplicavel
3. Atualizar roteamento interno

### Fase 5: Registro

1. Adicionar entrada em expert-registry.json com tipo subagent
2. Referenciar agente pai no registro

## Saida esperada

- Arquivo .kilo/agents/<nome-subagente>.md criado
- Agente pai atualizado com referencia
- Registro atualizado
- Relatorio: nome, pai, especialidade, permissoes

## Modelos Pequenos (64k context)

1. Ler agente pai primeiro
2. Criar subagente em seguida
3. Atualizar pai por ultimo

## Regras

- Sempre vincule ao pai - subagente sem pai nao existe
- Sempre restrinja permissoes - subagente tem escopo menor
- Sempre documente a interface - entrada e saida claras
- Nunca duplique responsabilidades - cada subagente tem funcao unica
