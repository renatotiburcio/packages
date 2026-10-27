# XForge Assets Generator (T080)

CLI .NET tool that converts SVG assets into C# classes for `XFIconCatalog` (T078) and `XFEmojiCatalog` (T079).

## Install

```bash
dotnet tool install --add-source ./tools
```

## Usage

```bash
# Icons
xforge-assets-gen --assets ./icons --target ./Generated --library icon --sizes 16,20,24

# Emojis
xforge-assets-gen --assets ./emojis --target ./Generated --library emoji
```

## Filename convention

- Icon: `{name}_{variant}_{size}.svg` (e.g. `add_Regular_20.svg`)
- Emoji: `{name}_{group}_{style}_{skintone?}.svg` (e.g. `rocket_TravelPlaces_Color_Default.svg`)

## Output

One `.cs` file per `(Variant, Size)` or `(Group, Style)` containing one inner class per icon/emoji with a `Svg` constant.

## License

MIT (XForge Design System). Forks FluentUI.AssetsGenerator (MIT) and adapts it for XForge.