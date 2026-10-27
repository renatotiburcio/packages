using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sample.Application.Clientes;
using Sample.Domain;
using Sample.Infrastructure;
using XForge.Mediator;
using XForge.Mediator.Behaviors;
using XForge.Mediator.Generated;
using XForge.Validation;

var builder = WebApplication.CreateBuilder(args);

// ---- DI: os 3 pacotes integrados ----
builder.Services.AddSingleton<IClienteRepository, InMemoryClienteRepository>(); // Infrastructure
builder.Services.AddXForgeValidation(typeof(CriarClienteValidator).Assembly);   // Validation (scanning do assembly)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Behaviors
builder.Services.AddXForgeMediatorGenerated();                                 // Mediator (source generator)

var app = builder.Build();

// ---- endpoints (minimal API) ----
app.MapPost("/clientes", async (CriarClienteCommand command, ISender sender, CancellationToken ct) =>
{
    try
    {
        await sender.Send(command, ct);
    }
    catch (ValidationException validationException)
    {
        // o ValidationBehavior barra o handler agregando as falhas
        var payload = System.Text.Json.JsonSerializer.Serialize(validationException.Errors);
        return Results.Text(payload, "application/json", statusCode: StatusCodes.Status422UnprocessableEntity);
    }

    return Results.Created();
});

app.MapGet("/clientes/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
{
    var cliente = await sender.Send(new ObterClienteQuery(id), ct);
    return cliente is null
        ? Results.NotFound()
        : Results.Text(System.Text.Json.JsonSerializer.Serialize(cliente), "application/json");
});

// auxiliar dos testes: último id criado (o repositório é singleton no sample)
app.MapGet("/clientes/ids", (IClienteRepository repository) =>
{
    var todos = repository.List();
    return Results.Text(System.Text.Json.JsonSerializer.Serialize(new { Ultimo = todos.Count == 0 ? 0 : todos[^1].Id }), "application/json");
});

// diagnostico simples em texto (a página de exceção do dev falha neste host de teste)
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    await context.Response.WriteAsync(feature?.Error?.ToString() ?? "erro desconhecido");
}));

app.Run();

/// <summary>Expõe o Program para os testes de integração (WebApplicationFactory).</summary>
public partial class Program;






