# XForge DesignSystem — Docs Quality Test Project

## Project: XForge.DesignSystem.Docs.QualityTests
- Purpose: Automated quality tests for documentation pages
- Framework: xUnit 2.9.2
- References: XForge.DesignSystem (via ProjectReference)

## Infrastructure Classes

### ComponentApiCatalog
Discovers all public Blazor components via reflection:
- Scans for ComponentBase subclasses with [Parameter] properties
- Extracts: Parameters, CascadingParameters, EventCallbacks, RenderFragments, PublicMethods, RelatedEnums, RelatedTypes
- Checks: IDisposable, IAsyncDisposable implementation

### DocsFileCatalog
Discovers all *Docs.razor and *Overview.razor pages via filesystem scan:
- Extracts: FileName, FullPath, RelativePath, Route, ComponentName, Content, LineCount

### PathResolver
Resolves project paths for test infrastructure.

### DocumentationContract
Defines required sections and accessibility terms for validation.

### AllowedDocsExceptions
Exceptions for components that don't need their own docs page.

## Test Files (12 test classes)

### 1. DocsPageExistenceTests
Every public XF component should have a corresponding *Docs.razor page.

### 2. DocsAccessibilityCoverageTests
Interactive components should document accessibility and keyboard (min 3 a11y terms).

### 3. DocsPublicApiCoverageTests
Validates:
- Every public parameter is documented
- Every EventCallback is documented
- Every RenderFragment is documented
- Every CascadingParameter is documented
- Every public @ref method is documented
- Every related public enum is documented

### 4. DocsRequiredSectionsTests
Every docs page should contain all required sections (aliases checked).

### 5. DocsGuardrailTests
Public API changes should require docs contract update. Uses JSON snapshot comparison:
- BuildCurrentSnapshot() → ComponentSnapshot (Parameters, EventCallbacks, RenderFragments, PublicMethods, RelatedEnums)
- Compares against saved snapshot
- Auto-updates snapshot on change
- Fails test if API changed without docs update

### 6. DocsChangelogPatternTests
Changelog pattern validation.

### 7. DocsDesignSystemUsageTests
XForge components used in docs (not raw HTML).

### 8. DocsExamplesTests
Examples are rendered (not code-only).

### 9. DocsKeyboardCoverageTests
Keyboard section exists for interactive components.

### 10. DocsOverviewPagePatternTests
Overview pages follow pattern.

### 11. DocsPerformanceCoverageTests
Performance section exists.

### 12. DocsRequiredSectionsTests
Required sections validation.

## Descriptor Types
- ComponentDescriptor: Name, FullName, Namespace, Category, Parameters, CascadingParameters, EventCallbacks, RenderFragments, PublicMethods, RelatedEnums, RelatedTypes, HasIdisposable, HasIAsyncDisposable
- ParameterDescriptor: Name, TypeName, IsRequired, HasUnmatchedValues, IsRenderFragment, IsEventCallback
- CascadingParameterDescriptor: Name, TypeName, IsPublic
- EventCallbackDescriptor: Name, ValueType
- RenderFragmentDescriptor: Name, IsTyped, ContextType
- PublicMethodDescriptor: Name, ReturnType, Parameters
- EnumDescriptor: Name, Values
- DocsPageDescriptor: FileName, FullPath, RelativePath, Route, ComponentName, Content, LineCount
