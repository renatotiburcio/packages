using Microsoft.AspNetCore.Components;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.OnScreenKeyboard
{
    public partial class XFOnScreenKeyboard : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-onscreen-keyboard";

        private bool _isShift;
        private bool _isCapsLock;

        #region Parameters

        /// <summary>Current keyboard layout.</summary>
        [Parameter]
        public XFOnScreenKeyboardLayout Layout { get; set; } = XFOnScreenKeyboardLayout.Alphanumeric;

        /// <summary>Layout changed callback.</summary>
        [Parameter]
        public EventCallback<XFOnScreenKeyboardLayout> LayoutChanged { get; set; }

        /// <summary>Key pressed callback.</summary>
        [Parameter]
        public EventCallback<XFOnScreenKeyboardKeyEventArgs> OnKeyPress { get; set; }

        /// <summary>Label above the keyboard.</summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>Key height in pixels.</summary>
        [Parameter]
        public int KeyHeight { get; set; } = 44;

        /// <summary>Whether to show the status bar.</summary>
        [Parameter]
        public bool ShowStatusBar { get; set; } = true;

        #endregion

        #region Labels

        private string AriaLabel => L("onscreen_keyboard.aria_label", "On-screen keyboard");
        private string CapsLockText => L("onscreen_keyboard.caps_lock", "Caps Lock");
        private string ShiftText => L("onscreen_keyboard.shift", "Shift");

        #endregion

        #region CSS

        private string KeyboardContainerClass => "rounded-xl border border-border-light bg-surface-100 p-2";

        private string KeyClass(XFOnScreenKeyboardKey key) => string.Join(" ",
            "inline-flex items-center justify-center rounded-lg border transition-all duration-100",
            "select-none cursor-pointer",
            key.IsWide ? "px-4" : "px-2",
            key.IsSpecial
                ? "border-surface-300 bg-surface-200 text-text-secondary hover:bg-surface-300 active:bg-surface-400"
                : "border-border-light bg-bg-surface text-text-primary hover:bg-surface-50 active:bg-surface-100 shadow-sm",
            _isShift || _isCapsLock ? "" : "");

        #endregion

        #region Layout

        private XFOnScreenKeyboardLayoutData CurrentLayout => Layout switch
        {
            XFOnScreenKeyboardLayout.Numeric => NumericLayout,
            XFOnScreenKeyboardLayout.Symbol => SymbolLayout,
            XFOnScreenKeyboardLayout.Email => EmailLayout,
            _ => AlphanumericLayout
        };

        private static readonly XFOnScreenKeyboardLayoutData AlphanumericLayout = new("Alphanumeric", new[]
        {
            CreateRow("QWERTYUIOP", 38),
            CreateRow("ASDFGHJKL", 38),
            CreateRow("ZXCVBNM", 38, specialKeys: new[] { ("SHIFT", "Shift", 52, true, true), ("BACKSPACE", "⌫", 52, true, true) }),
            CreateRowSpecial(new[] { ("@","@", 44, false, false), (".",".", 44, false, false), ("SPACE","Space", 80, false, false), ("ENTER", "Enter", 0, true, true) })
        });

        private static readonly XFOnScreenKeyboardLayoutData NumericLayout = new("Numeric", new[]
        {
            CreateRow("123", 44),
            CreateRow("456", 44),
            CreateRow("789", 44),
            CreateRowSpecial(new[] { (".",".", 44, false, false), ("0","0", 44, false, false), ("BACKSPACE","⌫", 44, true, true) })
        });

        private static readonly XFOnScreenKeyboardLayoutData SymbolLayout = new("Symbols", new[]
        {
            CreateRowSpecial(new[] { ("!","!", 44, false, false), ("@","@", 44, false, false), ("#","#", 44, false, false), ("$","$", 44, false, false), ("%","%", 44, false, false), ("^","^", 44, false, false) }),
            CreateRowSpecial(new[] { ("&","&", 44, false, false), ("*","*", 44, false, false), ("(","(", 44, false, false), (")",")", 44, false, false), ("-","-", 44, false, false), ("_","_", 44, false, false) }),
            CreateRowSpecial(new[] { ("+","+", 44, false, false), ("=", "=", 44, false, false), ("[","[", 44, false, false), ("]","]", 44, false, false), ("{","{", 44, false, false), ("}","}", 44, false, false) }),
            CreateRowSpecial(new[] { ("<","<", 44, false, false), (">",">", 44, false, false), ("?","?", 44, false, false), ("/","/", 44, false, false), ("BACKSPACE","⌫", 0, true, true) })
        });

        private static readonly XFOnScreenKeyboardLayoutData EmailLayout = new("Email", new[]
        {
            CreateRow("QWERTYUIOP", 38),
            CreateRow("ASDFGHJKL", 38),
            CreateRow("ZXCVBNM", 38, specialKeys: new[] { ("SHIFT", "Shift", 52, true, true), ("BACKSPACE", "⌫", 52, true, true) }),
            CreateRowSpecial(new[] { ("@","@", 44, false, false), (".",".", 44, false, false), ("_","_", 44, false, false), ("SPACE","Space", 80, false, false), ("ENTER", "Enter", 0, true, true) })
        });

        private static XFOnScreenKeyboardRow CreateRow(string letters, int keyWidth, (string Value, string Label, int Width, bool IsWide, bool IsSpecial)[]? specialKeys = null)
        {
            var keys = letters.Select(c => new XFOnScreenKeyboardKey
            {
                Value = c.ToString(),
                Label = c.ToString(),
                Width = keyWidth,
                AriaLabel = c.ToString()
            }).ToList();

            if (specialKeys != null)
            {
                keys.AddRange(specialKeys.Select(s => new XFOnScreenKeyboardKey
                {
                    Value = s.Value,
                    Label = s.Label,
                    Width = s.Width,
                    IsWide = s.IsWide,
                    IsSpecial = s.IsSpecial,
                    AriaLabel = s.Label
                }));
            }

            return new XFOnScreenKeyboardRow(keys);
        }

        private static XFOnScreenKeyboardRow CreateRowSpecial((string Value, string Label, int Width, bool IsWide, bool IsSpecial)[] keys)
        {
            return new XFOnScreenKeyboardRow(keys.Select(k => new XFOnScreenKeyboardKey
            {
                Value = k.Value,
                Label = k.Label,
                Width = k.Width,
                IsWide = k.IsWide,
                IsSpecial = k.IsSpecial,
                AriaLabel = k.Label
            }).ToList());
        }

        #endregion

        #region Key Handling

        private async Task OnKeyPressAsync(XFOnScreenKeyboardKey key)
        {
            switch (key.Value)
            {
                case "SHIFT":
                    _isShift = !_isShift;
                    StateHasChanged();
                    return;
                case "CAPS_LOCK":
                    _isCapsLock = !_isCapsLock;
                    StateHasChanged();
                    return;
                case "BACKSPACE":
                    await OnKeyPress.InvokeAsync(new XFOnScreenKeyboardKeyEventArgs { Key = "Backspace", Value = "", IsSpecial = true });
                    return;
                case "ENTER":
                    await OnKeyPress.InvokeAsync(new XFOnScreenKeyboardKeyEventArgs { Key = "Enter", Value = "\n", IsSpecial = true });
                    return;
                case "SPACE":
                    await OnKeyPress.InvokeAsync(new XFOnScreenKeyboardKeyEventArgs { Key = " ", Value = " " });
                    return;
                case "TAB":
                    await OnKeyPress.InvokeAsync(new XFOnScreenKeyboardKeyEventArgs { Key = "Tab", Value = "\t", IsSpecial = true });
                    return;
                default:
                    var value = key.Value;
                    if (_isShift ^ _isCapsLock)
                        value = value.ToUpperInvariant();
                    else if (!_isCapsLock)
                        value = value.ToLowerInvariant();

                    await OnKeyPress.InvokeAsync(new XFOnScreenKeyboardKeyEventArgs { Key = value, Value = value });

                    if (_isShift)
                    {
                        _isShift = false;
                        StateHasChanged();
                    }
                    return;
            }
        }

        /// <summary>Sets the keyboard layout.</summary>
        public async Task SetLayoutAsync(XFOnScreenKeyboardLayout layout)
        {
            Layout = layout;
            await LayoutChanged.InvokeAsync(layout);
            StateHasChanged();
        }

        #endregion
    }

    #region Supporting types

    public enum XFOnScreenKeyboardLayout
    {
        Alphanumeric,
        Numeric,
        Symbol,
        Email
    }

    public class XFOnScreenKeyboardLayoutData
    {
        public string Name { get; set; }
        public IReadOnlyList<XFOnScreenKeyboardRow> Rows { get; set; }

        public XFOnScreenKeyboardLayoutData(string name, XFOnScreenKeyboardRow[] rows)
        {
            Name = name;
            Rows = rows;
        }
    }

    public class XFOnScreenKeyboardRow
    {
        public IReadOnlyList<XFOnScreenKeyboardKey> Keys { get; set; }

        public XFOnScreenKeyboardRow(IReadOnlyList<XFOnScreenKeyboardKey> keys)
        {
            Keys = keys;
        }
    }

    public class XFOnScreenKeyboardKey
    {
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
        public string? AriaLabel { get; set; }
        public int Width { get; set; } = 38;
        public bool IsWide { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSpacer { get; set; }
        public int SpacerWidth { get; set; }
        public string? Icon { get; set; }
    }

    public class XFOnScreenKeyboardKeyEventArgs
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public bool IsSpecial { get; set; }
    }

    #endregion
}
