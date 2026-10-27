using XForge.Validation;
using XForge.Validation.TestHelper;
using Xunit;

namespace XForge.Validation.TestHelper.Tests;

public class TestHelperTests
{
    private static ValidationResult With(params ValidationFailure[] failures) => new(failures);

    // ---------- caminhos que passam ----------

    [Fact]
    public void ShouldNotHaveAnyValidationErrors_passa_quando_nao_ha_falhas()
        => ValidationResult.Success.ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void ShouldHaveValidationErrorFor_passa_quando_existe_falha()
        => With(new ValidationFailure("Nome", "Nome é obrigatório", "NOT_NULL"))
            .ShouldHaveValidationErrorFor("Nome");

    [Fact]
    public void ShouldHaveValidationErrorFor_com_expressao_passa_quando_existe_falha()
        => With(new ValidationFailure("Email", "E-mail inválido", "EMAIL"))
            .ShouldHaveValidationErrorFor<Pessoa, string>(p => p.Email);

    [Fact]
    public void ShouldHaveValidationErrorFor_com_mensagem_passa_quando_mensagem_bate()
        => With(new ValidationFailure("Idade", "Idade deve ser maior que 18", "GREATER_THAN"))
            .ShouldHaveValidationErrorFor("Idade", "Idade deve ser maior que 18");

    [Fact]
    public void ShouldNotHaveValidationErrorFor_passa_quando_falha_e_de_outro_membro()
        => With(new ValidationFailure("Nome", "Nome é obrigatório"))
            .ShouldNotHaveValidationErrorFor("Email");

    [Fact]
    public void ShouldHaveValidationErrorWithCode_passa_quando_codigo_existe()
        => With(new ValidationFailure("Documento", "CPF inválido", "CPF"))
            .ShouldHaveValidationErrorWithCode("Documento", "CPF");

    [Fact]
    public void ShouldNotHaveValidationErrorsWithSeverity_passa_quando_so_existem_avisos()
        => With(new ValidationFailure("Apelido", "evite apelidos", "CUSTOM", Severity.Warning))
            .ShouldNotHaveValidationErrorsWithSeverity(Severity.Error);

    // ---------- caminhos que falham (a mensagem importa) ----------

    [Fact]
    public void ShouldNotHaveAnyValidationErrors_falha_quando_existem_falhas()
    {
        var exception = Assert.Throws<Xunit.Sdk.XunitException>(() =>
            With(new ValidationFailure("Nome", "Nome é obrigatório")).ShouldNotHaveAnyValidationErrors());

        Assert.Contains("Nome", exception.Message);
    }

    [Fact]
    public void ShouldHaveValidationErrorFor_falha_quando_membro_nao_tem_falha()
    {
        var exception = Assert.Throws<Xunit.Sdk.XunitException>(() =>
            With(new ValidationFailure("Nome", "Nome é obrigatório"))
                .ShouldHaveValidationErrorFor("Email"));

        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void ShouldHaveValidationErrorFor_falha_quando_mensagem_diverge()
    {
        var exception = Assert.Throws<Xunit.Sdk.XunitException>(() =>
            With(new ValidationFailure("Nome", "Nome é obrigatório"))
                .ShouldHaveValidationErrorFor("Nome", "mensagem diferente"));

        Assert.Contains("mensagem diferente", exception.Message);
    }

    [Fact]
    public void ShouldHaveValidationErrorWithCode_falha_quando_codigo_diverge()
    {
        var exception = Assert.Throws<Xunit.Sdk.XunitException>(() =>
            With(new ValidationFailure("Nome", "Nome é obrigatório", "NOT_NULL"))
                .ShouldHaveValidationErrorWithCode("Nome", "EMAIL"));

        Assert.Contains("EMAIL", exception.Message);
    }

    [Fact]
    public void Expressao_invalida_gera_ArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            With().ShouldHaveValidationErrorFor<Pessoa, int>(p => p.Idade + 1));

        Assert.Contains("expressão", exception.Message);
    }

    // ---------- helpers de validador ----------

    [Fact]
    public void ShouldValidateSuccessfully_passa_para_validador_que_nao_tem_regras_quebradas()
        => new PessoaValidator().ShouldValidateSuccessfully(new Pessoa { Nome = "ana", Idade = 30 });

    [Fact]
    public void ShouldValidateSuccessfully_falha_para_instancia_invalida()
    {
        var exception = Assert.Throws<Xunit.Sdk.XunitException>(() =>
            new PessoaValidator().ShouldValidateSuccessfully(new Pessoa { Nome = "", Idade = 10 }));

        Assert.Contains("falha", exception.Message);
    }

    [Fact]
    public async Task TestValidationResultAsync_devolve_resultado_para_encadear()
    {
        var result = await new PessoaValidator().TestValidationResultAsync(
            new Pessoa { Nome = "", Idade = 10 });

        result.ShouldHaveValidationErrorFor("Nome");
        Assert.False(result.IsValid);
    }

    // ---------- modelos ----------

    public class Pessoa
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Idade { get; set; }
    }

    private sealed class PessoaValidator : Validator<Pessoa>
    {
        public PessoaValidator()
        {
            RuleFor(x => x.Nome).NotEmpty();
            RuleFor(x => x.Idade).GreaterThan(18);
        }
    }
}
