---
name: criar-sdd
description: Cria Software Design Document (SDD) - documento de design de software completo com arquitetura, decisoes e implementacao.
category: xforge-public
agent: code
---

# /criar-sdd

## Objetivo

Criar um Software Design Document (SDD) completo que documenta o design de software de um sistema, modulo ou feature, incluindo arquitetura, decisoes, alternativas e plano de implementacao.

## Quando usar

- Antes de implementar um sistema ou modulo complexo
- Ex: /criar-sdd --nome sdd-modulo-fiscal --escopo modulo
- Ex: /criar-sdd --nome sdd-api-pagamentos --escopo sistema
- Ex: /criar-sdd --nome sdd-auth-service --escopo microservico

## Roteamento interno

- /criar-spec - para especificacao tecnica detalhada
- /criar-feature - para implementar baseado no SDD
- /analisar-projeto - para entender contexto arquitetural

## Uso

/criar-sdd --nome <nome-sdd> --escopo <modulo|sistema|microservico|feature>
/criar-sdd --nome <nome-sdd> --escopo <escopo> --template <template>

## Procedimento

### Fase 1: Contexto Arquitetural

1. Ler .xforge/project-dna/PROJECT-DNA.md
2. Ler .xforge/knowledge/arquitetura/ para decisoes existentes
3. Ler .xforge/decisions/ADR-INDEX.md para ADRs relacionadas
4. Identificar padroes arquiteturais do projeto
5. Mapear sistemas e modulos relacionados

### Fase 2: Levantamento de Design

1. Definir escopo do SDD (o que esta dentro e fora)
2. Identificar componentes principais
3. Definir interfaces entre componentes
4. Mapear fluxo de dados
5. Identificar decisoes arquiteturais necessarias
6. Levantar alternativas para cada decisao

### Fase 3: Criacao do SDD

1. Criar arquivo em .xforge/knowledge/sdd/<nome-sdd>.md
2. Estrutura obrigatoria:
   - Visao Geral - resumo executivo
   - Contexto - problema sendo resolvido
   - Escopo - limites do documento
   - Nao-Objetivos - o que explicitamente nao faz parte
   - Arquitetura - visao geral da solucao
   - Componentes - detalhamento de cada componente
   - Interfaces - contratos entre componentes
   - Fluxo de Dados - como dados transitam
   - Decisoes Arquiteturais - ADRs inline com alternativas
   - Design Detalhado - classes, metodos, APIs
   - Banco de Dados - esquema, tabelas, indices
   - Seguranca - autenticacao, autorizacao, dados sensiveis
   - Performance - requisitos e estrategias
   - Testes - estrategia de testes
   - Implantacao - como fazer deploy
   - Riscos - identificacao e mitigacao
   - Referencias - links, docs, ADRs

### Fase 4: Decisoes Arquiteturais

1. Para cada decisao significativa, criar ADR inline com: Contexto, Opcoes, Decisao, Consequencias
2. Registrar ADRs no ADR-INDEX.md

### Fase 5: Revisao

1. Revisar com agente de arquitetura
2. Verificar consistencia com padroes existentes
3. Verificar viabilidade tecnica
4. Aprovar ou solicitar ajustes

## Saida esperada

- Arquivo .xforge/knowledge/sdd/<nome-sdd>.md criado
- ADRs registradas no indice
- Diagramas incluidos (ASCII ou referencias)
- Status: draft -> review -> approved

## Modelos Pequenos (64k context)

1. Ler contexto em chunks
2. Criar SDD incrementalmente (2-3 secoes por escrita)
3. Salvar progresso e continuar

## Regras

- Sempre inclua contexto - por que este SDD existe
- Sempre documente alternativas - decisoes sem alternativas sao arbitrarias
- Sempre defina escopo e nao-objetivos - evitar scope creep
- Sempre inclua diagramas - uma imagem vale mais que mil palavras
- Sempre registre ADRs - decisoes devem ser rastreaveis
- Sempre revise - SDD sem revisao e opiniao, nao design
