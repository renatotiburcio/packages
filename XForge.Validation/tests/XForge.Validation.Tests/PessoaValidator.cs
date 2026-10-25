using Microsoft.Extensions.DependencyInjection;

namespace XForge.Validation.Tests;

public class PessoaValidator : Validator<PessoaV>
{
    public PessoaValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaxLength(100);

        RuleFor(p => p.Idade)
            .InclusiveBetween(0, 150)
            .WithSeverity(Severity.Warning);

        RuleFor(p => p.Email)
            .EmailAddress()
            .When(p => !string.IsNullOrEmpty(p.Email));

        RuleFor(p => p.Saldo)
            .GreaterThanOrEqualTo(0);

        RuleFor(p => p.Tipo)
            .IsInEnum();

        RuleFor(p => p.ConfirmacaoSenha)
            .Equal(p => p.Senha)
            .When(p => !string.IsNullOrEmpty(p.Senha));

        RuleForEach(p => p.Pedidos)
            .ChildRules(pedido =>
            {
                pedido.RuleFor(i => i.Produto).NotEmpty();
                pedido.RuleFor(i => i.Quantidade).GreaterThan(0);
            });

        RuleFor(p => p.Endereco)
            .SetValidator(new EnderecoValidator())
            .When(p => p.Endereco is not null);
    }
}

public class EnderecoValidator : Validator<EnderecoV>
{
    public EnderecoValidator()
    {
        RuleFor(e => e.Cidade).NotEmpty();
    }
}
