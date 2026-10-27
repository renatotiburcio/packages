using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.FileInput;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFFileInputTest : BunitContext
{
    public XFFileInputTest() { JSInterop.Mode = JSRuntimeMode.Loose; }

    [Fact] public void FileInput_Renders_Default()
    {
        var cut = Render<XFFileInput>();
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithCustomLabel()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Label, "Upload File"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithAccept()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Accept, ".pdf,.doc"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithMultiple()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Multiple, true));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithMaximumFileCount()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.MaximumFileCount, 5));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithMaximumFileSize()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.MaximumFileSize, 5_000_000));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithRequired()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Required, true));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithFullWidthFalse()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.FullWidth, false));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithCustomClass()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Class, "custom-file"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithShowProgress()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.ShowProgress, true));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithVariantFilled()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithSizeSm()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Size, XFSize.Sm));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithTonePrimary()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithCustomChooseText()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.ChooseText, "Browse"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithSingleTitle()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.SingleTitle, "Drop a file"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithMultipleTitle()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.MultipleTitle, "Drop files"));
        Assert.NotNull(cut);
    }

    [Fact] public void FileInput_Renders_WithEmptyContent()
    {
        var cut = Render<XFFileInput>(p => p.Add(x => x.EmptyContent, (RenderFragment)(b => b.AddContent(0, "empty"))));
        Assert.NotNull(cut);
    }
}
