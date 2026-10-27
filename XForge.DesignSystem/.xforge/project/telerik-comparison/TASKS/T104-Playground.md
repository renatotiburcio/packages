---
id: T104
title: Playground/REPL (Monaco+Roslyn+permalink)
priority: P0
status: done
estimated_time: 24h
dependencies: [T102]
---

## Objetivo
REPL in-browser com Monaco editor + Roslyn compile + share permalink.

## Passos
1. Monaco editor embed
2. Roslyn workspace in-browser
3. Share via permalink (base64)
4. Real-time compile + render

## Criterios de Aceite
- Code compila in-browser
- Permalink roundtrip
- ~30MB bundle