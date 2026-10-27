# Changelog — XForge.Validation.TestHelper

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/).

## [0.1.0] - 2026-10-26

### Adicionado
- Assertions de teste sobre `ValidationResult` (estilo FluentValidation.TestHelper):
  `ShouldNotHaveAnyValidationErrors`, `ShouldHaveAnyValidationError`,
  `ShouldNotHaveValidationErrorFor` (nome ou expressão), `ShouldHaveValidationErrorFor`
  (nome ou expressão + mensagem opcional), `ShouldHaveValidationErrorWithCode`
  (códigos padronizados do XForge) e `ShouldNotHaveValidationErrorsWithSeverity`.
- Helpers de resultado: `TestValidationResult`, `TestValidationResultAsync` (incluindo overload
  com RuleSet) e os atalhos `ShouldValidateSuccessfully` / `ShouldValidateSuccessfullyAsync`.
- Falhas de assertion via `XunitException` (mensagens integradas ao xUnit).
- 15 testes cobrindo caminhos que passam, caminhos que falham (com verificação de mensagem),
  expressão inválida e os helpers de validador — net8.0 + net10.0.
- Multi-target `netstandard2.0` + `net8.0` (consumível de qualquer framework de teste).
