using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using XForge.DesignSystem.Components.Forms.FileInput;
using XForge.DesignSystem.Core.Primitives;

namespace XForge.DesignSystem.Tests.Components.Forms;

public sealed class XFFileInputInteractionTest : BunitContext
{
    public XFFileInputInteractionTest() { JSInterop.Mode = JSRuntimeMode.Loose; }
    [Fact] public void Renders_Default() => Render<XFFileInput>();
    [Fact] public void Renders_WithLabel() { var cut = Render<XFFileInput>(p => p.Add(x => x.Label, "Upload")); Assert.Contains("Upload", cut.Markup); }
    [Fact] public void Renders_WithAccept() => Render<XFFileInput>(p => p.Add(x => x.Accept, ".pdf,.doc"));
    [Fact] public void Renders_Multiple() => Render<XFFileInput>(p => p.Add(x => x.Multiple, true));
    [Fact] public void Renders_Required() => Render<XFFileInput>(p => p.Add(x => x.Required, true));
    [Fact] public void Renders_ShowProgress() => Render<XFFileInput>(p => p.Add(x => x.ShowProgress, true));
    [Fact] public void Renders_VariantOutline() => Render<XFFileInput>(p => p.Add(x => x.Variant, XFFormVariant.Outline));
    [Fact] public void Renders_VariantFilled() => Render<XFFileInput>(p => p.Add(x => x.Variant, XFFormVariant.Filled));
    [Fact] public void Renders_SizeSm() => Render<XFFileInput>(p => p.Add(x => x.Size, XFSize.Sm));
    [Fact] public void Renders_SizeLg() => Render<XFFileInput>(p => p.Add(x => x.Size, XFSize.Lg));
    [Fact] public void Renders_TonePrimary() => Render<XFFileInput>(p => p.Add(x => x.Tone, XFSemanticColor.Primary));
    [Fact] public void Renders_FullWidthFalse() => Render<XFFileInput>(p => p.Add(x => x.FullWidth, false));
    [Fact] public void Renders_HelperText() { var cut = Render<XFFileInput>(p => p.Add(x => x.HelperText, "Max 5MB")); Assert.Contains("Max 5MB", cut.Markup); }
    [Fact] public void Renders_Title() { var cut = Render<XFFileInput>(p => p.Add(x => x.Title, "Choose file")); Assert.Contains("Choose file", cut.Markup); }
    [Fact] public void Renders_MaxFileCount() => Render<XFFileInput>(p => p.Add(x => x.MaximumFileCount, 5));
    [Fact] public void Renders_MaxFileSize() => Render<XFFileInput>(p => p.Add(x => x.MaximumFileSize, 5 * 1024 * 1024));
    [Fact] public void FileInputItem_Created() { var item = new XFFileInputItem(Name: "test.txt", Size: 1024, ContentType: "text/plain", LastModified: DateTimeOffset.UtcNow, IsValid: true, Error: null); Assert.Equal("test.txt", item.Name); Assert.Equal(1024, item.Size); Assert.True(item.IsValid); }
    [Fact] public void FileInputItem_Invalid() { var item = new XFFileInputItem(Name: "big.zip", Size: 999999999, ContentType: "application/zip", LastModified: DateTimeOffset.UtcNow, IsValid: false, Error: "Too large"); Assert.False(item.IsValid); Assert.Equal("Too large", item.Error); }
    [Fact] public void FileInputItem_StatusDefaultPending() => Assert.Equal(XFFileInputStatus.Pending, new XFFileInputItem(Name: "a", Size: 1, ContentType: "t", LastModified: DateTimeOffset.UtcNow, IsValid: true, Error: null).Status);
    [Fact] public void FileInputItem_ProgressDefaultZero() => Assert.Equal(0, new XFFileInputItem(Name: "a", Size: 1, ContentType: "t", LastModified: DateTimeOffset.UtcNow, IsValid: true, Error: null).Progress);
    [Fact] public void FileInputStatus_Has5Values() => Assert.Equal(5, Enum.GetValues<XFFileInputStatus>().Length);
}
