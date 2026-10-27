---
name: criar-template
description: Cria template generico baseado em analise profunda de uma pagina/arquivo existente. Analisa estrutura, componentes, padroes e gera template reutilizavel.
category: xforge-public
agent: code
---

# /criar-template

## Objetivo

Analisar uma pagina ou arquivo existente de forma profunda e exaustiva, extrair todos os padroes visuais, estruturais e comportamentais, e gerar um template generico reutilizavel.

## Quando usar

- Usuario quer criar um template baseado em uma pagina existente
- Ex: /criar-template local/nome-da-pagina.ext
- Ex: /criar-template src/Pages/Produtos/Index.razor --tipo crud
- Ex: /criar-template src/Components/Modals/FormCliente.razor --tipo modal

## Roteamento interno

- /analisar-projeto - reconhece stack e padroes do projeto
- /criar-feature - se o template precisar de feature associada

## Uso

`
/criar-template <caminho-pagina>
/criar-template <caminho-pagina> --tipo crud
/criar-template <caminho-pagina> --tipo modal
/criar-template <caminho-pagina> --tipo form
/criar-template <caminho-pagina> --tipo list
/criar-template <caminho-pagina> --tipo dashboard
/criar-template <caminho-pagina> --nome MeuTemplate
`

## Procedimento

### Fase 1: Reconhecimento do Projeto

1. Ler .xforge/project-dna/PROJECT-DNA.md para entender stack e convencoes
2. Ler .xforge/memory/index.json para contexto atual
3. Identificar: stack frontend, padroes de nomenclatura, localizacao de templates

### Fase 2: Analise Profunda da Pagina

1. **Ler o arquivo alvo completo** - cada linha, cada detalhe
2. Identificar componentes e subcomponentes:
   - Layout (grid, flex, sidebar, header, footer)
   - Tabelas e listas (colunas, ordenacao, paginacao, filtros)
   - Formularios (campos, validacoes, mascaras, obrigatoriedade)
   - Modais (tamanho, botoes, eventos de abrir/fechar)
   - Cards e widgets
   - Navegacao (breadcrumbs, tabs, menus)
   - Estados (loading, empty, error, success)
3. Extrair padroes de nomenclatura:
   - Nomes de variaveis, funcoes, classes
   - Convenções de CSS/Tailwind
   - Padrões de eventos (onClick, onSubmit, onChange)
4. Identificar dependencias:
   - Componentes importados
   - Servicos injetados
   - APIs chamadas
   - Estado gerenciado (local, global, server)
5. Mapear regras de negocio visiveis:
   - Validacoes
   - Formatacoes (data, moeda, CPF, CNPJ)
   - Permissoes (botões condicionais)
   - Fluxos (wizard, steps, redirect)

### Fase 3: Extracao do Template

1. **Generalizar** - substituir nomes especificos por placeholders
2. **Parametrizar** - identificar o que varia entre usos
3. **Preservar** - manter estrutura, layout, padroes de estilo
4. **Documentar** - criar guia de uso do template

### Fase 4: Geracao do Template

1. Criar arquivo do template em .xforge/scaffolding/templates/{NOME}/
2. Escrever o template com placeholders claros
3. Criar arquivo de configuracao 	emplate-config.json
4. Criar README do template com instrucoes de uso

### Fase 5: Validacao e Ajuste

1. **Comparar** template gerado com original
2. **Testar mentalmente** - se alguem usar o template, vai funcionar?
3. **Ajustar** se necessario
4. **Perguntar** ao usuario se o template atende ou precisa de ajustes

## Saida esperada

- Arquivo do template criado com placeholders
- Arquivo 	emplate-config.json com metadados
- README com instrucoes de uso
- Lista de placeholders e como preencher
- Relatorio da analise: componentes encontrados, padroes identificados

## Modelos Pequenos (64k context)

1. Ler a pagina alvo em chunks (500 linhas por vez)
2. Fazer analise incremental - uma secao por vez
3. Salvar analise parcial em .xforge/scaffolding/analysis-{NOME}.md
4. Gerar template em multiplas escritas (uma secao por vez)
5. Se contexto encher, salvar progresso e continuar

## Regras

- **Sempre analise a pagina real** - nao assuma, leia o codigo
- **Sempre generalize** - template deve servir para qualquer entidade
- **Sempre valide** - compare template com original
- **Sempre ajuste** - se nao ficou bom, refina ate ficar
- **Nunca copie dados reais** - substitua tudo por placeholders
- **Sempre use write_file** - escreva o template no disco, nao descreva no chat
