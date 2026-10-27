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
    public IReadOnlyList<FilterChip> Chips => ChipGenerator?.GenerateChips(CurrentModel).ToList() ?? new List<FilterChip>();
    protected override void OnParametersSet() { if (Model is not null) CurrentModel = Model; }
    public Task ApplyAsync() => OnApply.InvokeAsync(CurrentModel);
    public Task ResetAsync() { CurrentModel = new T(); return OnReset.InvokeAsync(); }
    public Task RemoveChipAsync(FilterChip chip) { OnReset.InvokeAsync(); return Task.CompletedTask; }
}
