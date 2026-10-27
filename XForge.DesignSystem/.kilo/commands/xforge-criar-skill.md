---
name: criar-skill
description: Cria nova skill no XForge. Analisa gaps de capacidade e gera skill com procedimento, inputs e outputs claros.
category: xforge-public
agent: code
---

# /criar-skill

## Objetivo

Criar uma nova skill no XForge Enterprise Development OS, preenchendo gaps de capacidade identificados na analise do ecossistema existente.

## Quando usar

- Existe uma capacidade que nao tem skill dedicada
- Ex: /criar-skill --nome relatorio-fiscal --dominio fiscal
- Ex: /criar-skill --nome deploy-kubernetes --dominio devops
- Ex: /criar-skill --nome teste-integracao --dominio qualidade

## Roteamento interno

- /analisar-projeto - identifica gaps de skills
- /criar-agent - se precisar de agente para usar a skill

## Uso

/criar-skill --nome <nome-skill> --dominio <dominio>
/criar-skill --nome <nome-skill> --dominio <dominio> --categoria <cat>

## Procedimento

### Fase 1: Analise de Gaps

1. Ler .kilo/core/skill-router/CANONICAL-SKILLS.md para skills existentes
2. Ler .kilo/core/registries/skill-registry.json para registro completo
3. Identificar: qual capacidade esta faltando?
4. Verificar se skill similar ja existe (evitar duplicacao)
5. Definir categoria: DevOps, Frontend, Backend, Seguranca, Qualidade, etc.

### Fase 2: Definicao da Skill

1. Definir nome seguindo convencao: lowercase-with-hyphens
2. Definir descricao clara (quando usar)
3. Definir categoria xforge
4. Definir versao inicial (1.0.0)
5. Mapear agentes que podem usar esta skill

### Fase 3: Criacao do Arquivo

1. Criar diretorio .kilo/skills/<nome-skill>/
2. Criar arquivo SKILL.md dentro do diretorio
3. Usar template base de .kilo/skills/_template/SKILL.md
4. Preencher secoes obrigatorias: Trigger, Do Not Use, Inputs, Procedure, Output
5. Incluir secao Modelos Pequenos (64k context) com estrategia de chunking

### Fase 4: Registro

1. Adicionar entrada em skill-registry.json
2. Se for skill canonica, adicionar em CANONICAL-SKILLS.md
3. Atualizar indice de skills se aplicavel

### Fase 5: Validacao

1. Verificar frontmatter (name, description, metadata)
2. Verificar se todas as secoes estao preenchidas
3. Verificar se procedimento e acionavel (passos claros)
4. Executar doctor.ps1

## Saida esperada

- Diretorio .kilo/skills/<nome-skill>/ criado
- Arquivo SKILL.md com conteudo completo
- Registro atualizado
- Relatorio: nome, categoria, trigger, agentes associados

## Modelos Pequenos (64k context)

1. Ler template e referencia em chunks
2. Criar skill em uma unica escrita
3. Atualizar registro separadamente

## Regras

- Sempre analise gaps primeiro - nao crie skill duplicada
- Sempre use o template - siga estrutura padrao
- Sempre inclua procedimento acionavel - passos claros e executaveis
- Sempre registre - toda skill deve estar no registry
- Sempre inclua secao 64k - preparar para contexto pequeno
