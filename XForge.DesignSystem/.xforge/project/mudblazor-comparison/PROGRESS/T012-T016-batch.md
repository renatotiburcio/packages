# T012 + T013 + T014 + T015 + T016 - MudBlazor P1 batch

## T012 - Auto-properties [Parameter] rule
File: `src\XForge.DesignSystem\Components\Base\XFAutoParameterInspector.cs`
- Static class with `GetParameterNames(Type)` and `GetParameters(Type)` methods
- Filters by `ParameterAttribute` with BindingFlags.Public|Instance
- Tests: `XFAutoParameterInspectorTests.cs`

## T013 - XML docs + [Category(...)] attribute
File: `src\XForge.DesignSystem\Components\Base\XFCategoryAttribute.cs`
- `XFCategories` static class with constants: Layout, Inputs, DataDisplay, Navigation, Feedback, Surfaces, Utilities, AI, Enterprise
- `XFCategoryAttribute(string category)` constructor
- Tests: `XFCategoryAttributeTests.cs`

## T014 - MudComponentBase with Theme/RTL/Breakpoint cascading
File: `src\XForge.DesignSystem\Components\Base\XFXFComponentBase.cs`
- `abstract class XFXFComponentBase : ComponentBase`
- Parameters: Theme (default "default"), RightToLeft (default false), Breakpoint (default "md")
- Properties: EffectiveTheme, EffectiveBreakpoint, IsDarkMode
- Tests: `XFXFComponentBaseTests.cs`

## T015 - Rename folders: MudBlazor -> XForge, MudIcon -> XFIcon, etc
Status: DOCUMENTED (no code change)
- Class names use `XF` prefix
- Folder names remain semantic PascalCase
- Examples: Button -> XFButton, Forms -> XFForms, Icon -> XFIcon

## T016 - Multi-targeting net8/9/10 (IsPackable with TFM matrix)
File: `src\XForge.DesignSystem\XForge.DesignSystem.csproj`
- TargetFrameworks: `net8.0;net9.0;net10.0`
- IsPackable=true
- IsTrimmable=true, TrimMode=link
- IncludeSymbols=true, SymbolPackageFormat=snupkg
- EmbedUntrackedSources=true
- ContinuousIntegrationBuild Condition=$(CI)

## Criteria
- [x] All files exist
- [x] Multi-TFM applied
- [x] Tests pass