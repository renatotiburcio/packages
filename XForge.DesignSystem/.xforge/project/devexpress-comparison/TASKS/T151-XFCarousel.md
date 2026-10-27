---
id: T151
title: XFCarousel
priority: P2
status: pending
estimated_time: 8h
dependencies: T001
---

# T151: XFCarousel

## Objetivo
Slideshow carousel com autoplay, pagination, swipe e transicoes.

## Contexto
- Source: https://www.devexpress.com/blazor/
- Consolidation: 9-way
- LGPD: Re-implementa padrao DevExpress DevExpress SEM dependencia comercial

## Descricao
XFCarousel oferece slideshow de imagens/conteudo com autoplay (pausable on hover), pagination dots, prev/next buttons, swipe gestures (mobile), loop mode, transicoes slide/fade e acessibilidade WCAG.

## Passos
1. Estudar API DevExpress Carousel no docs.devexpress.com
2. Definir modelo XForge (XFCarousel, CarouselItem, CarouselTransition)
3. Implementar autoplay com pause on hover/focus
4. Implementar swipe via touch events
5. Criar XFDemo page com 5+ exemplos
6. Escrever testes xUnit (10+ tests)
7. Documentar com XML comments + DOCS-RAZOR-PAGE-STANDARD

## Arquivos
- Criar: src/XForge.DesignSystem/Components/T151/XFCarousel.razor + .razor.cs
- Criar: src/XForge.DesignSystem/Components/T151/CarouselItem.cs
- Criar: src/XForge.DesignSystem/Components/T151/CarouselTransition.cs
- Criar: src/XForge.DesignSystem/Components/T151/wwwroot/carousel.js
- Criar: src/XForge.DesignSystem.Tests/T151CarouselTests.cs
- Criar: docs/components/t151-carousel/index.md

## Criterios de Aceite
- [ ] Compila sem warnings
- [ ] 10+ testes xUnit passando
- [ ] XFDemo page com 5+ exemplos
- [ ] XML comments completos
- [ ] A11y: WCAG 2.1 AA (ARIA roledescription carousel, pause control)
- [ ] Cobertura > 85%
- [ ] LGPD compliant
- [ ] Multi-TFM compila net8/net9/net10

## Notas
- LGPD Art. 46/50: NAO usar pacotes NuGet comerciais DevExpress
- Inspirar-se no PADRAO/API DevExpress
- 9-way consolidation: 9a fonte do roadmap