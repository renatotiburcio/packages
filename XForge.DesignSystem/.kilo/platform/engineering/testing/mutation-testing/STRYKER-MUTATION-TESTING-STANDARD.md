# Stryker.NET Mutation Testing Standard

## Objetivo
Garantir que os testes unitários realmente validem comportamento, e não apenas executem código.

## Por que coverage não basta
Um teste pode executar o método e gerar cobertura, mas não validar resultado.

```csharp
[Fact]
public void DeveExecutarSemErro()
{
    Somar(10, 5);
}
```

## Como o Stryker funciona
O Stryker altera temporariamente o código e roda os testes.

Original:

```csharp
return a + b;
```

Mutação:

```csharp
return a - b;
```

Se o teste falha, o mutante foi morto. Se passa, o mutante sobreviveu.

## Fórmula
```text
Stryker Score = Mutantes Mortos / Mutantes Totais
```

## Interpretação XForge

| Score | Qualidade |
|---:|---|
| < 50% | Muito ruim |
| 50-70% | Fraco |
| 70-80% | Aceitável |
| 80-90% | Bom |
| 90-95% | Excelente |
| > 95% | Nível Enterprise |

## Regra de Ouro XForge

- Coverage mínimo: 85%
- Stryker Score mínimo: 80%
- Meta recomendada: 90%+
- Componentes críticos: 95%+

Componentes críticos: segurança, permissões, financeiro, fiscal, contábil, folha, pagamentos, Open Finance, cálculo tributário e assinatura digital.

## Quality Gates
- Build = 0 erros
- Warnings = 0
- Testes = 100% aprovados
- Coverage >= 85%
- Stryker Score >= 80%
- Componentes críticos >= 95%

## Instalação
```bash
dotnet tool install -g dotnet-stryker
```

## Execução
```bash
dotnet stryker
dotnet stryker --reporter html
```

## Configuração recomendada
```json
{
  "stryker-config": {
    "reporters": ["progress", "html", "cleartext"],
    "thresholds": {
      "high": 90,
      "low": 80,
      "break": 80
    }
  }
}
```

## Componentes críticos
```json
{
  "stryker-config": {
    "reporters": ["progress", "html", "cleartext"],
    "thresholds": {
      "high": 95,
      "low": 95,
      "break": 95
    }
  }
}
```
