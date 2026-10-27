# Análise REA — TestHelper de validação (XForge.Validation.TestHelper)

Este pacote fecha um item que a análise REA do `XForge.Validation` já havia mapeado: o
**FluentValidation.TestHelper** (`FluentValidation` 12.2.0, analisado em
[`../XForge.Validation/docs/ANALISE-PACOTES.md`](../XForge.Validation/docs/ANALISE-PACOTES.md)).
Não houve novo alvo binário: a análise prévia documentou a API do TestHelper
(`TestValidationResult`, `ShouldHaveValidationError…`, assertions por expressão lambda) e as
diferenças de modelo (o XForge usa `ValidationResult.ErrorsFor(membro)` com códigos padronizados
e severidade, não `ValidationFailure` único por membro).

## Achados herdados da análise do FluentValidation 12.2.0

| Aspecto | FluentValidation.TestHelper | Decisão no XForge.TestHelper |
|---|---|---|
| Ponto de entrada | `validator.TestValidationResult(instance)` → resultado | Idêntico (`TestValidationResult` / `…Async`, com overload de RuleSet) |
| Assertions por membro | Expressão lambda `ShouldHaveValidationErrorFor(x => x.Prop)` | Idêntico **+ overload por string** (nossa API é por nome) |
| Comparação de mensagem | Exata por padrão | Idêntica (case-sensitive, ordinal) |
| Códigos de erro | Não faziam parte do modelo | `ShouldHaveValidationErrorWithCode` (recurso exclusivo XForge) |
| Severidade | Não existia | `ShouldNotHaveValidationErrorsWithSeverity` |
| Dependência | xUnit (exceções de assert) | Idêntica (`XunitException`) |
| Pacote separado | `FluentValidation.TestHelper` | Idêntico (pacote irmão) |

## Diferenças de design (herdadas do modelo XForge)

1. **Falhas por membro**: o resultado do XForge agrega `ValidationFailure` com
   `PropertyName`/`ErrorMessage`/`ErrorCode`/`Severity` — as assertions consultam
   `ErrorsFor(membro)` (case-insensitive), coerente com `ValidationResult.GetError`.
2. **Semântica de "válido"**: o XForge considera falhas de aviso (`Severity.Warning`) como
   não-bloqueantes — daí o assertion específico por severidade em vez de um
   "ShouldNotHaveValidationError" genérico.
3. **Erro de programação detectado cedo**: expressões que não são acesso simples a membro
   (`x => x.Idade + 1`) lançam `ArgumentException` — evita falso-positivo silencioso nos testes.

## Limitações conhecidas (v0.1)

- Sem assertion de "quantidade exata" de falhas por membro (`ShouldHaveValidationErrorCount`?) — roadmap.
- Sem suporte a coleções (`RuleForEach` produz nomes como `Itens[0].Produto`): a comparação por
  string cobre o caso, mas sem helper específico de índice.
