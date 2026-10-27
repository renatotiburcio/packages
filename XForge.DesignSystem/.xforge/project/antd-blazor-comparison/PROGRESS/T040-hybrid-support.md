# T040 - Hybrid support (MAUI/WPF/WinForms) (P3, 40h, T016)

## Status: SCAFFOLDED (deferred - large effort)

## Goal
Enable XForge.DesignSystem to render in MAUI, WPF, and WinForms apps via Blazor Hybrid.

## Approach
- Reference MAUI Hybrid sample
- Reuse BlazorWebView for cross-platform rendering
- Conditional compilation for non-web JS interop (use platform-specific code)

## Constraints
- 40h estimated (real effort, multi-week)
- antd-blazor chose 7 TFMs including net6.0-windows, net6.0-maui, etc.
- T016 sets up the multi-TFM basis (net8.0;net9.0;net10.0)

## Reference
- T040 in antd-blazor-comparison BACKLOG
- Inspired by antd-blazor Hybrid targets

## Decision
- Defer to v2.1 (post v2.0 stable release)
- v2.0 ships net8.0+net9.0+net10.0 only