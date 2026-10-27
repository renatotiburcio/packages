# T015 - Rename Folders (XF prefix)

## Status: COMPLETE (documentation only)

## Convention
All XForge.DesignSystem folders use the `XF` prefix to avoid namespace conflicts with consumer projects.

| Old | New | Reason |
|-----|-----|--------|
| Components/Button | Components/XFButton | Namespace alignment |
| Components/Forms | Components/XFForms | Namespace alignment |
| Components/Icon | Components/XFIcon | Namespace alignment |
| (already applied) | | |

## Rule
- All public classes/components: `XF` prefix (e.g. `XFButton`, `XFInput`)
- All namespaces: `XForge.Components.XF*`
- All folder names: PascalCase, no `XF` prefix in folder itself (only on classes)

## Reference
- T015 in antd-blazor-comparison BACKLOG
- T015 in MudBlazor comparison BACKLOG
- Convention adopted from MudBlazor