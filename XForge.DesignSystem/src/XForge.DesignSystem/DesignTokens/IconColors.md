# Icon Color Tokens (T083)

## Semantic colors

| Token | Light | Dark | CSS var |
|-------|-------|------|---------|
| Accent | #0078d4 | #2899f5 | `--xf-color-accent` |
| Lightweight | #605e5c | #c8c6c4 | `--xf-color-text-secondary` |
| Success | #107c10 | #54b054 | `--xf-color-success` |
| Warning | #ffb900 | #ffb900 | `--xf-color-warning` |
| Error | #d13438 | #e81123 | `--xf-color-error` |
| Important | #8a00c2 | #b146ee | `--xf-color-important` |

## Usage

```razor
<XFFluentIcon Value="..." Color="Success" />
<XFFluentIcon Value="..." Color="Custom" CustomColor="#ff0080" />
```