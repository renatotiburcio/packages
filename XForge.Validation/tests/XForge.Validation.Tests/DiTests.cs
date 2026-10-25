using Microsoft.Extensions.DependencyInjection;

namespace XForge.Validation.Tests;

public class DiTests
{
    [Fact]
    public void AddXForgeValidation_registers_validators_from_assembly()
    {
        var provider = new ServiceCollection()
            .AddXForgeValidation(typeof(PessoaValidator).Assembly)
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<PessoaV>>();
        Assert.IsType<PessoaValidator>(validator);

        var result = validator.Validate(new PessoaV { Nome = "ok", Tipo = TipoCliente.PessoaFisica });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validators_are_singletons()
    {
        var provider = new ServiceCollection()
            .AddXForgeValidation(typeof(PessoaValidator).Assembly)
            .BuildServiceProvider();

        Assert.Same(
            provider.GetRequiredService<IValidator<PessoaV>>(),
            provider.GetRequiredService<IValidator<PessoaV>>());
    }

    [Fact]
    public void ValidateAndThrow_throws_with_errors()
    {
        var validator = new PessoaValidator();

        var exception = Assert.Throws<ValidationException>(() => validator.ValidateAndThrow(new PessoaV()));

        Assert.NotEmpty(exception.Errors);
        Assert.Contains(exception.Errors, e => e.PropertyName == "Nome");
    }

    [Fact]
    public void ValidateAndThrow_passes_when_valid()
    {
        var validator = new PessoaValidator();
        var result = validator.ValidateAndThrow(new PessoaV { Nome = "ok", Idade = 20, Tipo = TipoCliente.PessoaFisica, Saldo = 0 });

        Assert.True(result.IsValid);
    }
}
