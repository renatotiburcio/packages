using Microsoft.Extensions.DependencyInjection;
using XForge.Mediator;
using XForge.Mediator.Behaviors;
using XForge.Validation;

namespace XForge.Mediator.Behaviors.Tests;

public record CriarCliente(string Nome, string Email) : IRequest<ClienteDto>;

public class ClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Cliente
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CriarClienteValidator : Validator<CriarCliente>
{
    public CriarClienteValidator()
    {
        RuleFor(c => c.Nome).NotEmpty().MaxLength(100);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
    }
}

public class CriarClienteHandler : IRequestHandler<CriarCliente, ClienteDto>
{
    public static List<Cliente> Banco { get; } = new();

    public ValueTask<ClienteDto> Handle(CriarCliente request, CancellationToken cancellationToken)
    {
        Banco.Add(new Cliente { Nome = request.Nome, Email = request.Email });
        return ValueTask.FromResult(new ClienteDto { Nome = request.Nome, Email = request.Email });
    }
}

public class IntegrationTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddXForgeMediator(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CriarClienteHandler>();
        });
        services.AddXForgeBehaviors();
        services.AddXForgeValidation(typeof(CriarClienteValidator).Assembly);

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Valid_request_flows_through_pipeline()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var dto = await mediator.Send(new CriarCliente("Renato", "renato@x.com"));

        Assert.Equal("Renato", dto.Nome);
        Assert.Single(CriarClienteHandler.Banco);
    }

    [Fact]
    public async Task Invalid_request_is_blocked_by_validation_behavior()
    {
        await using var provider = BuildProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => mediator.Send(new CriarCliente(string.Empty, "email-invalido")).AsTask());

        Assert.Contains(exception.Errors, e => e.PropertyName == "Nome" && e.ErrorCode == "NOT_EMPTY");
        Assert.Contains(exception.Errors, e => e.PropertyName == "Email" && e.ErrorCode == "EMAIL");
        Assert.Empty(CriarClienteHandler.Banco);
    }
}
