---
name: criar-rule
description: Cria nova regra de negocio, operacional ou tecnica no XForge. Regras sao carregadas pelo kilo.jsonc e aplicadas globalmente.
category: xforge-public
agent: code
---

# /criar-rule

## Objetivo

Criar uma nova regra no XForge Enterprise Development OS. Regras sao carregadas automaticamente pelo kilo.jsonc (instrucoes) e aplicadas globalmente a todos os agentes.

## Quando usar

- Nova regra de negocio que deve ser seguida por todos os agentes
- Ex: /criar-rule --nome regra-ibge --tipo negocio --dominio fiscal
- Ex: /criar-rule --nome regra-api-versioning --tipo tecnica --dominio backend
- Ex: /criar-rule --nome regra-privacy-by-design --tipo seguranca --dominio geral

## Roteamento interno

- /analisar-projeto - identifica regras existentes
- /criar-skill - se a skill precisa reforcar a regra

## Uso

/criar-rule --nome <nome-regra> --tipo <negocio|tecnica|seguranca|qualidade>
/criar-rule --nome <nome-regra> --tipo <tipo> --dominio <dominio>
/criar-rule --nome <nome-regra> --tipo <tipo> --prioridade <alta|media|baixa>

## Tipos de Regra

| Tipo | Descricao | Exemplo |
|------|-----------|---------|
| negocio | Regra de negocio do dominio | Calculo ICMS por UF |
| tecnica | Padrao tecnico obrigatorio | Versionamento de API |
| seguranca | Regra de seguranca/privacidade | LGPD, RBAC |
| qualidade | Padrao de qualidade de codigo | Cobertura de testes |
| operacional | Regra de operacao do XForge | Uso de memoria |

## Procedimento

### Fase 1: Analise de Regras Existentes

1. Ler .kilo/rules/00-xforge-rule-index.md para indice de regras
2. Listar arquivos em .kilo/rules/ para evitar duplicacao
3. Identificar familia da regra (golden, memory, security, etc.)
4. Definir numero de ordenacao (prefixo NN-)

### Fase 2: Definicao da Regra

1. Definir nome: NN-<nome-regra>.md (numero para ordenacao)
2. Definir tipo e dominio
3. Definir prioridade (alta, media, baixa)
4. Escopo: global (todos os agentes) ou especifico (dominio)

### Fase 3: Criacao do Arquivo

1. Criar arquivo em .kilo/rules/NN-<nome-regra>.md
2. Estrutura obrigatoria: Titulo, Regra, Obrigatorio, Proibido, Aplicacao, Exemplos, Consequencias
3. Usar formato claro com checkboxes e listas
4. Incluir exemplos de codigo quando aplicavel

### Fase 4: Integracao

1. Regra e carregada automaticamente se estiver em .kilo/rules/
2. Se for regra fundamental, adicionar ao 00-xforge-rule-index.md
3. Se for regra de golden rules, adicionar a 01-xforge-golden-rules.md

### Fase 5: Validacao

1. Verificar formato e estrutura
2. Verificar se regra e clara e acionavel
3. Verificar se nao conflita com regras existentes
4. Executar doctor.ps1

## Saida esperada

- Arquivo .kilo/rules/NN-<nome-regra>.md criado
- Indice atualizado (se aplicavel)
- Relatorio: nome, tipo, dominio, prioridade, escopo

## Modelos Pequenos (64k context)

1. Ler indice de regras primeiro
2. Criar regra em uma unica escrita
3. Atualizar indice separadamente

## Regras

- Sempre verifique duplicacao - nao crie regra que ja existe
- Sempre seja especifico - regras vagas nao sao uteis
- Sempre inclua exemplos - mostre como aplicar
- Sempre defina proibidos - o que nao fazer e tao importante quanto o que fazer
- Sempre use numero de prefixo - para ordenacao correta
