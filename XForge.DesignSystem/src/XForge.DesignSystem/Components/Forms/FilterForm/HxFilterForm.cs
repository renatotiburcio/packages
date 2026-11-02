using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.Forms.FilterForm;

public record FilterChip(string Key, string Label, string? Value);

public interface IHxChipGenerator<T> { IEnumerable<FilterChip> GenerateChips(T model); }

public abstract class HxChipGeneratorBase<T> : IHxChipGenerator<T> { public abstract IEnumerable<FilterChip> GenerateChips(T model); }

public class HxFilterForm<T> : XForgeComponentBase where T : class, new()
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public T? Model { get; set; }
    [Parameter] public IHxChipGenerator<T>? ChipGenerator { get; set; }
    [Parameter] public EventCallback<T> OnApply { get; set; }
    [Parameter] public EventCallback OnReset { get; set; }
    public T CurrentModel { get; protected set; } = new();

    // Model atribuido fora do ciclo de render (inicializador de objeto, testes unitarios)
    // nunca passa por SetParametersAsync; Chips resolve o modelo efetivo sob demanda.
    private T EffectiveModel => Model is not null && !ReferenceEquals(Model, CurrentModel) ? Model : CurrentModel;

    public IReadOnlyList<FilterChip> Chips => ChipGenerator?.GenerateChips(EffectiveModel).ToList() ?? new List<FilterChip>();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        var result = base.SetParametersAsync(parameters);
        if (Model is not null && !ReferenceEquals(Model, CurrentModel))
        {
            CurrentModel = Model;
        }
        return result;
    }
    protected override void OnParametersSet() { if (Model is not null) CurrentModel = Model; }
    public Task ApplyAsync() => OnApply.InvokeAsync(CurrentModel);
    public Task ResetAsync() { CurrentModel = new T(); return OnReset.InvokeAsync(); }
    public Task RemoveChipAsync(FilterChip chip) { OnReset.InvokeAsync(); return Task.CompletedTask; }
}
