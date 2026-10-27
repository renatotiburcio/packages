# XForge Design System - Internationalization (i18n) Guide

## Overview

XForge Design System supports internationalization through the `IXFLocalizer` interface and JSON-based locale files. All UI strings can be localized by providing translated values in locale files.

## Architecture

```
IXFLocalizer (interface)
  └── XFLocalizer (implementation)
       └── XFJsonLocaleProvider (loads from embedded JSON resources)
            └── Localization/Json/Locales/{culture}.json
```

### Default Locales

| Locale | File | Language |
|--------|------|----------|
| `en` | `en.json` | English (default) |
| `pt-BR` | `pt-BR.json` | Portuguese (Brazil) |
| `es` | `es.json` | Spanish |

## Usage

### 1. Inject the Localizer

```csharp
[Inject] private IXFLocalizer? Localizer { get; set; }
private string L(string key, string fallback) => Localizer?[key] ?? fallback;
```

### 2. Localize in OnInitialized

```csharp
protected override void OnInitialized()
{
    EmptyText = L("datagrid.empty", EmptyText);
    base.OnInitialized();
}
```

### 3. Localize in RenderTreeBuilder

```csharp
builder.AddContent(7, L("datagrid.filter_all", "All"));
```

## Available Keys

### Button (`button.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `button.ok` | OK | OK | OK |
| `button.cancel` | Cancel | Cancelar | Cancelar |
| `button.save` | Save | Salvar | Guardar |
| `button.edit` | Edit | Editar | Editar |

### Dialog (`dialog.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `dialog.confirm` | Are you sure? | Tem certeza? | Esta seguro? |
| `dialog.move` | Move dialog | Mover dialogo | Mover dialogo |

### DataGrid (`datagrid.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `datagrid.empty` | No records found. | Nenhum registro encontrado. | Ningun registro encontrado. |
| `datagrid.columns` | Columns | Colunas | Columnas |
| `datagrid.show_format` | Show {0} | Mostrar {0} | Mostrar {0} |
| `datagrid.hide_format` | Hide {0} | Ocultar {0} | Ocultar {0} |
| `datagrid.move_left_format` | Move {0} left | Mover {0} para esquerda | Mover {0} a la izquierda |
| `datagrid.move_right_format` | Move {0} right | Mover {0} para direita | Mover {0} a la derecha |
| `datagrid.decrease_width_format` | Decrease width of {0} | Reduzir largura de {0} | Reducir ancho de {0} |
| `datagrid.increase_width_format` | Increase width of {0} | Aumentar largura de {0} | Aumentar ancho de {0} |
| `datagrid.edit_actions` | Actions | Acoes | Acciones |
| `datagrid.edit` | Edit | Editar | Editar |
| `datagrid.save` | Save | Salvar | Guardar |
| `datagrid.cancel` | Cancel | Cancelar | Cancelar |
| `datagrid.sort_ascending_format` | Sort {0} ascending | Ordenar {0} crescente | Ordenar {0} ascendente |
| `datagrid.sort_descending_format` | Sort {0} descending | Ordenar {0} decrescente | Ordenar {0} descendente |
| `datagrid.clear_sort_format` | Clear sort of {0} | Remover ordenacao de {0} | Limpiar orden de {0} |
| `datagrid.no_group` | No group | Sem grupo | Sin grupo |
| `datagrid.aggregate_total` | Total: | Total: | Total: |
| `datagrid.aggregate_average` | Average: | Media: | Promedio: |
| `datagrid.aggregate_count` | Count: | Qtd: | Cantidad: |
| `datagrid.aggregate_min` | Min: | Min: | Min: |
| `datagrid.aggregate_max` | Max: | Max: | Max: |
| `datagrid.export_csv` | Export CSV | Exportar CSV | Exportar CSV |
| `datagrid.export_excel` | Export Excel | Exportar Excel | Exportar Excel |
| `datagrid.filter_all` | All | Todos | Todos |
| `datagrid.filter_yes` | Yes | Sim | Si |
| `datagrid.filter_no` | No | Nao | No |
| `datagrid.filter_placeholder` | Filter... | Filtrar... | Filtrar... |

### Layout/Shell (`rail.*`, `sidebar.*`, `appshell.*`, `appbar.*`, `toolbar.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `rail.navigation` | Main navigation | Navegacao principal | Navegacion principal |
| `sidebar.navigation` | Secondary navigation | Navegacao secundaria | Navegacion secundaria |
| `appshell.main_content` | Main content | Conteudo principal | Contenido principal |
| `appshell.navigation` | Application navigation | Navegacao do aplicativo | Navegacion de la aplicacion |
| `appbar.label` | Application bar | Barra do aplicativo | Barra de la aplicacion |
| `toolbar.label` | Toolbar | Barra de ferramentas | Barra de herramientas |

### Wizard (`wizard.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `wizard.title` | Step wizard | Assistente de etapas | Asistente de pasos |
| `wizard.back` | Back | Voltar | Atras |
| `wizard.next` | Continue | Continuar | Continuar |
| `wizard.finish` | Finish | Finalizar | Finalizar |

### Navigation (`toc.*`, `contextmenu.*`, `pagination.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `toc.empty` | No sections to display. | Nenhuma secao para exibir. | No hay secciones para mostrar. |
| `contextmenu.label` | Context menu | Menu de contexto | Menu de contexto |
| `pagination.label` | Pagination | Paginacao | Paginacion |
| `pagination.page_size` | Items per page | Itens por pagina | Elementos por pagina |
| `pagination.first` | First page | Primeira pagina | Primera pagina |
| `pagination.prev` | Previous page | Pagina anterior | Pagina anterior |
| `pagination.next` | Next page | Proxima pagina | Proxima pagina |
| `pagination.last` | Last page | Ultima pagina | Ultima pagina |
| `pagination.reload` | Reload current page | Recarregar pagina actual | Recargar pagina actual |
| `pagination.current_format` | Page {0}, current | Pagina {0}, atual | Pagina {0}, actual |
| `pagination.goto_format` | Go to page {0} | Ir para pagina {0} | Ir a pagina {0} |
| `pagination.range_format` | {0}-{1} of {2} | {0}-{1} de {2} | {0}-{1} de {2} |
| `pagination.empty` | 0 items | 0 itens | 0 elementos |

### Sortable (`sortable.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `sortable.move_up` | Move up | Mover para cima | Mover arriba |
| `sortable.move_down` | Move down | Mover para baixo | Mover abajo |

### Validation (`validation.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `validation.valid` | Form is valid | Formulario valido | Formulario valido |
| `validation.invalid` | Form contains errors | Formulario contem erros | El formulario contiene errores |
| `validation.neutral` | Awaiting validation | Aguardando validacao | Esperando validacion |
| `validation.status` | Validation status | Status da validacao | Estado de validacion |

### TransferList (`transferlist.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `transferlist.available` | Available items | Itens disponiveis | Elementos disponibles |
| `transferlist.selected` | Selected items | Itens selecionados | Elementos seleccionados |
| `transferlist.move_right` | Move right | Mover para direita | Mover a la derecha |
| `transferlist.move_left` | Move left | Mover para esquerda | Mover a la izquierda |
| `transferlist.move_all_right` | Move all right | Mover todos para direita | Mover todos a la derecha |
| `transferlist.move_all_left` | Move all left | Mover todos para esquerda | Mover todos a la izquierda |
| `transferlist.search` | Search... | Buscar... | Buscar... |
| `transferlist.move_selected_right` | Move selected right | Mover selecionados para direita | Mover seleccionados a la derecha |
| `transferlist.move_selected_left` | Move selected left | Mover selecionados para esquerda | Mover seleccionados a la izquierda |

### SignaturePad (`signaturepad.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `signaturepad.label` | Signature field | Campo de assinatura | Campo de firma |
| `signaturepad.instruction` | Sign using mouse, pen, or touch. | Assine usando mouse, caneta ou toque. | Firme usando mouse, lapiz o tactil. |
| `signaturepad.clear` | Clear | Limpar | Limpiar |
| `signaturepad.undo` | Undo | Desfazer | Deshacer |
| `signaturepad.required` | Signature required. | Assinatura obrigatoria. | Firma obligatoria. |

### Password (`password.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `password.enter` | Enter a password | Digite uma senha | Ingrese una contrasena |
| `password.uppercase` | One uppercase letter | Uma letra maiuscula | Una letra mayuscula |
| `password.lowercase` | One lowercase letter | Uma letra minuscula | Una letra minuscula |
| `password.number` | One number | Um numero | Un numero |
| `password.special` | One special character | Um caractere especial | Un caractere especial |
| `password.min_length` | At least 8 characters | No minimo 8 caracteres | Al menos 8 caracteres |
| `password.strong` | Strong | Forte | Fuerte |
| `password.medium` | Medium | Media | Media |
| `password.weak` | Weak | Fraca | Debil |

### EditForm (`editform.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `editform.unsaved` | Unsaved changes | Alteracoes nao salvas | Cambios no guardados |
| `editform.save_close` | Save and close | Salvar e fechar | Guardar y cerrar |
| `editform.back_continue` | Back and continue | Voltar e continuar | Volver y continuar |

### MultiSelect / Combobox (`multiselect.*`, `combobox.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `multiselect.no_options` | No options. | Nenhuma opcao. | Ninguna opcion. |
| `combobox.no_options` | No options. | Nenhuma opcao. | Ninguna opcion. |

### Chat / MessageList (`chat.*`, `messagelist.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `chat.placeholder` | Type your message | Digite sua mensagem | Escriba su mensaje |
| `chat.empty` | No messages yet. | Nenhuma mensagem ainda. | Ningun mensaje aun. |
| `messagelist.empty` | No messages yet. | Nenhuma mensagem ainda. | Ningun mensaje aun. |

### Data Display (`video.*`, `timeline.*`, `avatar.*`, `chip.*`, `badge.*`, `chart.*`, `menu.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `video.label` | Video media | Midia de video | Medio de video |
| `timeline.label` | Timeline | Linha do tempo | Linea del tiempo |
| `avatar.group` | User group | Grupo de usuarios | Grupo de usuarios |
| `avatar.online` | Available | Disponivel | Disponible |
| `avatar.away` | Away | Ausente | Ausente |
| `avatar.busy` | Busy | Ocupado | Ocupado |
| `avatar.offline` | Unavailable | Indisponivel | No disponible |
| `avatar.status` | Status | Status | Estado |
| `chip.dismiss` | Remove | Remover | Eliminar |
| `badge.dismiss` | Remove badge | Remover marcador | Eliminar insignia |
| `badge.dismiss_format` | Remove {0} | Remover {0} | Eliminar {0} |
| `chart.empty` | No data to display. | Nenhum dado para exibir. | Sin datos para mostrar. |
| `chart.series_name` | Serie {0} | Serie {0} | Serie {0} |
| `menu.empty` | No items to display. | Nenhum item para exibir. | Sin elementos para mostrar. |

### Scheduler / Gantt (`scheduler.*`, `gantt.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `scheduler.views` | Schedule views | Visualizacoes da agenda | Visualizaciones de la agenda |
| `scheduler.move` | Move appointment | Mover agendamento | Mover cita |
| `gantt.empty` | No tasks found | Nenhuma tarefa encontrada | Ninguna tarea encontrada |

### Rail Item (`railitem.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `railitem.label` | Navigation item | Item de navegacao | Elemento de navegacion |

### Select (`select.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `select.search` | Search... | Buscar... | Buscar... |

### Speech (`speech.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `speech.microphone_unavailable` | Microphone not available | Microfone nao disponivel | Microfono no disponible |

### Upload (`upload.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `upload.cancel` | Cancel | Cancelar | Cancelar |
| `upload.remove` | Remove | Remover | Eliminar |

### DropDownDataGrid (`dropdownDatagrid.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `dropdownDatagrid.empty` | No results. | Nenhum resultado. | Ningun resultado. |
| `dropdownDatagrid.search` | Search... | Buscar... | Buscar... |

### Autocomplete (`autocomplete.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `autocomplete.empty` | No results. | Nenhum resultado. | Ningun resultado. |

### Empty States (`virtuallist.*`, `treeview.*`, `table.*`, `pivotdatagrid.*`, `markdown.*`, `list.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `virtuallist.empty` | No items to display. | Nenhum item para exibir. | Ningun elemento para mostrar. |
| `treeview.empty` | No items to display. | Nenhum item para exibir. | Ningun elemento para mostrar. |
| `table.empty` | No data to display. | Nenhum dado para exibir. | Ningun dato para mostrar. |
| `pivotdatagrid.empty` | No data to display. | Nenhum dado para exibir. | Ningun dato para mostrar. |
| `markdown.empty` | No content. | Nenhum conteudo. | Sin contenido. |
| `list.empty` | No items to display. | Nenhum item para exibir. | Ningun elemento para mostrar. |

### Switch (`switch.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `switch.on` | Yes | Sim | Si |
| `switch.off` | No | Nao | No |

### Search (`search.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `search.placeholder` | Search | Pesquisar | Buscar |
| `search.clear` | Clear search | Limpar pesquisa | Limpiar busqueda |

### Input (`input.*`, `inputmask.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `input.clear` | Clear value | Limpar valor | Limpiar valor |
| `inputmask.clear` | Clear | Limpar | Limpiar |

### Popover (`popover.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `popover.open` | Open | Abrir | Abrir |

### SplashScreen (`splash.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `splash.loading` | Loading | Carregando | Cargando |
| `splash.loading_app` | Loading application | Carregando aplicacao | Cargando aplicacion |

### Progress (`progress.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `progress.loading` | Loading | Carregando | Cargando |

### FileInput (`fileinput.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `fileinput.choose` | Choose | Escolher | Elegir |
| `fileinput.drop_single` | Drop a file here or select | Solte um arquivo aqui ou selecione | Suelte un archivo aqui o seleccione |
| `fileinput.drop_multiple` | Drop files here or select | Solte arquivos aqui ou selecione | Suelte archivos aqui o seleccione |
| `fileinput.max_single_format` | Single file - max. {0} | Arquivo unico - max. {0} | Archivo unico - max. {0} |
| `fileinput.max_multiple_format` | Multiple files - max. {0} | Multiplos arquivos - max. {0} | Multiples archivos - max. {0} |
| `fileinput.too_large` | File exceeds maximum size. | Arquivo excede o tamanho maximo. | El archivo excede el tamano maximo. |
| `fileinput.uploading` | Uploading | Enviando | Enviando |
| `fileinput.uploaded` | Uploaded | Enviado | Enviado |
| `fileinput.error` | Error | Erro | Error |
| `fileinput.cancelled` | Cancelled | Cancelado | Cancelado |

### DatePicker (`datepicker.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `datepicker.open` | Open calendar | Abrir calendario | Abrir calendario |
| `datepicker.label` | Calendar | Calendario | Calendario |
| `datepicker.prev_month` | Previous month | Mes anterior | Mes anterior |
| `datepicker.next_month` | Next month | Proximo mes | Proximo mes |
| `datepicker.today` | Today | Hoje | Hoy |
| `datepicker.clear` | Clear | Limpar | Limpiar |
| `datepicker.selected` | selected | selecionado | seleccionado |
| `datepicker.unavailable` | unavailable | indisponivel | no disponible |
| `datepicker.prev_year` | Previous year | Ano anterior | Ano anterior |
| `datepicker.next_year` | Next year | Proximo ano | Proximo ano |
| `datepicker.prev_decade` | Previous decade | Decada anterior | Decada anterior |
| `datepicker.next_decade` | Next decade | Proxima decada | Proxima decada |
| `datepicker.week_format` | Week {0} | Semana {0} | Semana {0} |

### PdfViewer (`pdfviewer.*`)

| Key | English | Portuguese | Spanish |
|-----|---------|------------|---------|
| `pdfviewer.document` | PDF Document | Documento PDF | Documento PDF |
| `pdfviewer.title` | PDF Viewer | Visualizador de PDF | Visor de PDF |
| `pdfviewer.placeholder` | Enter a URL or base64 to view the PDF. | Informe uma URL ou base64 para visualizar o PDF. | Ingrese una URL o base64 para ver el PDF. |
| `pdfviewer.prev` | Previous | Anterior | Anterior |
| `pdfviewer.next` | Next | Proxima | Siguiente |
| `pdfviewer.zoom_in` | Zoom in | Aumentar zoom | Acercar |
| `pdfviewer.zoom_out` | Zoom out | Reduzir zoom | Alejar |
| `pdfviewer.page_format` | Page {0} | Pagina {0} | Pagina {0} |
| `pdfviewer.download` | Download | Baixar | Descargar |
| `pdfviewer.print` | Print | Imprimir | Imprimir |
| `pdfviewer.fullscreen` | Fullscreen | Tela cheia | Pantalla completa |
| `pdfviewer.loading` | Loading PDF... | Carregando PDF... | Cargando PDF... |

## Adding a New Locale

1. Create a new JSON file at `Localization/Json/Locales/{culture}.json`
2. Copy all keys from `en.json`
3. Translate the values
4. The locale provider will automatically load the file based on the current culture

## Format Strings

Keys ending in `_format` accept placeholder arguments:

```csharp
// Usage
builder.AddContent(0, string.Format(L("pagination.range_format", "{0}-{1} of {2}"), start, end, total));

// English: "1-10 of 100"
// Portuguese: "1-10 de 100"
// Spanish: "1-10 de 100"
```

## Best Practices

1. **Always provide a fallback** - The `L()` method returns the fallback if the key is not found
2. **Use descriptive keys** - Follow the `component.element` pattern (e.g., `datepicker.today`)
3. **Keep format strings consistent** - Use `{0}`, `{1}`, etc. for placeholders
4. **Test with TestLocalizer** - Returns the key itself, making assertions easy
5. **Don't hardcode strings** - Always use `L()` for user-facing text
