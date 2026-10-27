# ADR-0001: Terminal como Template Copiável

## Status
accepted

## Data
2026-06-08

## Contexto
O XForge Development OS acumulou múltiplas pastas históricas (.xforge/reports, .xforge/backlog, .xforge/testing) que confundiam o que era template copiável e o que era histórico de desenvolvimento. Era necessário definir claramente o escopo do produto "terminal".

## Decisão
Focar em `terminal/` como **template copiável único**. Todas as pastas históricas foram removidas. O terminal contém apenas o que é necessário para um novo projeto começar.

## Alternativas Consideradas

### Alternativa 1: Manter tudo em um repositório
**Prós:** Histórico completo preservado
**Contras:** Confusão sobre o que é template, templates inchados, releases grandes
**Por que não foi escolhida:** Dificulta o uso como template

### Alternativa 2: Separar em múltiplos repositórios
**Prós:** Cada produto independente
**Contras:** Complexidade de manutenção, sincronização entre repos
**Por que não foi escolhida:** Overhead desnecessário para o estágio atual

## Consequências

### Positivas
- Template limpo e focado
- Clareza sobre o que copiar para novos projetos
- Releases enxutas
- Documentação como parte do template

### Negativas
- Histórico de desenvolvimento não está no template (mas está no git)
- Necessidade de disciplina para manter separação .kilo/.xforge

## Implementação
- Pastas históricas removidas de `.xforge/`
- Estrutura documentada em `README.md` e `.xforge/docs/INSTALL.md`
- Política de conhecimento criada para manter organização

## Fontes
- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Trunk Based Development](https://trunkbaseddevelopment.com/)
