# Arquitetura — FluentUI Blazor (6-way)

## Stack
| Camada | Tecnologia | Versão |
|--------|-----------|--------|
| Frontend | Blazor WebAssembly / Server | net8.0/net9.0/net10.0 |
| Backend | ASP.NET Core | 8.0.27/9.0.16/10.0.8 |
| Database | EF Core (Sqlite) | 8.0.27/9.0.16/10.0.8 |
| CSS | Fluent Design System (custom) | 4.14.2 |
| Build | MSBuild + Central Package Management | Directory.Packages.props |
| Test | xUnit 2.9.3 + bUnit 1.38.5 + coverlet 10.0.0 | Microsoft.NET.Test.Sdk 18.0.1 |

## Estrutura do Repositório
```
fluentui-blazor/
├── Directory.Build.props           # Version 4.14.2
├── Directory.Packages.props        # Central package mgmt
├── Microsoft.FluentUI.sln          # 45 projects
├── src/
│   ├── Core/                       # 578 files
│   │   ├── Components/  DesignTokens/  Enums/
│   │   ├── Events/       Extensions/    Infrastructure/
│   │   ├── Resources/    Utilities/     wwwroot/
│   ├── Assets/
│   │   ├── FluentUI.Icons/         # ICON CATALOG (21034 icons)
│   │   ├── FluentUI.Emojis/        # EMOJI CATALOG (7885 emojis GZip)
│   │   ├── Sample/                 # Demo app
│   │   └── Generator/              # CLI .NET tool
│   ├── Extensions/                 # DataGrid EF, DataGrid OData, DesignToken SG
│   └── Templates/                  # dotnet new templates
└── tests/                          # xUnit + bUnit
```

## ICON CATALOG ARCHITECTURE (FOCO ESPECIAL)

### Modelo de dados
```csharp
public class IconInfo {
    public string Name { get; init; }
    public IconSize Size { get; init; } = Size24;
    public IconVariant Variant { get; init; } = Regular;
}
public class Icon : IconInfo {
    public string Content { get; }                 // SVG path
    public string? Color { get; private set; }
    public MarkupString ToMarkup(...);              // Renders svg
    public string ToDataUri(...);                   // base64 data URI
}
public class CustomIcon : Icon { }                  // Loader
public class IconFromImage : Icon { }               // img alt
public enum IconSize   { Size10, Size12, Size16, Size20, Size24, Size28, Size32, Size48, Custom }
public enum IconVariant{ Regular, Filled, Light, Color }
```

### Estrutura gerada (auto-gerada)
```csharp
namespace Microsoft.FluentUI.AspNetCore.Components.Icons.Regular;
[ExcludeFromCodeCoverage]
public static partial class Size20 {
    public class AddCircle : Icon { public AddCircle() : base("AddCircle", IconVariant.Regular, IconSize.Size20, "<path d=.../>") { } }
    // 2871 icons
}
```

### Lookup via reflection
```csharp
[RequiresUnreferencedCode("This method requires dynamic access to code.")]
public static CustomIcon GetInstance(this IconInfo icon, bool? throwOnError = true) {
    var assembly = GetAssembly($"Microsoft.FluentUI.AspNetCore.Components.Icons.{icon.Variant}");
    var iconType = assembly.GetTypes()
                           .Where(i => i.BaseType == typeof(Icon))
                           .FirstOrDefault(i => i.FullName == 
                               $"...Icons.{icon.Variant}.Size{(int)icon.Size}+{icon.Name}");
    return new CustomIcon((Icon)Activator.CreateInstance(iconType));
}
```

### DRAWBACKS
- RequiresUnreferencedCode quebra AOT/trimming
- Reflection-based lookup lento
- Bundle full: ~21034 classes em 4 DLLs (~30 MB)
- Sem keywords/categories para search

## EMOJI CATALOG ARCHITECTURE (FOCO ESPECIAL)

```csharp
public class EmojiInfo {
    public string Name { get; init; }
    public EmojiSize Size { get; init; } = Size32;
    public EmojiGroup Group { get; init; } = Activities;
    public EmojiSkintone Skintone { get; init; } = Default;
    public EmojiStyle Style { get; init; } = Color;
    public string KeyWords { get; init; } = "";
}
public class Emoji : EmojiInfo {
    private byte[] Data { get; }                    // GZip-compressed SVG bytes
    public string Content => _content ??= EmojiCompress.Unzip(Data);  // Lazy decode
    public MarkupString ToMarkup(...);
}
public enum EmojiSize    { Size16, Size32, Size48 }
public enum EmojiGroup   { Activities, AnimalsNature, Flags, FoodDrink, Objects, PeopleBody, SmileysEmotion, Symbols, TravelPlaces }
public enum EmojiSkintone{ Default, Light, MediumLight, Medium, MediumDark, Dark }
public enum EmojiStyle   { Color, Flat, HighContrast }
```

### Vantagens da Compressão GZip
- SVG emoji = 5-20 KB, GZip reduz 70-80%
- Lazy decode apenas na 1a vez
- Cache automatico em `_content`

## Generator Tool (AssetsGenerator)
CLI args: --assets (-a), --target (-t), --namespace (-ns), --sizes (-s), --names (-n), --library (-l), --help (-h)
1. ReadAllAssets() enumera *.svg em AssetsFolder
2. Parse filename pattern (name_size_variant.svg)
3. Para cada (variant, size): gera {Variant}{Size}.cs
4. Cada SVG = 1 class C# derivada de Icon/Emoji

## Componentes Principais (Core)
- FluentIcon.razor generic (Icon : Icon, new())
- FluentEmoji.razor generic (Emoji : Emoji, new())
- CssBuilder/StyleBuilder (fluent API)
- Debouncer (System.Timers)
- LibraryConfiguration (DI)
- DesignToken.Generator (Roslyn SG para Figma tokens)

## Padrões Comparados (6-way)
| Padrão | FluentUI | XForge | Status |
|--------|----------|--------|--------|
| Multi-TFM | net8/9/10 | net8/9/10 | OK |
| Central Pkg Mgmt | SIM | pendente (T015) | parcial |
| Icon catalog | 21034 classes | 3698 Dictionary | upgrade |
| Emoji catalog | 7885 GZip | 0 (nao tem) | create |
| Source Generators | 1 (DesignToken) | 0 (T076 = Bootstrap) | upgrade |
| Asset Generator CLI | SIM | NAO | create T080 |
| bUnit tests | 1.38.5 | 2.3.4 | upgrade T052 |

## Decisões Arquiteturais a aplicar no XForge
1. 1 SVG = 1 class tipada (AOT-friendly, IntelliSense)
2. Pacote separado por variant (Icons.Regular.dll)
3. CoreIcons embedded no Core
4. Generator tool versionado junto com lib
5. Compressao GZip para emojis
6. MarkupString direto (sem SVG lib externa)
7. Color tokens via Figma (DesignToken SG)
8. Skin tone para PeopleBody
