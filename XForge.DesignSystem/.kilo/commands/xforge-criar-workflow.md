---
name: criar-workflow
description: Cria novo workflow operacional no XForge. Workflows definem fluxos de trabalho multi-etapa com validacao e governanca.
category: xforge-public
agent: code
---

# /criar-workflow

## Objetivo

Criar um novo workflow operacional no XForge Enterprise Development OS, definindo fluxos de trabalho multi-etapa com pontos de validacao, governanca e saida rastreavel.

## Quando usar

- Definir fluxo de trabalho para operacoes complexas
- Ex: /criar-workflow --nome deploy-producao --etapas 5
- Ex: /criar-workflow --nome code-review --etapas 3
- Ex: /criar-workflow --nome incident-response --etapas 7

## Roteamento interno

- /criar-agent - se precisar de agente para executar o workflow
- /criar-skill - se o workflow precisa de skill dedicada

## Uso

/criar-workflow --nome <nome-workflow> --etapas <n>
/criar-workflow --nome <nome-workflow> --etapas <n> --agente <agente>
/criar-workflow --nome <nome-workflow> --etapas <n> --gated

## Procedimento

### Fase 1: Analise de Workflows Existentes

1. Ler diretorio .kilo/commands/workflows/ para workflows existentes
2. Verificar se workflow similar ja existe
3. Identificar padroes de fluxo reutilizaveis
4. Definir complexidade: simples (1-3 etapas), media (4-6), complexa (7+)

### Fase 2: Definicao do Fluxo

1. Definir nome: lowercase-with-hyphens, descritivo
2. Definir numero de etapas
3. Para cada etapa, definir: Nome, Responsavel, Entrada, Saida, Validacao, Proxima etapa
4. Definir pontos de decisao (if/else no fluxo)
5. Definir tratamento de erros por etapa

### Fase 3: Criacao do Arquivo

1. Criar arquivo em .kilo/commands/workflows/<nome-workflow>.md
2. Frontmatter com name, description, agent, type: workflow, version: 1.0.0
3. Estrutura obrigatoria:
   - Visao Geral - resumo do workflow
   - Quando Usar - condicoes de ativacao
   - Quando Nao Usar - limites
   - Diagrama de Fluxo - ASCII art do fluxo
   - Etapa 1..N - detalhamento de cada etapa
   - Tratamento de Erros - tabela de erros e acoes
   - Metricas - como medir sucesso
   - Integracoes - workflows relacionados
   - Procedimento Minimo - 8 passos padrao
   - Saida Esperada - resultado final

### Fase 4: Registro

1. Adicionar ao indice de workflows
2. Referenciar em comandos que podem usar este workflow

### Fase 5: Validacao

1. Verificar se fluxo e completo (todas as etapas)
2. Verificar se tratamento de erros existe
3. Verificar se saida e rastreavel
4. Executar doctor.ps1

## Saida esperada

- Arquivo .kilo/commands/workflows/<nome-workflow>.md criado
- Diagrama de fluxo incluido
- Todas as etapas documentadas
- Tratamento de erros definido

## Modelos Pequenos (64k context)

1. Ler workflows de referencia em chunks
2. Criar workflow incrementalmente (2-3 etapas por escrita)
3. Completar tratamento de erros separadamente

## Regras

- Sempre inclua diagrama de fluxo - visualizar o processo
- Sempre trate erros - cada etapa deve ter tratamento de falha
- Sempre defina gates - pontos de validacao entre etapas
- Sempre use procedimento minimo - 8 passos padrao do XForge
- Sempre versione - use semantic versioning
