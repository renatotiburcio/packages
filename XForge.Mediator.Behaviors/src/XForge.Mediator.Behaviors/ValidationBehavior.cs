using XForge.Mediator;
using XForge.Validation;

namespace XForge.Mediator.Behaviors;

/// <summary>
/// Behavior de validação: executa todos os IValidator do request registrados antes do handler
/// e lança ValidationException quando houver falhas.
/// Register: services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Cria o behavior.</summary>
    /// <param name="validators">Validadores do request (resolvidos do DI; pode ser vazio).</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
