using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using XForge.DesignSystem.Components.Feedback.RichTextEditor.Toolbar;
using XForge.Localization;

namespace XForge.DesignSystem.Tests.Components.Feedback.RichTextEditor;

public class XFRichTextEditorExtendedToolbarComponentTest : BunitContext
{
    public XFRichTextEditorExtendedToolbarComponentTest() { Services.AddSingleton<IXFLocalizer, TestLocalizer>(); }

    [Fact] public void AlignLeft_RendersButton() { var cut = Render<XFRichTextEditorAlignLeft>(); Assert.Contains("Align Left", cut.Markup); }
    [Fact] public void AlignCenter_RendersButton() { var cut = Render<XFRichTextEditorAlignCenter>(); Assert.Contains("Align Center", cut.Markup); }
    [Fact] public void AlignRight_RendersButton() { var cut = Render<XFRichTextEditorAlignRight>(); Assert.Contains("Align Right", cut.Markup); }
    [Fact] public void AlignJustify_RendersButton() { var cut = Render<XFRichTextEditorAlignJustify>(); Assert.Contains("Justify", cut.Markup); }
    [Fact] public void FontFamily_RendersButton() { var cut = Render<XFRichTextEditorFontFamily>(); Assert.Contains("Font Family", cut.Markup); }
    [Fact] public void FontSize_RendersButton() { var cut = Render<XFRichTextEditorFontSize>(); Assert.Contains("Font Size", cut.Markup); }
    [Fact] public void FontColor_RendersButton() { var cut = Render<XFRichTextEditorFontColor>(); Assert.Contains("Font Color", cut.Markup); }
    [Fact] public void BackColor_RendersButton() { var cut = Render<XFRichTextEditorBackColor>(); Assert.Contains("Background Color", cut.Markup); }
    [Fact] public void InsertImage_RendersButton() { var cut = Render<XFRichTextEditorInsertImage>(); Assert.Contains("Insert Image", cut.Markup); }
    [Fact] public void InsertTable_RendersButton() { var cut = Render<XFRichTextEditorInsertTable>(); Assert.Contains("Insert Table", cut.Markup); }
    [Fact] public void InsertHR_RendersButton() { var cut = Render<XFRichTextEditorInsertHorizontalRule>(); Assert.Contains("Horizontal Rule", cut.Markup); }
    [Fact] public void Indent_RendersButton() { var cut = Render<XFRichTextEditorIndent>(); Assert.Contains("Increase Indent", cut.Markup); }
    [Fact] public void Outdent_RendersButton() { var cut = Render<XFRichTextEditorOutdent>(); Assert.Contains("Decrease Indent", cut.Markup); }
    [Fact] public void FormatBlock_RendersButton() { var cut = Render<XFRichTextEditorFormatBlock>(); Assert.Contains("Format Block", cut.Markup); }
    [Fact] public void Blockquote_RendersButton() { var cut = Render<XFRichTextEditorBlockquote>(); Assert.Contains("Blockquote", cut.Markup); }
    [Fact] public void ViewSource_RendersButton() { var cut = Render<XFRichTextEditorViewSource>(); Assert.Contains("View Source", cut.Markup); }
}
