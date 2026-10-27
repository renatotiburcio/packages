file_path = r'C:\Temp\teste\componentesnew\XForge.DesignSystem\Components\DataDisplay\DataGrid\XFDataGrid.razor.cs'

def append(text):
    with open(file_path, 'a', encoding='utf-8') as f:
        f.write(text)

# We need to insert RenderFilterInput before the VariantClass method.
# Let's read and rebuild with the method inserted at the right place.
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

insert_marker = '        private string VariantClass => Variant switch'

filter_helper = '''        private RenderFragment<XFDataGridColumn<TItem>> RenderFilterInput => column =>
        {
            var filterType = GetColumnFilterType(column);
            var currentValue = GetColumnFilterValue(column.ResolvedKey);
            var ariaLabel = "Filtrar " + column.ResolvedTitle;

            return builder =>
            {
                if (filterType == XFDataGridFilterType.Boolean)
                {
                    builder.OpenElement(0, "select");
                    builder.AddAttribute(1, "class", FilterInputClass);
                    builder.AddAttribute(2, "aria-label", ariaLabel);
                    builder.AddAttribute(3, "value", currentValue ?? "");
                    builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.OpenElement(5, "option");
                    builder.AddAttribute(6, "value", "");
                    builder.AddContent(7, "Todos");
                    builder.CloseElement();
                    builder.OpenElement(8, "option");
                    builder.AddAttribute(9, "value", "true");
                    builder.AddContent(10, "Sim");
                    builder.CloseElement();
                    builder.OpenElement(11, "option");
                    builder.AddAttribute(12, "value", "false");
                    builder.AddContent(13, "Nao");
                    builder.CloseElement();
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Enum)
                {
                    builder.OpenElement(0, "select");
                    builder.AddAttribute(1, "class", FilterInputClass);
                    builder.AddAttribute(2, "aria-label", ariaLabel);
                    builder.AddAttribute(3, "value", currentValue ?? "");
                    builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.OpenElement(5, "option");
                    builder.AddAttribute(6, "value", "");
                    builder.AddContent(7, "Todos");
                    builder.CloseElement();
                    var enumType = ResolvePropertyType(column.Property ?? "");
                    if (enumType != null && enumType.IsEnum)
                    {
                        foreach (var name in Enum.GetNames(enumType))
                        {
                            builder.OpenElement(8, "option");
                            builder.AddAttribute(9, "value", name);
                            builder.AddContent(10, name);
                            builder.CloseElement();
                        }
                    }
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Date)
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "date");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", "Filtrar...");
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterSubmittedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                    {
                        if (e.Key == "Enter")
                        {
                            _ = OnColumnFilterSubmittedAsync(column, GetColumnFilterValue(column.ResolvedKey));
                        }
                    }));
                    builder.CloseElement();
                }
                else if (filterType == XFDataGridFilterType.Number)
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "number");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", "Filtrar...");
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterSubmittedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                    {
                        if (e.Key == "Enter")
                        {
                            _ = OnColumnFilterSubmittedAsync(column, GetColumnFilterValue(column.ResolvedKey));
                        }
                    }));
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(0, "input");
                    builder.AddAttribute(1, "type", "text");
                    builder.AddAttribute(2, "class", FilterInputClass);
                    builder.AddAttribute(3, "aria-label", ariaLabel);
                    builder.AddAttribute(4, "placeholder", "Filtrar...");
                    builder.AddAttribute(5, "value", currentValue ?? "");
                    builder.AddAttribute(6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterValueChangedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(7, "onblur", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        var val = e.Value?.ToString();
                        _ = OnColumnFilterSubmittedAsync(column, string.IsNullOrEmpty(val) ? null : val);
                    }));
                    builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                    {
                        if (e.Key == "Enter")
                        {
                            _ = OnColumnFilterSubmittedAsync(column, GetColumnFilterValue(column.ResolvedKey));
                        }
                    }));
                    builder.CloseElement();
                }
            };
        };

'''

if insert_marker in content:
    content = content.replace(insert_marker, filter_helper + insert_marker)
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
    print('Inserted RenderFilterInput method')
else:
    print('ERROR: Could not find insertion point')
