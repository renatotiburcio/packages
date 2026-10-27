# XForge.Validation.TestHelper

Assertions de teste para o XForge.Validation — estilo FluentValidation.TestHelper, adaptadas ao
modelo do XForge (falhas por nome de membro, códigos padronizados e severidade).

## Uso

```csharp
using XForge.Validation;
using XForge.Validation.TestHelper;

public class CriarClienteValidatorTests
{
    [Fact]
    public void Nome_vazio_deve_falhar()
    {
        var validator = new CriarClienteValidator();
        var result = validator.TestValidationResult(new CriarClienteCommand { Nome = "" });

        result.ShouldHaveValidationErrorFor(c => c.Nome);   // por expressão
        result.ShouldHaveValidationErrorWithCode("Nome", "NOT_NULL");  // código padronizado
    }

    [Fact]
    public void Instancia_valida_nao_deve_ter_falhas()
    {
        var validator = new CriarClienteValidator();

        validator.ShouldValidateSuccessfully(new CriarClienteCommand { Nome = "acme" });
        // ou: validator.TestValidationResult(cmd).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public Task Validacao_async()
        => new CriarClienteValidator()
            .ShouldValidateSuccessfullyAsync(new CriarClienteCommand { Nome = "acme" });
}
```

## API

| Assertion | Significado |
|---|---|
| `ShouldNotHaveAnyValidationErrors()` | nenhuma falha (nenhuma severidade) |
| `ShouldHaveAnyValidationError()` | ao menos uma falha em qualquer membro |
| `ShouldNotHaveValidationErrorFor("Nome")` / `(x => x.Nome)` | sem falhas para o membro |
| `ShouldHaveValidationErrorFor("Nome")` / `(x => x.Nome)` | falha para o membro (mensagem opcional comparada exatamente) |
| `ShouldHaveValidationErrorWithCode("Nome", "CPF")` | falha com o código padronizado do XForge |
| `ShouldNotHaveValidationErrorsWithSeverity(Severity.Warning)` | nenhuma falha da severidade informada |
| `TestValidationResult(x)` / `TestValidationResultAsync(x)` | obtém o resultado para encadear |
| `ShouldValidateSuccessfully(x)` / `…Async(x)` | valida e já asserta ausência de falhas |

As assertions usam `XunitException` — integração nativa com xUnit (inclusive nas mensagens de falha).

## Desenvolvimento

```bash
dotnet test XForge.Validation.TestHelper/XForge.Validation.TestHelper.sln
```
