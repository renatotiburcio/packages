namespace XForge.Validation.Tests;

public class ValidatorTests
{
    private static PessoaV ValidPessoa() => new()
    {
        Id = 1,
        Nome = "Renato",
        Idade = 30,
        Email = "renato@x.com",
        Saldo = 100m,
        Tipo = TipoCliente.PessoaFisica,
        Endereco = new EnderecoV { Cidade = "Sorocaba" },
        Pedidos = new List<PedidoV> { new() { Produto = "prod", Quantidade = 1, Total = 10m } },
    };

    private static PessoaV Clone(PessoaV source) => new()
    {
        Id = source.Id,
        Nome = source.Nome,
        Sobrenome = source.Sobrenome,
        Idade = source.Idade,
        Email = source.Email,
        Tipo = source.Tipo,
        Saldo = source.Saldo,
        Cidade = source.Cidade,
        Senha = source.Senha,
        ConfirmacaoSenha = source.ConfirmacaoSenha,
        Pedidos = source.Pedidos is null ? null : new List<PedidoV>(source.Pedidos),
        Endereco = source.Endereco is null ? null : new EnderecoV { Cidade = source.Endereco.Cidade, Uf = source.Endereco.Uf },
    };

    private readonly PessoaValidator _validator = new();

    [Fact]
    public void Valid_instance_passes()
    {
        var result = _validator.Validate(ValidPessoa());
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void NotEmpty_fails_for_empty_and_null()
    {
        var source = ValidPessoa();
        source.Nome = string.Empty;

        var result = _validator.Validate(source);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome" && e.ErrorCode == "NOT_EMPTY");
    }

    [Fact]
    public void MaxLength_reports_total_length()
    {
        var source = ValidPessoa();
        source.Nome = new string('a', 101);

        var result = _validator.Validate(source);

        Assert.False(result.IsValid);
        var failure = result.Errors.Single(e => e.PropertyName == "Nome" && e.ErrorCode == "MAX_LENGTH");
        Assert.Contains("101", failure.ErrorMessage);
    }

    [Fact]
    public void Cascade_stop_on_first_failure_yields_single_error_per_chain()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome)
            .NotEmpty()
            .Length(5, 100);

        var result = validator.Validate(new PessoaV { Nome = string.Empty });

        // NOT_EMPTY falha e a cadeia para: MAX_LENGTH não executa
        var nomeErrors = result.Errors.Where(e => e.PropertyName == "Nome").ToList();
        Assert.Single(nomeErrors);
        Assert.Equal("NOT_EMPTY", nomeErrors[0].ErrorCode);
    }

    [Fact]
    public void Cascade_continue_collects_all_errors()
    {
        var validator = new Validator<PessoaV>(CascadeMode.Continue);
        validator.RuleFor(p => p.Nome)
            .NotEmpty()
            .Length(5, 100);

        var result = validator.Validate(new PessoaV { Nome = string.Empty });

        Assert.Equal(2, result.Errors.Count(e => e.PropertyName == "Nome"));
    }

    [Fact]
    public void Severity_is_applied()
    {
        var source = ValidPessoa();
        source.Idade = 200; // InclusiveBetween com Severity.Warning

        var result = _validator.Validate(source);
        var warning = result.Errors.Single(e => e.PropertyName == "Idade");

        Assert.Equal(Severity.Warning, warning.Severity);
    }

    [Fact]
    public void Email_validates_only_when_present()
    {
        var source = ValidPessoa();
        source.Email = "invalido";

        var result = _validator.Validate(source);

        Assert.Contains(result.Errors, e => e.ErrorCode == "EMAIL");
    }

    [Fact]
    public void Equal_to_other_member()
    {
        var source = ValidPessoa();
        source.Senha = "123";
        source.ConfirmacaoSenha = "456";

        var result = _validator.Validate(source);

        Assert.Contains(result.Errors, e => e.ErrorCode == "EQUAL" && e.PropertyName == "ConfirmacaoSenha");
    }

    [Fact]
    public void IsInEnum_rejects_undefined_value()
    {
        var source = ValidPessoa();
        source.Tipo = (TipoCliente)99;

        var result = _validator.Validate(source);

        Assert.Contains(result.Errors, e => e.ErrorCode == "IS_IN_ENUM");
    }

    [Fact]
    public void ChildRules_validate_each_item_with_indexed_property_name()
    {
        var source = ValidPessoa();
        source.Pedidos = new List<PedidoV>
        {
            new() { Produto = "ok", Quantidade = 1 },
            new() { Produto = string.Empty, Quantidade = 0 },
        };

        var result = _validator.Validate(source);

        Assert.Contains(result.Errors, e => e.PropertyName == "Pedidos[1].Produto" && e.ErrorCode == "NOT_EMPTY");
        Assert.Contains(result.Errors, e => e.PropertyName == "Pedidos[1].Quantidade" && e.ErrorCode == "GREATER_THAN");
    }

    [Fact]
    public void SetValidator_validates_nested_object()
    {
        var source = ValidPessoa();
        source.Endereco = new EnderecoV { Cidade = string.Empty };

        var result = _validator.Validate(source);

        Assert.Contains(result.Errors, e => e.PropertyName == "Endereco.Cidade");
    }

    [Fact]
    public void Nested_null_is_skipped()
    {
        var source = ValidPessoa();
        source.Endereco = null;

        var result = _validator.Validate(source);

        Assert.DoesNotContain(result.Errors, e => e.PropertyName.StartsWith("Endereco"));
    }

    [Fact]
    public void RuleSet_runs_only_selected_rules()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome).NotEmpty();
        validator.RuleSet("Cadastro", v =>
        {
            validator.RuleFor(p => p.Idade).InclusiveBetween(18, 99);
        });

        var instance = new PessoaV { Nome = string.Empty, Idade = 10 };

        var defaultResult = validator.Validate(instance);
        Assert.False(defaultResult.IsValid);
        Assert.Single(defaultResult.Errors);
        Assert.Equal("Nome", defaultResult.Errors[0].PropertyName);

        var cadastroResult = validator.Validate(instance, "Cadastro");
        Assert.False(cadastroResult.IsValid);
        Assert.Single(cadastroResult.Errors);
        Assert.Equal("Idade", cadastroResult.Errors[0].PropertyName);
    }

    [Fact]
    public async Task Async_rules_run_in_ValidateAsync()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome)
            .MustAsync((name, ct) => ValueTask.FromResult(name != "bloqueado"));

        var result = await validator.ValidateAsync(new PessoaV { Nome = "bloqueado" });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "MUST");
    }

    [Fact]
    public void Sync_validate_with_async_rules_throws()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome)
            .MustAsync((name, ct) => ValueTask.FromResult(true));

        Assert.Throws<InvalidOperationException>(() => validator.Validate(new PessoaV()));
    }

    [Fact]
    public void When_scopes_conditions_to_rules_created_inside()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome).NotEmpty();
        validator.When(p => p.Tipo == TipoCliente.PessoaJuridica, v =>
        {
            v.RuleFor(p => p.Cidade).NotEmpty();
        });

        var juridicaSemCidade = new PessoaV { Nome = "x", Tipo = TipoCliente.PessoaJuridica };
        var fisicaSemCidade = new PessoaV { Nome = "x", Tipo = TipoCliente.PessoaFisica };

        Assert.False(validator.Validate(juridicaSemCidade).IsValid);
        Assert.True(validator.Validate(fisicaSemCidade).IsValid);
    }

    [Fact]
    public void Include_merges_rules_from_another_validator()
    {
        var baseValidator = new Validator<PessoaV>();
        baseValidator.RuleFor(p => p.Nome).NotEmpty();

        var validator = new Validator<PessoaV>();
        validator.Include(baseValidator);
        validator.RuleFor(p => p.Idade).InclusiveBetween(18, 99);

        var result = validator.Validate(new PessoaV { Nome = string.Empty, Idade = 10 });

        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void Placeholders_are_formatted()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome).Length(5, 10)
            .WithMessage("'{PropertyName}' tem {TotalLength} chars (min {MinLength}, max {MaxLength}).");

        var result = validator.Validate(new PessoaV { Nome = "abc" });

        var failure = result.Errors.Single();
        Assert.Equal("'Nome' tem 3 chars (min 5, max 10).", failure.ErrorMessage);
    }

    [Fact]
    public void WithErrorCode_and_WithSeverity_overrides()
    {
        var validator = new Validator<PessoaV>();
        validator.RuleFor(p => p.Nome).NotEmpty()
            .WithErrorCode("NOME_OBRIGATORIO")
            .WithSeverity(Severity.Info);

        var result = validator.Validate(new PessoaV { Nome = string.Empty });

        var failure = result.Errors.Single();
        Assert.Equal("NOME_OBRIGATORIO", failure.ErrorCode);
        Assert.Equal(Severity.Info, failure.Severity);
    }

    [Fact]
    public void Result_helpers_work()
    {
        var source = ValidPessoa();
        source.Nome = string.Empty;
        source.Email = "invalido";

        var result = _validator.Validate(source);
        var dictionary = result.ToDictionary();

        Assert.True(dictionary.ContainsKey("Nome"));
        Assert.Equal("EMAIL", result.ErrorsFor("Email").First().ErrorCode);
        Assert.Null(result.GetError("Inexistente"));
        Assert.StartsWith("'", result.GetError("Email"));
    }
}
