# Decisoes Tecnicas — Ant Design Blazor Adoption (consolidado)

## D001: Adotar ParameterState<T> (origem MudBlazor)
- **Contexto**: XForge atual usa fields + setters; sem change handler centralizado
- **Opcoes**: A) Manter status quo, B) Mudar para ParameterState<T>, C) ReactiveUI
- **Escolha**: B
- **Rationale**: Padrao comprovado (10.4k-star lib), elimina BL0005
- **Tradeoffs**: Maior boilerplate no constructor
- **Esforco**: 16h (T001)

## D002: Criar XForge.DesignSystem.Analyzers (origem MudBlazor)
- **Contexto**: Sem static analysis; DocsQualityTests pega gaps em runtime
- **Opcoes**: A) Manter manual, B) Roslyn analyzers
- **Escolha**: B
- **Rationale**: Feedback em compile-time, distribuivel como NuGet
- **Esforco**: 24h (T002)

## D003: Migrar build de assets para Bun (origem MudBlazor)
- **Contexto**: Bundling JS/SCSS manual
- **Opcoes**: A) Manter, B) Bun, C) esbuild
- **Escolha**: B
- **Esforco**: 8h (T003)

## D004: Adotar DomEventService pattern (origem antd-blazor)
- **Contexto**: Componentes XForge usam inline EventCallback + JS interop repetido
- **Opcoes**: A) Manter inline, B) MudEventCallback, C) DomEventService (antd style)
- **Escolha**: C
- **Rationale**: Centraliza DOM listeners; remove boilerplate; melhor testabilidade
- **Tradeoffs**: Mais uma dependencia DI; aprender o service
- **Esforco**: 12h (T004)
- **Risco**: Baixo (Service eh opcional, EventCallback continua funcionando)

## D005: Localizar XML docs em 6 idiomas (origem antd-blazor)
- **Contexto**: Sem Intellisense localizada; 80% usuarios brasileiros
- **Opcoes**: A) Manter en only, B) pt-BR only, C) 6 idiomas (en/pt-BR/es/zh/ja/ru)
- **Escolha**: C
- **Rationale**: Mercado brasileiro (pt-BR) + expansao internacional
- **Tradeoffs**: 6x traducoes para manter; precisa-se processo de i18n
- **Esforco**: 24h (T005)
- **Risco**: Medio (processo de traducao, commits de trans)

## D006: Multi-targeting comecar conservador (net8/9/10)
- **Contexto**: antd-blazor target 7 TFMs (netstd2.1 + net5-10); complexidade alta
- **Opcoes**: A) 3 TFMs (net8/9/10), B) 7 TFMs (igual antd)
- **Escolha**: A
- **Rationale**: LTS net8 + atuais; cobre 95% audiencia
- **Tradeoffs**: Perde netstandard2.1 (legacy), net5/6/7 (obsoletos)
- **Esforco**: 2h (T016)

## D007: Manter prefixo XF (nao copiar Mud nem Ant)
- **Contexto**: Decisao de marca
- **Opcoes**: A) Mud, B) Ant, C) XF
- **Escolha**: C
- **Rationale**: Brand identity XForge consolidado
- **Esforco**: 4h (T015)

## D008: Adotar TestKit (separar test infra) (origem antd-blazor)
- **Contexto**: XForge tem 2 test projects (Tests + DocsQualityTests) misturando
- **Opcoes**: A) Manter, B) TestApp+TestKit+Tests (antd), C) UnitTests+Viewer+Shared (Mud)
- **Escolha**: C (padrao Mud, mais maduro)
- **Rationale**: Viewer = Razor test components; Shared = helpers; UnitTests = bUnit puro
- **Esforco**: 16h (T020)

## D009: MCP server para docs (origem antd-blazor)
- **Contexto**: Docs sao estaticas; AI agents nao conseguem query
- **Opcoes**: A) Manter, B) MCP server, C) RAG sobre docs
- **Escolha**: B
- **Rationale**: Padrao emergente (PR #4758 antd-blazor), simples de implementar
- **Tradeoffs**: Adiciona complexidade de deployment
- **Esforco**: 32h (T028)
- **Risco**: Medio (MCP ainda emergente)

## D010: JSON-based locales (origem antd-blazor)
- **Contexto**: XForge sem localization; resx eh .NET-centric
- **Opcoes**: A) resx (.NET default), B) JSON, C) YAML, D) TOML
- **Escolha**: B
- **Rationale**: Universal, editavel em qualquer editor, version-control friendly
- **Tradeoffs**: Sem tooling de designer de .resx
- **Esforco**: 8h (T019)

## D011: Adotar DraftMonitor component (origem antd-blazor)
- **Contexto**: Forms complexos precisam de draft state mgmt
- **Opcoes**: A) Implementar manualmente por form, B) DraftMonitor service
- **Escolha**: B
- **Rationale**: Reutilizavel; padrao testado
- **Esforco**: 8h (T030)

## D012: OneOf para param union types (origem antd-blazor)
- **Contexto**: Alguns params aceitam tipos multiplos (string | enum | number)
- **Opcoes**: A) object (perde type safety), B) OneOf<T1, T2, ...>
- **Escolha**: B
- **Rationale**: Type-safe; valido em compile-time
- **Tradeoffs**: Adiciona dep OneOf
- **Esforco**: 4h (T018)

## D013: CallAfterRender queue (origem antd-blazor)
- **Contexto**: JS interop em OnAfterRenderAsync as vezes eh problematico (re-render)
- **Opcoes**: A) Inline await, B) Queue + flush pos render
- **Escolha**: B (para casos especificos)
- **Rationale**: Reduz erro em interop
- **Esforco**: 6h (T017)

## D014: AntDesign.targets (origem antd-blazor)
- **Contexto**: Consumers do package precisam copiar config
- **Opcoes**: A) Manual config, B) MSBuild targets auto-included
- **Escolha**: B
- **Rationale**: Zero-config para consumidor
- **Esforco**: 4h (T027)

## D015: Manter 3-file pattern XForge (nao migrar para hierarchy profunda)
- **Contexto**: XForge usa 3-file (.razor / .razor.cs / .types.cs); Mud usa inheritance
- **Opcoes**: A) Manter 3-file, B) Migrar para MudBaseInput/AntInputComponentBase
- **Escolha**: A
- **Rationale**: Mais clean; menos inheritance depth
- **Esforco**: 0h (decisao de nao migrar)
