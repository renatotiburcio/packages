using Microsoft.AspNetCore.Components;
using System.Text;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.DataDisplay.Avatar
{
    public partial class XFAvatar : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-avatar";

        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Initials { get; set; }
        [Parameter] public string? ImageSrc { get; set; }
        [Parameter] public string? Alt { get; set; }
        [Parameter] public string? Email { get; set; }
        [Parameter] public bool UseGravatar { get; set; }
        [Parameter] public bool Lazy { get; set; } = true;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Decorative { get; set; }
        [Parameter] public XFAvatarSize Size { get; set; } = XFAvatarSize.Md;
        [Parameter] public XFAvatarShape Shape { get; set; } = XFAvatarShape.Circle;
        [Parameter] public XFAvatarStatus Status { get; set; } = XFAvatarStatus.None;
        [Parameter] public string? StatusTitle { get; set; }
        [Parameter] public string DefaultAltText { get; set; } = "Avatar";
        [Parameter] public string OnlineStatusLabel { get; set; } = "Disponivel";
        [Parameter] public string AwayStatusLabel { get; set; } = "Ausente";
        [Parameter] public string BusyStatusLabel { get; set; } = "Ocupado";
        [Parameter] public string OfflineStatusLabel { get; set; } = "Indisponivel";
        [Parameter] public string DefaultStatusLabel { get; set; } = "Status";
        [Parameter] public RenderFragment? Icon { get; set; }

        private bool imageFailed;

        private bool ShouldRenderImage => !imageFailed && !string.IsNullOrWhiteSpace(ResolvedImageSource);

        private string? ResolvedImageSource => !string.IsNullOrWhiteSpace(ImageSrc)
            ? ImageSrc
            : UseGravatar && !string.IsNullOrWhiteSpace(Email)
                ? BuildGravatarUrl(Email)
                : null;

        private string? Role => Decorative ? null : "img";

        private string? AriaLabelValue => Decorative ? null : Alt ?? Name ?? ResolvedInitials;

        private string? TitleValue => string.IsNullOrWhiteSpace(Name) ? null : Name;

        private string ImageAlt => Decorative ? string.Empty : Alt ?? Name ?? DefaultAltText;

        private string LoadingValue => Lazy ? "lazy" : "eager";

        private string ResolvedInitials => !string.IsNullOrWhiteSpace(Initials)
            ? Initials.Trim().ToUpperInvariant()
            : BuildInitials(Name);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            OnlineStatusLabel = L("avatar.online", "Disponivel");
            AwayStatusLabel = L("avatar.away", "Ausente");
            BusyStatusLabel = L("avatar.busy", "Ocupado");
            OfflineStatusLabel = L("avatar.offline", "Indisponivel");
            DefaultStatusLabel = L("avatar.status", "Status");

            if (!string.IsNullOrWhiteSpace(ImageSrc) || UseGravatar)
            {
                imageFailed = false;
            }
        }

        protected override string GetComponentClass()
        {
            return GetClass($"{BaseClass} relative inline-flex shrink-0 select-none items-center justify-center overflow-visible bg-[color-mix(in_oklab,var(--color-primary)_10%,transparent)] text-primary ring-1 ring-[color-mix(in_oklab,var(--color-primary)_16%,transparent)] {SizeClass} {ShapeClass}");
        }

        private string SizeClass => Size switch
        {
            XFAvatarSize.Xs => "h-6 w-6 text-[10px]",
            XFAvatarSize.Sm => "h-8 w-8 text-xs",
            XFAvatarSize.Lg => "h-12 w-12 text-base",
            XFAvatarSize.Xl => "h-16 w-16 text-lg",
            XFAvatarSize.Xxl => "h-20 w-20 text-xl",
            _ => "h-10 w-10 text-sm"
        };

        private string ShapeClass => Shape switch
        {
            XFAvatarShape.Square => "rounded-lg",
            XFAvatarShape.Rounded => "rounded-xl",
            _ => "rounded-full"
        };

        private string InitialsClass => "grid h-full w-full place-items-center rounded-[inherit] font-semibold leading-none";

        private string IconClass => "grid h-full w-full place-items-center rounded-[inherit] text-current";

        private string StatusClass
        {
            get
            {
                var size = Size switch
                {
                    XFAvatarSize.Xs => "h-2 w-2 border",
                    XFAvatarSize.Sm => "h-2.5 w-2.5 border-2",
                    XFAvatarSize.Lg => "h-3.5 w-3.5 border-2",
                    XFAvatarSize.Xl or XFAvatarSize.Xxl => "h-4 w-4 border-2",
                    _ => "h-3 w-3 border-2"
                };

                return $"absolute bottom-0 right-0 z-30 rounded-full border-bg-surface shadow-sm {size} {StatusToneClass}";
            }
        }

        private string StatusToneClass => Status switch
        {
            XFAvatarStatus.Online => "bg-success",
            XFAvatarStatus.Away => "bg-warning",
            XFAvatarStatus.Busy => "bg-danger",
            XFAvatarStatus.Offline => "bg-surface-400",
            _ => "bg-primary"
        };

        private string StatusAriaLabel => StatusTitle ?? Status switch
        {
            XFAvatarStatus.Online => OnlineStatusLabel,
            XFAvatarStatus.Away => AwayStatusLabel,
            XFAvatarStatus.Busy => BusyStatusLabel,
            XFAvatarStatus.Offline => OfflineStatusLabel,
            _ => DefaultStatusLabel
        };

        private Task HandleImageError()
        {
            imageFailed = true;
            return Task.CompletedTask;
        }

        private static string BuildInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                return "?";
            }

            if (parts.Length == 1)
            {
                return parts[0][..1].ToUpperInvariant();
            }

            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
        }

        private static string BuildGravatarUrl(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();
            var hash = ComputeMd5Hex(Encoding.UTF8.GetBytes(normalized));

            return $"https://secure.gravatar.com/avatar/{hash}?d=retro&s=160";
        }

        private static string ComputeMd5Hex(byte[] input)
        {
            var shifts = new[]
            {
                7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
                5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
                4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
                6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
            };

            var constants = new uint[64];
            for (var i = 0; i < constants.Length; i++)
            {
                constants[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * uint.MaxValue + 1);
            }

            var bitLength = (ulong)input.Length * 8;
            var paddedLength = ((input.Length + 8) / 64 + 1) * 64;
            var padded = new byte[paddedLength];
            Array.Copy(input, padded, input.Length);
            padded[input.Length] = 0x80;
            BitConverter.GetBytes(bitLength).CopyTo(padded, paddedLength - 8);

            uint a0 = 0x67452301;
            uint b0 = 0xefcdab89;
            uint c0 = 0x98badcfe;
            uint d0 = 0x10325476;

            for (var offset = 0; offset < padded.Length; offset += 64)
            {
                var words = new uint[16];
                for (var i = 0; i < words.Length; i++)
                {
                    words[i] = BitConverter.ToUInt32(padded, offset + i * 4);
                }

                var a = a0;
                var b = b0;
                var c = c0;
                var d = d0;

                for (var i = 0; i < 64; i++)
                {
                    uint f;
                    int g;

                    if (i < 16)
                    {
                        f = (b & c) | (~b & d);
                        g = i;
                    }
                    else if (i < 32)
                    {
                        f = (d & b) | (~d & c);
                        g = (5 * i + 1) % 16;
                    }
                    else if (i < 48)
                    {
                        f = b ^ c ^ d;
                        g = (3 * i + 5) % 16;
                    }
                    else
                    {
                        f = c ^ (b | ~d);
                        g = (7 * i) % 16;
                    }

                    var temp = d;
                    d = c;
                    c = b;
                    b += RotateLeft(a + f + constants[i] + words[g], shifts[i]);
                    a = temp;
                }

                a0 += a;
                b0 += b;
                c0 += c;
                d0 += d;
            }

            var hash = new byte[16];
            BitConverter.GetBytes(a0).CopyTo(hash, 0);
            BitConverter.GetBytes(b0).CopyTo(hash, 4);
            BitConverter.GetBytes(c0).CopyTo(hash, 8);
            BitConverter.GetBytes(d0).CopyTo(hash, 12);

            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static uint RotateLeft(uint value, int shift)
        {
            return (value << shift) | (value >> (32 - shift));
        }
    }
}
