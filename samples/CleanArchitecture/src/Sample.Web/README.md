# Sample.Web — front Blazor 100% XForge.DesignSystem

Front de demonstração do sample Clean Architecture. Todo o shell e todas as telas usam
exclusivamente componentes do `XForge.DesignSystem`.

## Rodar

```bash
dotnet run --project src/Sample.Web     # http://localhost:5xxx
```

Um único processo: o front consome a camada `Sample.Application` pelo `ISender` do Mediator,
no mesmo processo. Nada de CORS, nada de segundo projeto no ar.

## O shell

`Components/Layout/MainLayout.razor` monta o mesmo shell do manual de componentes:

```razor
<XFAppShell Theme="@Theme" Brand="@CurrentBrand">
    <Rail>       <SampleRail />    </Rail>
    <AppBar>     <SampleAppBar />  </AppBar>
    <Footer>     <XFFooter ... />  </Footer>
    <ChildContent>@Body</ChildContent>
</XFAppShell>
```

- **Rail** (`XFRail`): dois itens `RailNavItem` — *Clientes* (`/clientes`) e *Arquitetura*
  (`/arquitetura`). O item ativo é rastreado pelo `NavigationStateService`, que persiste a
  marca em `localStorage`.
- **AppBar**: título da marca ativa + alternância de tema claro/escuro via `xfTheme`.
- **Rodapé**: `XFFooter`.
- **Providers** no `ChildContent`: `XFDialogProvider`, `XFDrawerProvider`, `XFToastProvider`.

## Telas

| Rota | Componentes XForge usados |
|---|---|
| `/clientes` | `XFDataGrid` (+ `XFDataGridColumn`), `XFSkeletonTable`, `XFAlert`, `XFCard`, `XFButton` |
| `/clientes/novo` | `XFTextInput`, `XFButton`, `XFAlert`, `XFCard`, `XFBreadcrumb`, `XFToastService` |
| `/clientes/{id}` | `XFCard`, `XFBreadcrumb`, `XFCode`, `XFAlert`, `XFSkeleton` |
| `/arquitetura` | `XFSteps`, `XFTable`, `XFCard`, `XFCode`, `XFDivider` |

O `XFDataGrid` tem seletor de colunas, paginação, listras e realce no hover. O clique na
linha navega para o detalhe (`RowClicked`).

## Onde a validação acontece

O formulário **não** duplica as regras em `DataAnnotations`. Ele envia o
`CriarClienteCommand` pelo pipeline; o `ValidationBehavior` (XForge.Mediator.Behaviors) roda
o `CriarClienteValidator` (XForge.Validation) e barra o handler antes dele. As falhas voltam
na `ValidationException` e são renderizadas no `XFAlert` + `XFToast`.

## Camadas

```
Clientes.razor  ──► ISender.Send(ObterClientesQuery) ──► ObterClientesHandler ──► IClienteRepository
ClienteNovo.razor ──► ISender.Send(CriarClienteCommand) ──► ValidationBehavior ──► CriarClienteHandler
```

`Cliente -> ClienteDto` é mapeado em compile-time pelo `XForge.Mapper.SourceGenerator`
(`[XForgeMap] ClienteMappings`).
