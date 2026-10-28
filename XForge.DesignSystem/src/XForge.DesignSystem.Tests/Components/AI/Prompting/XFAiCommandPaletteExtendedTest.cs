using Bunit;
using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Components.AI.Prompting;
using Xunit;

namespace XForge.DesignSystem.Tests.Components.AI.Prompting;

public class XFAiCommandPaletteExtendedTest : XForgeTestContext
{
    public XFAiCommandPaletteExtendedTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private static XFAiCommandItem CreateCommand(string id, string label, string? description = null,
        string? icon = null, string? category = null, string? shortcut = null, bool disabled = false)
    {
        return new XFAiCommandItem
        {
            Id = id,
            Label = label,
            Description = description,
            Icon = icon,
            Category = category,
            Shortcut = shortcut,
            Disabled = disabled
        };
    }

    private IRenderedComponent<XFAiCommandPalette> RenderPalette(
        IReadOnlyList<XFAiCommandItem>? commands = null,
        Action<ComponentParameterCollectionBuilder<XFAiCommandPalette>>? configure = null)
    {
        return Render<XFAiCommandPalette>(parameters =>
        {
            parameters.Add(p => p.Visible, true);
            parameters.Add(p => p.Commands, commands ?? Array.Empty<XFAiCommandItem>());
            configure?.Invoke(parameters);
        });
    }

    [Fact]
    public void DefaultRender_RendersDialog()
    {
        var cut = RenderPalette();
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void AriaLabel_Renders()
    {
        var cut = RenderPalette();
        Assert.Contains("Paleta de comandos", cut.Markup);
    }

    [Fact]
    public void AriaModal_Renders()
    {
        var cut = RenderPalette();
        Assert.Contains("aria-modal=\"true\"", cut.Markup);
    }

    [Fact]
    public void SearchInput_RendersWithRoleCombobox()
    {
        var cut = RenderPalette();
        Assert.Contains("role=\"combobox\"", cut.Markup);
    }

    [Fact]
    public void Placeholder_RendersDefault()
    {
        var cut = RenderPalette();
        Assert.Contains("Pesquisar comandos", cut.Markup);
    }

    [Fact]
    public void Placeholder_Custom_Renders()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.Placeholder, "Search..."));
        Assert.Contains("Search...", cut.Markup);
    }

    [Fact]
    public void CommandList_RendersWithRoleListbox()
    {
        var cut = RenderPalette();
        Assert.Contains("role=\"listbox\"", cut.Markup);
    }

    [Fact]
    public void Commands_RenderWithRoleOption()
    {
        var commands = new[] { CreateCommand("c1", "Command 1") };
        var cut = RenderPalette(commands);
        var options = cut.FindAll("div[role=option]");
        Assert.True(options.Count >= 1);
    }

    [Fact]
    public void CommandLabel_Renders()
    {
        var commands = new[] { CreateCommand("c1", "My Command") };
        var cut = RenderPalette(commands);
        Assert.Contains("My Command", cut.Markup);
    }

    [Fact]
    public void CommandDescription_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd", description: "A description") };
        var cut = RenderPalette(commands);
        Assert.Contains("A description", cut.Markup);
    }

    [Fact]
    public void CommandCategory_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd", category: "General") };
        var cut = RenderPalette(commands);
        Assert.Contains("General", cut.Markup);
    }

    [Fact]
    public void CommandShortcut_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd", shortcut: "Ctrl+K") };
        var cut = RenderPalette(commands);
        Assert.Contains("Ctrl+K", cut.Markup);
    }

    [Fact]
    public void AriaSelected_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd 1"), CreateCommand("c2", "Cmd 2") };
        var cut = RenderPalette(commands);
        Assert.Contains("aria-selected", cut.Markup);
    }

    [Fact]
    public void CustomNoResultsText_Accepted()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.NoResultsText, "Nothing found"));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void EmptySearchContent_Renders()
    {
        var cut = RenderPalette(configure: p => p
            .Add(p => p.EmptySearchContent, (RenderFragment)(b => b.AddContent(0, "Empty content"))));
        Assert.Contains("Empty content", cut.Markup);
    }

    [Fact]
    public void RecentActionsLabel_ParameterAccepted()
    {
        // RecentActionsLabel only renders when _recentActions.Count > 0 (populated after command execution)
        // Just verify the parameter is accepted and component renders
        var cut = RenderPalette(configure: p => p
            .Add(x => x.ShowRecentActions, true)
            .Add(x => x.RecentActionsLabel, "Recent"));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void CommandsLabel_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd 1"), CreateCommand("c2", "Cmd 2") };
        var cut = RenderPalette(commands, p => p.Add(x => x.CommandsLabel, "All Commands"));
        Assert.Contains("All Commands", cut.Markup);
    }

    [Fact]
    public void FooterHints_Render()
    {
        var cut = RenderPalette();
        Assert.Contains("navegar", cut.Markup);
        Assert.Contains("executar", cut.Markup);
        Assert.Contains("fechar", cut.Markup);
    }

    [Fact]
    public void KeyboardHints_Render()
    {
        var cut = RenderPalette();
        var kbds = cut.FindAll("kbd");
        Assert.True(kbds.Count >= 3);
    }

    [Fact]
    public void VisibleFalse_HidesComponent()
    {
        var cut = Render<XFAiCommandPalette>(p => p.Add(x => x.Visible, false));
        Assert.DoesNotContain("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void CustomClass_Appended()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.Class, "custom-palette"));
        Assert.Contains("custom-palette", cut.Markup);
    }

    [Fact]
    public void Id_Renders()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.Id, "my-palette"));
        Assert.Contains("my-palette", cut.Markup);
    }

    [Fact]
    public void Style_Renders()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.Style, "width: 500px"));
        Assert.Contains("width: 500px", cut.Markup);
    }

    [Fact]
    public void AdditionalAttributes_Render()
    {
        var cut = RenderPalette(configure: p => p
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "value" } }));
        Assert.Contains("data-test", cut.Markup);
    }

    [Fact]
    public void CommandTemplate_Renders()
    {
        var commands = new[] { CreateCommand("c1", "Cmd 1") };
        var cut = RenderPalette(commands, p => p
            .Add(p => p.CommandTemplate, (RenderFragment<XFAiCommandItem>)(ctx =>
                (RenderFragment)(b => b.AddContent(0, $"Template: {ctx.Label}")))));
        Assert.Contains("Template: Cmd 1", cut.Markup);
    }

    [Fact]
    public void DisabledCommand_RendersWithOpacity()
    {
        var commands = new[] { CreateCommand("c1", "Disabled", disabled: true) };
        var cut = RenderPalette(commands);
        Assert.Contains("opacity-50", cut.Markup);
    }

    [Fact]
    public void MultipleCommands_Render()
    {
        var commands = new[]
        {
            CreateCommand("c1", "Command 1"),
            CreateCommand("c2", "Command 2"),
            CreateCommand("c3", "Command 3")
        };
        var cut = RenderPalette(commands);
        Assert.Contains("Command 1", cut.Markup);
        Assert.Contains("Command 2", cut.Markup);
        Assert.Contains("Command 3", cut.Markup);
    }

    [Fact]
    public void MaxResults_ParameterAccepted()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.MaxResults, 5));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void ShowRecentActions_ParameterAccepted()
    {
        var cut = RenderPalette(configure: p => p.Add(x => x.ShowRecentActions, false));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void OnCommandExecute_CallbackAccepted()
    {
        var cut = RenderPalette(configure: p => p
            .Add(p => p.OnCommandExecute, EventCallback.Factory.Create<XFAiCommandItem>(this, _ => { })));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void OnSearch_CallbackAccepted()
    {
        var cut = RenderPalette(configure: p => p
            .Add(p => p.OnSearch, EventCallback.Factory.Create<string>(this, _ => { })));
        Assert.Contains("role=\"dialog\"", cut.Markup);
    }

    [Fact]
    public void Overlay_Renders()
    {
        var cut = RenderPalette();
        Assert.Contains("fixed inset-0", cut.Markup);
    }

    [Fact]
    public void BackdropBlur_Renders()
    {
        var cut = RenderPalette();
        Assert.Contains("backdrop-blur-sm", cut.Markup);
    }
}
