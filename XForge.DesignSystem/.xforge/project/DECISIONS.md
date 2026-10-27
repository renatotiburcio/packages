# Decisões Técnicas — XForge DesignSystem

## D001: XForge.MediatR obrigatório
- **Contexto**: Escolha de mediator pattern para CQRS
- **Opções**: MediatR oficial, XForge.MediatR, custom mediator
- **Escolha**: XForge.MediatR
- **Rationale**: Projeto exige XForge.MediatR por padrão (Golden Rule 3)
- **Tradeoffs**: Sem compatibilidade com pacote MediatR oficial

## D002: AutoMapper obrigatório
- **Contexto**: Mapeamento DTO/entity
- **Opções**: AutoMapper, Mapster, manual mapping
- **Escolha**: AutoMapper
- **Rationale**: Golden Rule exige AutoMapper
- **Tradeoffs**: Performance ligeiramente inferior a Mapster

## D003: Tailwind CSS v4 com tokens CSS
- **Contexto**: Sistema de design tokens para temas
- **Opções**: CSS puro, CSS Modules, Tailwind v3, Tailwind v4
- **Escolha**: Tailwind v4 + CSS variables
- **Rationale**: Utility-first + tokens = flexibilidade máxima
- **Tradeoffs**: Curva de aprendizado, bundle size

## D004: BOM-safe UTF-8 encoding
- **Contexto**: Arquivos gerados por subagentes tinham encoding incorreto
- **Opções**: UTF8 sem BOM, UTF8 com BOM, ASCII
- **Escolha**: UTF8 com BOM (`UTF8Encoding($true)`)
- **Rationale**: Compilador C# exige BOM em alguns cenários
- **Tradeoffs**: Git diff pode mostrar BOM como mudança

## D005: Testes com bUnit + BunitContext
- **Contexto**: Framework de testes para Blazor
- **Opções**: bUnit, Playwright, Selenium, xUnit direto
- **Escolha**: bUnit com `BunitContext` base class
- **Rationale**: Padrão da comunidade Blazor, sem overhead de browser
- **Tradeoffs**: Não testa renderização visual real

## D006: Documentação interativa em Blazor
- **Contexto**: Documentação de componentes
- **Opções**: Markdown estático, DocFX, Blazor interativo, Storybook
- **Escolha**: Blazor Server com live demos
- **Rationale**: Exemplos reais executáveis, não mockados
- **Tradeoffs**: Requer servidor para renderização

## D007: AOT-ready com UnconditionalSuppressMessage
- **Contexto**: Warnings IL2026/IL3050 em reflection
- **Opções**: JsonSerializerContext, suppress, evitar reflection
- **Escolha**: `[UnconditionalSuppressMessage("Trimming", "IL2xxx")]`
- **Rationale**: Preserva funcionalidade sem warnings
- **Tradeoffs**: Requer análise manual para cada caso

## D008: CPM strict em Directory.Packages.props
- **Contexto**: Gerenciamento de pacotes NuGet
- **Opções**: PackageReference por projeto, Central Package Management
- **Escolha**: CPM strict com versão fixa por pacote
- **Rationale**: Consistência, evita conflitos de versão
- **Tradeoffs**: Menos flexibilidade por projeto
