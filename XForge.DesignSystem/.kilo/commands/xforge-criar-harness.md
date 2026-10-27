---
name: criar-harness
description: Cria harness de testes - estrutura completa de testes automatizados para um modulo, feature ou sistema.
category: xforge-public
agent: code
---

# /criar-harness

## Objetivo

Criar um harness de testes completo no XForge, fornecendo a estrutura de testes automatizados para um modulo, feature ou sistema, incluindo testes unitarios, de integracao e end-to-end.

## Quando usar

- Criar estrutura de testes para um novo modulo
- Ex: /criar-harness --nome harness-modulo-fiscal --modulo fiscal
- Ex: /criar-harness --nome harness-api-clientes --modulo clientes
- Ex: /criar-harness --nome harness-auth --modulo autenticacao

## Roteamento interno

- /criar-spec - para especificar o que testar
- /criar-feature - para implementar o que sera testado
- /qualidade - para validar cobertura

## Uso

/criar-harness --nome <nome-harness> --modulo <modulo>
/criar-harness --nome <nome-harness> --modulo <modulo> --tipo <unitario|integracao|e2e|completo>
/criar-harness --nome <nome-harness> --modulo <modulo> --cobertura <percentual>

## Tipos de Teste

| Tipo | Descricao | Ferramenta |
|------|-----------|------------|
| unitario | Testa unidades isoladas | xUnit, NUnit |
| integracao | Testa integracao entre componentes | WebApplicationFactory |
| e2e | Testa fluxo completo | Playwright, Selenium |
| contrato | Testa contratos de API | Pact |
| performance | Testa carga e stress | k6, BenchmarkDotNet |

## Procedimento

### Fase 1: Analise do Modulo

1. Ler codigo fonte do modulo alvo
2. Identificar classes, metodos e endpoints para testar
3. Mapear dependencias (servicos, repositorios, APIs)
4. Identificar cenarios de teste: Happy path, Edge cases, Error handling, Validacoes, Seguranca

### Fase 2: Definicao da Estrutura

1. Definir tipos de teste necessarios
2. Definir cobertura minima (ex: 80%)
3. Definir fixtures e mocks necessarios
4. Definir dados de teste (seed data)
5. Definir ambiente de teste (banco, servicos)

### Fase 3: Criacao do Harness

1. Criar estrutura de diretorios:
   tests/Unit/<Modulo>/Services/, Handlers/, Validators/
   tests/Integration/<Modulo>/Controllers/, Repositories/, APIs/
   tests/E2E/<Modulo>/Flows/, Scenarios/
2. Criar arquivos base: TestBase.cs, DataFixture.cs, MockFactory.cs, CollectionDefinitions.cs
3. Criar testes unitarios para cada componente
4. Criar testes de integracao para fluxos criticos
5. Criar testes E2E para cenarios principais

### Fase 4: Configuracao

1. Criar/atualizar .xforge/tests/config/test-config.json
2. Configurar cobertura de codigo
3. Configurar relatorios de teste
4. Configurar CI/CD para executar harness

### Fase 5: Validacao

1. Executar testes: dotnet test
2. Verificar cobertura
3. Verificar se todos os cenarios estao cobertos
4. Corrigir testes que falham

## Saida esperada

- Estrutura de diretorios de testes criada
- Classes base (TestFixture, MockFactory, etc.)
- Testes unitarios para componentes principais
- Testes de integracao para fluxos criticos
- Configuracao de cobertura
- Relatorio: tipos de teste, cobertura, cenarios

## Modelos Pequenos (64k context)

1. Analisar modulo em chunks
2. Criar harness incrementalmente (uma categoria de teste por vez)
3. Escrever testes em batches (5-10 testes por escrita)

## Regras

- Sempre siga o padrao AAA - Arrange, Act, Assert
- Sempre use dados deterministicos - testes devem ser reprodutiveis
- Sempre isole dependencias - use mocks para servicos externos
- Sempre nomeie testes claramente - Metodo_Cenario_ResultadoEsperado
- Sempre cubra erros - teste o que pode dar errado
- Sempre configure cobertura minima - 80% para codigo de producao
