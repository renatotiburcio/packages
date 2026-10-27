namespace XForge.DesignSystem.Components.DataDisplay.Video;

public sealed class XFVideoTrack
{
    public string Src { get; set; } = string.Empty;
    public string Kind { get; set; } = "subtitles";
    public string? SrcLang { get; set; }
    public string? Label { get; set; }
    public bool Default { get; set; }
}
