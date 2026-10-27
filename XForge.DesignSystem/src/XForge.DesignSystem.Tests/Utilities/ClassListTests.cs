using System.Linq;
using XForge.DesignSystem.Components.Utilities.Css;
using Xunit;
namespace XForge.DesignSystem.Tests.Utilities;
public class ClassListTests
{
    [Fact] public void Empty_ClassList_Has_No_Classes() { var c = new ClassList(); Assert.Equal(0, c.Count); Assert.True(c.IsEmpty); Assert.Equal(string.Empty, c.ToString()); }
    [Fact] public void Add_Class_Increases_Count() { var c = new ClassList().Add("a"); Assert.Equal(1, c.Count); Assert.Equal("a", c.ToString()); }
    [Fact] public void Add_Multiple_Classes_Concatenates() { var c = new ClassList().Add("a").Add("b").Add("c"); Assert.Equal("a b c", c.ToString()); }
    [Fact] public void Add_Empty_Or_Null_Ignored() { var c = new ClassList().Add("a").Add(null).Add("").Add("b"); Assert.Equal(2, c.Count); Assert.Equal("a b", c.ToString()); }
    [Fact] public void Add_With_Condition_True_Adds() { var c = new ClassList().Add("a",true).Add("b",false); Assert.Equal("a", c.ToString()); }
    [Fact] public void Add_With_Condition_False_Skips() { var c = new ClassList().Add("a").Add("hidden",false); Assert.Equal("a", c.ToString()); }
    [Fact] public void Add_True_False_By_Condition() { var c = new ClassList().Add("active","inactive",true); Assert.Equal("active", c.ToString()); var c2 = new ClassList().Add("active","inactive",false); Assert.Equal("inactive", c2.ToString()); }
    [Fact] public void Contains_Returns_True_If_Class_Present() { var c = new ClassList().Add("foo").Add("bar"); Assert.True(c.Contains("foo")); Assert.False(c.Contains("baz")); }
    [Fact] public void Implicit_String_Operator() { ClassList c = new ClassList().Add("x"); string s = c; Assert.Equal("x", s); }
}