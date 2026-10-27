using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.Combobox;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFComboboxTypesTest
{
    [Fact] public void Filter_Contains_IsDefined() => Assert.True(Enum.IsDefined(XFComboboxFilter.Contains));
    [Fact] public void Filter_StartsWith_IsDefined() => Assert.True(Enum.IsDefined(XFComboboxFilter.StartsWith));
    [Fact] public void Filter_Equality() => Assert.Equal(XFComboboxFilter.Contains, XFComboboxFilter.Contains);
    [Fact] public void Filter_Inequality() => Assert.NotEqual(XFComboboxFilter.Contains, XFComboboxFilter.StartsWith);
}
