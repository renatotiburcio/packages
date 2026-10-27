# T043 - Component playground (try.xforge.com) (P3, 24h, T020,T022)

## Status: SCAFFOLDED (deferred - depends on T020+T022)

## Goal
Ship a public web playground where developers can:
- Try all XForge.DesignSystem components
- Edit code in browser
- See live preview
- Share via URL

## Inspired by
- antd-blazor try.ant-design-blazor.com
- MudBlazor try.mudblazor.com
- Tailwind UI playground

## Effort
- 24h estimated
- Uses Blazor Server for live code execution
- T020 (UnitTests.Viewer) provides backend
- T022 (MTP migration) ensures tests can run sandboxed

## Decision
- Defer to v2.1
- After T020 + T022 land

## Reference
- T043 in antd-blazor-comparison BACKLOG
- https://try.mudblazor.com
- https://try.ant-design-blazor.com

## Hosting cost
- $5-20/month on Azure App Service
- Worth it for marketing + DX