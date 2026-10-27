---
description: Analisa projeto, produto, UX, segurança, arquitetura, conhecimento, maturidade e gaps.
agent: code
---

# /analisar

## Categoria

orquestração

## Objetivo

Análise profunda e multi-dimensional de um projeto ou componente.

## Sub-comandos

| Sub-comando | Foco |
|-------------|------|
| `/analisar projeto` | Estrutura, arquitetura, maturidade |
| `/analisar segurança` | Vulnerabilidades, LGPD, OWASP |
| `/analisar ux` | Usabilidade, acessibilidade, design |
| `/analisar arquitetura` | Padrões, acoplamento, SOLID |
| `/analisar conhecimento` | Knowledge graph, gaps, confiança |

## Procedimento

1. **Identificar escopo**: O que está sendo analisado? (projeto, módulo, feature)
2. **Recuperar memória**: Buscar decisões, incidentes, contexto anterior
3. **Selecionar experts**: Escolher skills relevantes (máx 3)
4. **Executar análise**:
   - Estrutura de diretórios
   - Código: padrões, complexidade, testes
   - Banco: schema, migrations, queries
   - API: contratos, segurança
   - Docs: completude, atualidade
5. **Classificar achados**: Crítico / Alto / Médio / Baixo
6. **Gerar saída**:
   - Score de maturidade (0-100)
   - Lista de gaps priorizada
   - Recomendações acionáveis
   - Backlog sugerido
7. **Atualizar memória**: Salvar conclusões

## Saída Esperada

```json
{
  "scope": "projeto ou módulo",
  "maturityScore": 72,
  "dimensions": {
    "architecture": 80,
    "tests": 65,
    "security": 70,
    "docs": 60,
    "dx": 75
  },
  "gaps": [
    {"severity": "high", "area": "tests", "description": "Coverage abaixo de 85%"},
    {"severity": "medium", "area": "docs", "description": "API sem XML docs"}
  ],
  "recommendations": [
    "Aumentar cobertura de testes no módulo X",
    "Adicionar XML docs em endpoints públicos"
  ]
}
```

## Exemplos

```
/analisar
/analisar projeto atual
/analisar segurança da API de autenticação
/analisar arquitetura do módulo de pagamentos
```
