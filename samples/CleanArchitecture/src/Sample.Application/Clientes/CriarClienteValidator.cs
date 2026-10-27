using XForge.Validation;

namespace Sample.Application.Clientes;

/// <summary>Regras de validação do comando (fluent, com códigos padronizados).</summary>
public sealed class CriarClienteValidator : Validator<CriarClienteCommand>
{
    public CriarClienteValidator()
    {
        RuleFor(c => c.Nome).NotEmpty().Length(1, 80);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
    }
}
