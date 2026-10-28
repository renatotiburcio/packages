using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using XForge.DesignSystem.Components.Layout.TileLayout;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Layout.TileLayout;

public class XFTileLayoutTest : XForgeTestContext
{
    public XFTileLayoutTest()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IXFLocalizer, TestLocalizer>();
    }

    [Fact]
    public void Render_WithNoTiles_ShowsEmpty()
    {
        var cut = Render<XFTileLayout>();
        Assert.Contains("xf-tile-layout", cut.Markup);
    }

    [Fact]
    public void Render_WithTitle_ShowsTitle()
    {
        var cut = Render<XFTileLayout>(p => p.Add(x => x.Title, "My Dashboard"));
        Assert.Contains("My Dashboard", cut.Markup);
    }

    [Fact]
    public void Render_WithTiles_ShowsTileTitles()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption { Id = "t1", Title = "Revenue" },
            new XFTileLayoutOption { Id = "t2", Title = "Users" }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("Revenue", cut.Markup);
        Assert.Contains("Users", cut.Markup);
    }

    [Fact]
    public void Render_WithTileContent_ShowsContent()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Chart"
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("Chart", cut.Markup);
    }

    [Fact]
    public void Render_WithTileIcon_ShowsIcon()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Stats",
                Icon = "<circle cx='12' cy='12' r='10' />"
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("Stats", cut.Markup);
    }

    [Fact]
    public void Render_WithTileColumnSpan_AdjustsLayout()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Wide",
                ColumnSpan = 6
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("grid-column: 6", cut.Markup);
    }

    [Fact]
    public void Render_WithTileRowSpan_AdjustsHeight()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Tall",
                RowSpan = 3
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("grid-row: 3", cut.Markup);
    }

    [Fact]
    public void Render_WithoutShowHeader_HidesHeader()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "No Header",
                ShowHeader = false
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.DoesNotContain("No Header", cut.Markup);
    }

    [Fact]
    public void Render_WithAllowEdit_ShowsEditModeButton()
    {
        var cut = Render<XFTileLayout>(p =>
        {
            p.Add(x => x.Title, "Dashboard");
            p.Add(x => x.AllowEdit, true);
        });
        Assert.Contains("tilelayout.edit_mode", cut.Markup);
    }

    [Fact]
    public void Render_WithoutAllowEdit_HidesEditModeButton()
    {
        var cut = Render<XFTileLayout>(p =>
        {
            p.Add(x => x.Title, "Dashboard");
            p.Add(x => x.AllowEdit, false);
        });
        Assert.DoesNotContain("tilelayout.edit_mode", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomColumns_AdjustsGrid()
    {
        var cut = Render<XFTileLayout>(p => p.Add(x => x.Columns, 6));
        Assert.Contains("grid-template-columns: repeat(6", cut.Markup);
    }

    [Fact]
    public void Render_WithCustomGap_AdjustsSpacing()
    {
        var cut = Render<XFTileLayout>(p => p.Add(x => x.Gap, 24));
        Assert.Contains("gap: 24px", cut.Markup);
    }

    [Fact]
    public void Render_ShowsAriaLabel()
    {
        var cut = Render<XFTileLayout>();
        Assert.Contains("aria-label", cut.Markup);
    }

    [Fact]
    public void Render_WithoutShowHeader_HidesHeaderSection()
    {
        var cut = Render<XFTileLayout>(p => p.Add(x => x.ShowHeader, false));
        Assert.DoesNotContain("tilelayout.edit_mode", cut.Markup);
    }

    [Fact]
    public void Render_WithMultipleTiles_AllVisible()
    {
        var tiles = Enumerable.Range(1, 4).Select(i => new XFTileLayoutOption
        {
            Id = $"t{i}",
            Title = $"Tile {i}"
        }).ToList();

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        for (int i = 1; i <= 4; i++)
        {
            Assert.Contains($"Tile {i}", cut.Markup);
        }
    }

    [Fact]
    public void Render_WithTileCssClass_AppliesClass()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Styled",
                CssClass = "custom-tile-class"
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("custom-tile-class", cut.Markup);
    }

    [Fact]
    public void Render_WithTileIconColor_AppliesColor()
    {
        var tiles = new[]
        {
            new XFTileLayoutOption
            {
                Id = "t1",
                Title = "Colored",
                Icon = "<circle />",
                IconColor = "text-green-500"
            }
        };

        var cut = Render<XFTileLayout>(p => p.Add(x => x.Tiles, tiles));
        Assert.Contains("text-green-500", cut.Markup);
    }

    [Fact]
    public void Render_RespectsDisabled()
    {
        var cut = Render<XFTileLayout>(p => p.Add(x => x.Disabled, true));
        Assert.Contains("xf-tile-layout", cut.Markup);
    }
}
