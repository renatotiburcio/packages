using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;

namespace XForge.DesignSystem.Components.DataDisplay.Video;

public partial class XFVideo : XForgeComponentBase
{
    [Parameter] public string? Src { get; set; }
    [Parameter] public string? Type { get; set; }
    [Parameter] public IReadOnlyList<XFVideoSource> Sources { get; set; } = Array.Empty<XFVideoSource>();
    [Parameter] public IReadOnlyList<XFVideoTrack> Tracks { get; set; } = Array.Empty<XFVideoTrack>();
    [Parameter] public string? Poster { get; set; }
    [Parameter] public string? AriaLabel { get; set; } = "Midia de video";
    [Parameter] public string FallbackText { get; set; } = "Seu navegador nao suporta video.";
    [Parameter] public bool Controls { get; set; } = true;
    [Parameter] public bool Autoplay { get; set; }
    [Parameter] public bool Muted { get; set; }
    [Parameter] public bool Loop { get; set; }
    [Parameter] public bool PlaysInline { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public string Preload { get; set; } = "metadata";
    [Parameter] public XFVideoFit Fit { get; set; } = XFVideoFit.Contain;
    [Parameter] public RenderFragment? FallbackContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', Src, Type, Sources.Count, Tracks.Count, Poster, AriaLabel, FallbackText, Controls, Autoplay, Muted, Loop, PlaysInline, Visible, Preload, Fit);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

    private bool HasVideo => !string.IsNullOrWhiteSpace(Src) || Sources.Any(source => !string.IsNullOrWhiteSpace(source.Src));

    private string RootClass => GetClass($"xf-video relative overflow-hidden rounded-xl bg-surface-950 ring-1 ring-border-light aspect-video");

    private string VideoClass => Join(
        "block h-full w-full bg-surface-950",
        FitClass);

    private string FallbackClass =>
        "flex h-full min-h-48 w-full items-center justify-center bg-surface-100 text-text-secondary";

    private string FitClass => Fit switch
    {
        XFVideoFit.Cover => "object-cover",
        XFVideoFit.Fill => "object-fill",
        _ => "object-contain"
    };

    private static string Join(params string?[] values) =>
        string.Join(" ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
}
