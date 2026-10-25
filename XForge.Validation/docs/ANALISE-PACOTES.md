# Análise REA — Consolidação dos pacotes de validação (XForge.Validation)

> Análise estática (shipped-artifact) via MCP **REA** (`inspect_managed_members`) dos pacotes NuGet
> baixados de `api.nuget.org`. Metadados ECMA-335 completos; comportamento de runtime é inferência rotulada.

## 1. Pacotes analisados

| Pacote | Versão | DLL | Tipos | Métodos | Evidência (parent ev_) |
|---|---|---|---:|---:|---|
| FluentValidation (JeremySkinner) | 12.2.0 | `lib/net8.0/FluentValidation.dll` | 362 | 1467 | `86c93820…88f` |
| Ardalis.GuardClauses | 5.0.0 | `lib/net8.0/Ardalis.GuardClauses.dll` | 110 | 267 | `173c3be9…2be8` |
| Validot (bartoszlenar) | 2.6.0 | `lib/netstandard2.0/Validot.dll` | 526 | 2016 | `31a418ba…e32f` |
| Valit | 2.0.0 | `lib/netstandard2.0/Valit.dll` | 458 | 1469 | `1956b21d…cb8e1` |
| System.ComponentModel.Annotations | 5.0.0 | `lib/netstandard2.0/…Annotations.dll` | 71 | 428 | `f521981c…2058` |

## 2. Superfície observada por biblioteca

### 2.1 FluentValidation 12.2.0 — a referência (namespace `FluentValidation`)
- **Núcleo** (evidência de métodos): `AbstractValidator<T>` com `RuleFor<>`, `RuleForEach<>`, `RuleSet`,
  `When/Unless(+Async)`, `Include/Include<TOther>`, `Validate/ValidateAsync` (com `ValidationContext` +
  selector), `PreValidate`, `RaiseValidationException`, descriptor, GetEnumerator (itera as rules).
- **Extensões canônicas** (`DefaultValidatorExtensions`, 60+ métodos): `NotNull`, `Null`, `NotEmpty`, `Empty`,
  `Length`/`MaximumLength`/`MinimumLength`, `Matches`, `EmailAddress`, `NotEqual/Equal` (constante, expressão,
  comparador), `Must/MustAsync` (3 overloads), comparações `LessThan/GreaterThan/…` (constante + expressão,
  8 overloads cada família), `InclusiveBetween/ExclusiveBetween`, `CreditCard`, `IsInEnum`, `IsEnumName`,
  `PrecisionScale`, `ScalePrecision`, `Custom/CustomAsync`, `ChildRules`, `SetValidator/SetAsyncValidator`,
  `SetInheritanceValidator`, `ValidateAndThrow(Async)`.
- **Pipeline interno** (`FluentValidation.Internal`): `PropertyRule<T,TProp>` (uma regra por propriedade),
  `RuleComponent<T,TProp>` (um validador dentro da cadeia), `CollectionPropertyRule` (RuleForEach com
  index builder), `ConditionBuilder` (When com Otherwise!), `RulesetValidatorSelector`,
  `MemberNameValidatorSelector`, `PropertyChain`, `MessageFormatter` (placeholders), `AccessorCache<T>`
  (cache de getters), `ValidationStrategy<T>` (API fluente de execução).
- **Validators** (`FluentValidation.Validators`): 25+ classes `IPropertyValidator<T,TProp>` separadas
  (NotNullValidator, EmailValidator com `AspNetCoreCompatibleEmailValidator`, CreditCardValidator,
  PrecisionScaleValidator, PolymorphicValidator…).
- **Recursos**: 40+ idiomas (`LanguageManager`), `ValidatorOptions.Global` (estado global: LanguageManager,
  DisplayName resolver, PropertyNameResolver, cascade default), `ValidationFailure` (PropertyName,
  ErrorMessage, AttemptedValue, CustomState, Severity, ErrorCode, FormattedMessagePlaceholderValues),
  `ValidationResult` (Errors, RuleSetsExecuted, ToDictionary), `ValidationException`.
- **TestHelper** (`FluentValidation.TestHelper`): `TestValidationResult`, `ShouldHaveValidationError…`.
- **DI**: `AddValidatorsFromAssembly` (em FluentValidation.DependencyInjectionExtensions — outro pacote),
  `AssemblyScanner`, `ValidatorFactoryBase`, `IValidatorFactory`.

### 2.2 Ardalis.GuardClauses 5.0.0 — guard clauses para argumentos
- `Guard` (entry: `Guard.Against.Null(x)`), `IGuardClause` (extensibility token), `GuardClauseExtensions`
  (50+ métodos estáticos: `Null`, `NullOrEmpty`, `NullOrWhiteSpace`, `OutOfRange`, `NullOrOutOfRange`,
  `StringTooShort/TooLong`, `Negative`, `NegativeOrZero`, `Zero`, `Default`, `EnumOutOfRange`,
  `InvalidInput`, `InvalidFormat`, `NotFound`, `OutOfSQLDateRange`, `Expression`, `AgainstExpression`).
- `ValidatedNotNullAttribute` (análise de nullability). `NotFoundException` especializada.
- Sem mensagens customizáveis ricas; sem suporte a objetos/DTOs; sem resultados agregados.

### 2.3 Validot 2.6.0 — performance máxima, API declarativa própria
- 526 tipos; API própria (`Predicate<T>`, template de mensagens, translation) — NÃO fluente com expressões
  do C#; curva de aprendizado própria; performance de topo (compile-time built). Sem async nativo? (v2 tem?).
- Não foi seguido como modelo de API ( paradigma diferente), mas referencia a meta de performance.

### 2.4 Valit 2.0.0 — fluent, mas BaseModel + result próprio
- `Valit`, `Valit.Rules` (8), `Valit.Result` — API fluente encadeada com `IValitator<TObject,TResult>`;
  pouco adotado; mensagens simples; sem async robusto.

### 2.5 DataAnnotations 5.0.0 — atributos built-in
- `ValidationAttribute` (`IsValid(object)`), `RequiredAttribute`, `StringLengthAttribute`, `RegularExpressionAttribute`,
  `RangeAttribute`, `EmailAddressAttribute` — declarativo por atributo, sem fluência, sem composição rica,
  mensagens por recurso; `Validator.TryValidateObject`.

## 3. Matriz de recursos consolidada

| Recurso | FluentValidation | GuardClauses | Validot | Valit | DataAnnotations | **XForge.Validation** |
|---|:-:|:-:|:-:|:-:|:-:|:-:|
| Fluent `RuleFor` com expressões | ✅ | ❌ | ❌ | ✅ | ❌ | ✅ |
| Built-in validators (NotNull, Length, Email…) | ✅ (25+) | ✅ (guard) | ✅ | ✅ | ✅ (10) | ✅ (20+) |
| Custom Must/MustAsync | ✅ | ✅ (Expression) | ✅ | ✅ | parcial | ✅ |
| Condições When/Unless (+Async) | ✅ (+Otherwise) | ❌ | ✅ | ❌ | ❌ | ✅ (+Otherwise) |
| RuleSets + selector | ✅ | ❌ | ✅ | ❌ | ❌ | ✅ |
| RuleForEach + ChildRules + SetValidator | ✅ | ❌ | ✅ | ✅ | ❌ | ✅ |
| Include de outros validators | ✅ | ❌ | ✅ | ❌ | ❌ | ✅ |
| Nested objects | ✅ | ❌ | ✅ | ❌ | ❌ | ✅ |
| Cascade modes | ✅ (class+rule) | n/a | ✅ | ❌ | ❌ | ✅ (class+chain+global default) |
| Async (ValidateAsync, MustAsync, WhenAsync) | ✅ (+exceção se sync usado c/ async rule) | ✅ (ExpressionAsync) | parcial | ❌ | ❌ | ✅ (ValueTask; sync valida sem async) |
| Guard clauses para argumentos | ❌ | ✅ (50+) | ❌ | ❌ | ❌ | ✅ (Ensure API) |
| Resultado agregado + ToDictionary | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ (+TryGetError por membro) |
| Placeholders em mensagens | ✅ | ❌ | ✅ (template) | ❌ | ❌ | ✅ |
| Severity (Error/Warning/Info) | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ |
| DI scanning built-in | ❌ (pacote extra) | ❌ | ❌ | ❌ | ❌ | ✅ (AddXForgeValidation) |
| Zero reflexão no hot path | ✅ (compilado) | ✅ | ✅ | ❌ | parcial | ✅ (delegates compilados 1×) |
| Sem estado global estático | ❌ (ValidatorOptions.Global) | ✅ | ✅ | ❌ | n/a | ✅ |
| TestHelper | ✅ (pacote extra) | ❌ | ✅ | ❌ | ❌ | 🗺️ roadmap |
| Multi-target amplo | ✅ | ✅ | ns2.0 | ns2.0 | ✅ | ✅ (ns2.0+net8.0) |

## 4. Gaps e oportunidades identificados

1. **Duas APIs em uma**: validação de objetos (FluentValidation) e guard clauses (GuardClauses) vivem em
   pacotes separados com APIs incompatíveis — unificar as duas necessidades (fluxo de aplicação valida DTOs;
   métodos públicos protegem argumentos) com mensagens/filosofia consistentes.
2. **Estado global estático** (`ValidatorOptions.Global`) dificulta testes paralelos/multi-tenant —
   XForge usa defaults por configuração/validador, sem singleton global.
3. **Async assimétrico** no FV (validar com `Validate` quando existem regras async lança
   `AsyncValidatorInvokedSynchronouslyException`) — XForge: `Validate` executa as regras síncronas e ignora
   as async com erro de validação? Melhor: `Validate` lança exceção clara ANTES de executar (fail-fast, sem
   meia-validação) — mantido, mas com `ValueTask` no `ValidateAsync` para zero alocação quando só há regras sync.
4. **Mensagens de erro com códigos padronizados** — FV usa nomes de classes de validator; XForge usa códigos
   curtos estáveis (`NOT_NULL`, `NOT_EMPTY`, `LENGTH`, `EMAIL`…) prontos para i18n por dicionário.
5. **DI scanning embutido** — FV exige pacote separado; XForge traz `AddXForgeValidation` no core.
6. **Severity além de Error** já existe no FV — manter paridade, mas com filtragem embutida
   (`result.Errors.Where(e => e.Severity == Error)` via helpers).
7. **When/Otherwise** — FV tem `ConditionBuilder` com Otherwise; XForge inclui (`When(cond, action).Otherwise(action)`).
8. **TestHelper** — roadmap (FV tem pacote separado).

## 5. Evidências REA

- Origem: `.nupkg` de `api.nuget.org` extraídos para `C:\Users\renat\AppData\Local\Temp\opencode\rea-nuget\ext`.
- IDs de evidência (membros completos): tabela §1. Limitações: inspeção estática não carrega/executa o assembly;
  tokens build-local (sha256+MVID).
