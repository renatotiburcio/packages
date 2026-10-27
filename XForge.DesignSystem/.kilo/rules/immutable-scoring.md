# Immutable Scoring System — XForge Engineer

## Visão Geral

Sistema de pontuação que o agente **NÃO pode modificar ou manipular**. A pontuação é calculada por um processo externo (validador) que o agente não controla. A única exceção é quando o humano solicita alteração explicitamente.

## Regra Fundamental

```
╔══════════════════════════════════════════════════════════════╗
║  O AGENTE NUNCA PODE MODIFICAR O SISTEMA DE PONTUAÇÃO.     ║
║'  A pontuação é calculada por um processo separado.          ║'
║'  A única exceção é solicitação humana explícita.            ║'
╚══════════════════════════════════════════════════════════════╝
```

## Como Funciona

### 1. Métrica Única: XForge Score (xfs)

O **XForge Score** é uma métrica composta de 5 dimensões, cada uma com peso fixo:

```
xfs = (correctness × 0.30) + (performance × 0.20) + (quality × 0.20) + (simplicity × 0.15) + (prevention × 0.15)
```

| Dimensão | Peso | O que mede | Como mede |
|----------|------|------------|-----------|
| **Correctness** | 30% | O código funciona? Testes passam? | `dotnet test` exit code |
| **Performance** | 20% | É rápido? Sem N+1, sem memory leaks? | Análise estática + métricas |
| **Quality** | 20% | Código limpo? Padrões seguidos? | `dotnet format` + lint |
| **Simplicity** | 15% | Mudança mínima? Sem over-engineering? | Análise de diff |
| **Prevention** | 15% | Erros prevenidos? Padrões aprendidos? | Error graph matches |

### 2. Validação Independente

O scoring é executado por um **validador separado** que o agente não controla:

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│   Agente XForge │────▶│  Scoring Script  │────▶│  results.tsv    │
│   (modifica     │     │  (calcula xfs)   │     │  (log imutável) │
│    código)      │     │  (agente NÃO     │     │                 │
│                 │     │   controla)      │     │                 │
└─────────────────┘     └──────────────────┘     └─────────────────┘
```

### 3. Proteções

| Proteção | Descrição |
|----------|-----------|
| **Script read-only** | `score.sh` tem permissão 555 (execute-only) |
| **Resultados append-only** | `results.tsv` só permite append, nunca overwrite |
| **Hash de integridade** | Cada linha tem hash SHA256 do conteúdo |
| **Validador externo** | Scoring roda em processo separado |
| **Auditoria** | Qualquer tentativa de modificação é logada |

## Métricas Detalhadas

### Correctness (30%)

```python
# Calculado automaticamente
correctness = (
    tests_passed / total_tests * 0.5 +        # 50%: testes passam
    build_success * 0.3 +                       # 30%: build limpo
    (1 - runtime_errors) * 0.2                  # 20%: sem erros de runtime
)
```

### Performance (20%)

```python
# Calculado por análise estática
performance = (
    (1 - n_plus_one_queries) * 0.3 +           # 30%: sem N+1
    (1 - sync_over_async) * 0.3 +               # 30%: sem sync-over-async
    (1 - memory_leaks) * 0.2 +                  # 20%: sem memory leaks
    response_time_score * 0.2                    # 20%: tempo de resposta
)
```

### Quality (20%)

```python
# Calculado por dotnet format + lint
quality = (
    format_compliance * 0.4 +                    # 40%: formatação
    (1 - warnings) * 0.3 +                       # 30%: sem warnings
    naming_convention * 0.15 +                   # 15%: padrões de nomes
    documentation * 0.15                         # 15%: documentação
)
```

### Simplicity (15%)

```python
# Calculado por análise de diff
simplicity = (
    lines_added_ratio * 0.4 +                    # 40%: poucas linhas adicionadas
    (1 - files_changed_ratio) * 0.3 +            # 30%: poucos arquivos alterados
    (1 - new_dependencies) * 0.3                 # 30%: sem novas dependências
)
```

### Prevention (15%)

```python
# Calculado por error graph
prevention = (
    errors_prevented * 0.5 +                     # 50%: erros prevenidos
    patterns_applied * 0.3 +                     # 30%: padrões aplicados
    knowledge_gaps_filled * 0.2                  # 20%: gaps preenchidos
)
```

## Formato do results.tsv

```
commit	xfs	correctness	performance	quality	simplicity	prevention	status	description	timestamp
a1b2c3d	0.875	0.92	0.85	0.90	0.88	0.82	keep	Adicionado FluentValidation	2026-06-11T10:30:00Z
b2c3d4e	0.750	0.80	0.70	0.85	0.60	0.78	discard	Mudança radical de arquitetura	2026-06-11T10:35:00Z
c3d4e5f	0.000	0.00	0.00	0.00	0.00	0.00	crash	Build falhou - null reference	2026-06-11T10:40:00Z
```

## Script de Validação

O script `score.ps1` é o único que calcula a pontuação:

```powershell
# score.ps1 — Execute-only, agente não pode modificar
# Calcula XForge Score para a última mudança
# Uso: .\score.ps1
# Saída: linha para results.tsv
```

## Regras para o Agente

1. **NUNCA** tentar modificar `score.ps1`
2. **NUNCA** tentar modificar `results.tsv` diretamente
3. **NUNCA** tentar manipular a pontuação de alguma forma
4. **SEMPRE** aguardar o resultado do scoring antes de decidir keep/discard
5. **SEMPRE** respeitar o resultado do scoring (mesmo que discorde)

## Integração com o Loop

```
1. Agente faz mudança no código
2. Agente faz commit
3. Scoring script roda (separado)
4. Scoring calcula xfs
5. Scoring registra em results.tsv
6. Agente lê resultado
7. Agente decide: keep (avança) ou discard (reverte)
8. Repete
```

## Exceções

A única forma de modificar o sistema de pontuação é:

1. **Solicitação humana explícita**: "Altere o peso de correctness para 40%"
2. **Auditoria humana**: Humano revisa e ajusta periodicamente
3. **Evolução controlada**: Mudanças só passam após review humano
