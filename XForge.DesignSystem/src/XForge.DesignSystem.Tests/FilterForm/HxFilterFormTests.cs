using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XForge.DesignSystem.Components.Forms.FilterForm;
using Xunit;
namespace XForge.DesignSystem.Tests.FilterForm;
class PersonFilter { public string? Name { get; set; } public int? MinAge { get; set; } public int? MaxAge { get; set; } }
class PersonChipGenerator : HxChipGeneratorBase<PersonFilter>
{
    public override IEnumerable<FilterChip> GenerateChips(PersonFilter m) { if (!string.IsNullOrEmpty(m.Name)) yield return new FilterChip("name", "Name", m.Name); if (m.MinAge is int mn) yield return new FilterChip("minAge", "Min Age", mn.ToString()); if (m.MaxAge is int mx) yield return new FilterChip("maxAge", "Max Age", mx.ToString()); }
}
public class HxFilterFormTests
{
    [Fact] public void New_Form_Has_Empty_Chips_When_No_Generator() { var f = new HxFilterForm<PersonFilter>(); Assert.Empty(f.Chips); }
#pragma warning disable BL0005
    [Fact] public void Chips_Are_Empty_When_Model_Is_Default() { var f = new HxFilterForm<PersonFilter> { ChipGenerator = new PersonChipGenerator() }; Assert.Empty(f.Chips); }
    [Fact] public void Chips_Are_Generated_From_Model() { var f = new HxFilterForm<PersonFilter> { ChipGenerator = new PersonChipGenerator(), Model = new PersonFilter { Name = "John", MinAge = 18, MaxAge = 65 } }; Assert.Equal(3, f.Chips.Count); Assert.Contains(f.Chips, c => c.Key == "name" && c.Value == "John"); }
    [Fact] public void Reset_Clears_Model() { var f = new HxFilterForm<PersonFilter> { Model = new PersonFilter { Name = "John" } }; f.ResetAsync().GetAwaiter().GetResult(); Assert.Null(f.CurrentModel.Name); }
#pragma warning restore BL0005
}