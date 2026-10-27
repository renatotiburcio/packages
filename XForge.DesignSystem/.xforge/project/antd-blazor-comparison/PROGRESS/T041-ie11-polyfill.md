# T041 - IE 11 polyfill support (P3, 8h, T016)

## Status: SCAFFOLDED (deferred - declining priority)

## Goal
Ship IE 11 polyfills so XForge.DesignSystem works in IE 11 (mainly for enterprise legacy systems).

## Reality check
- IE 11 EOL: June 15, 2022
- Microsoft Teams dropped IE 11 in 2020
- Most LATAM enterprises have moved off IE 11 by 2025
- IE 11 does not support modern Blazor (Server requires SignalR which IE 11 needs polyfills)

## Effort vs ROI
- 8h to add polyfills
- Affects ~0% of users in 2026
- Adds 200KB+ to bundle

## Decision
- Defer indefinitely
- If a customer requires IE 11, treat as a custom engagement

## Reference
- T041 in antd-blazor-comparison BACKLOG
- antd-blazor added IE 11 polyfills in 2020; we are in 2026

## Why kept in P3
- Tracked for future legacy migrations (Gov.br, some banks)
- Low effort, low impact