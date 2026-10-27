---
name: criar-spec
description: Cria especificacao tecnica detalhada (spec) para uma feature, componente ou sistema. Base para implementacao.
category: xforge-public
agent: code
---

# /criar-spec

## Objetivo

Criar uma especificacao tecnica detalhada (spec) que serve como base para implementacao de uma feature, componente ou sistema no XForge.

## Quando usar

- Antes de implementar uma feature complexa
- Ex: /criar-spec --nome spec-auth-jwt --feature autenticacao
- Ex: /criar-spec --nome spec-relatorio-pdf --feature relatorios
- Ex: /criar-spec --nome spec-api-pagamentos --feature integracao

## Roteamento interno

- /criar-feature - para implementar baseado na spec
- /criar-sdd - para documento de design mais amplo
- /analisar-projeto - para entender contexto

## Uso

/criar-spec --nome <nome-spec> --feature <feature>
/criar-spec --nome <nome-spec> --feature <feature> --tipo <feature|componente|sistema|api>
/criar-spec --nome <nome-spec> --feature <feature> --prioridade <alta|media|baixa>

## Procedimento

### Fase 1: Contexto

1. Ler .xforge/project-dna/PROJECT-DNA.md para contexto do projeto
2. Ler .xforge/knowledge/padroes/ para padroes arquiteturais
3. Ler features relacionadas para evitar conflito
4. Identificar stakeholders e dependencias

### Fase 2: Levantamento

1. Definir escopo da spec (o que esta dentro e fora)
2. Identificar requisitos funcionais (RF001, RF002, ...)
3. Identificar requisitos nao-funcionais (RNF001, RNF002, ...)
4. Mapear dependencias externas (APIs, bibliotecas, servicos)
5. Identificar riscos e restricoes

### Fase 3: Especificacao

1. Criar arquivo em .xforge/knowledge/specs/<nome-spec>.md
2. Estrutura obrigatoria:
   - Visao Geral - resumo do que e a spec
   - Escopo - limites claros
   - Requisitos Funcionais - lista numerada com criterios de aceite
   - Requisitos Nao-Funcionais - performance, seguranca, usabilidade
   - Design Tecnico - diagramas, fluxos, decisoes
   - API/Interface - contratos, DTOs, endpoints
   - Banco de DDL - tabelas, indices, migracoes
   - Dependencias - servicos externos, pacotes
   - Riscos - identificacao e mitigacao
   - Criterios de Aceite - como validar que esta completo
   - Fora do Escopo - o que explicitamente nao faz parte

### Fase 4: Revisao

1. Verificar completude (todos os requisitos cobertos?)
2. Verificar consistencia (conflito com specs existentes?)
3. Verificar viabilidade (e possivel implementar?)
4. Revisar com agente de qualidade

### Fase 5: Aprovacao

1. Marcar status: draft -> review -> approved
2. Se aprovada, criar feature branch
3. Vincular a backlog/roadmap

## Saida esperada

- Arquivo .xforge/knowledge/specs/<nome-spec>.md criado
- Requisitos numerados e rastreaveis
- Criterios de aceite definidos
- Status inicial: draft

## Modelos Pequenos (64k context)

1. Ler contexto em chunks
2. Criar spec incrementalmente (uma secao por vez)
3. Salvar progresso em arquivo parcial

## Regras

- Sempre defina escopo claro - o que esta dentro e fora
- Sempre numere requisitos - rastreabilidade
- Sempre inclua criterios de aceite - como validar
- Sempre documente fora do escopo - evitar scope creep
- Sempre revise com qualidade - spec sem revisao e especulacao
