# Bun Asset Pipeline

XForge.DesignSystem uses [Bun](https://bun.sh) as the unified asset pipeline for TypeScript + SCSS bundling.

## Why Bun?

- 10-100x faster than npm scripts + webpack
- Native TypeScript support (no ts-node, no transpile step)
- Native test runner (`bun test`)
- Drop-in replacement for Node.js

## Scripts

- `bun run build` — One-shot build (TS + SCSS)
- `bun run dev` — Watch mode
- `bun test` — Run unit tests
- `bun run clean` — Clean `wwwroot/dist`

## Why antd-blazor chose this pattern

antd-blazor migrated from npm + rollup to Bun in 2025, reducing build times from 60s to 4s. We adopt the same pattern for performance parity.

## Reference

- T003 in antd-blazor-comparison BACKLOG
- Inspired by MudBlazor Bun pipeline