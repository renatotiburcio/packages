using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.DropDownDataGrid;

namespace XForge.DesignSystem.Tests.Components;

public class XFDropDownDataGridComponentTest : XForgeTestContext
{
    private readonly IReadOnlyList<Product> products = new[]
    {
        new Product(1, "Laptop", "Electronics"),
        new Product(2, "Desk Chair", "Furniture"),
        new Product(3, "Monitor", "Electronics")
    };

    [Fact]
    public void Render_Default_ShowsInputAndToggle()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Label, "Product");
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
        });

        Assert.Contains("xf-dropdown-datagrid", cut.Markup);
        Assert.Contains("Product", cut.Markup);
        Assert.Contains("role=\"combobox\"", cut.Markup);
    }

    [Fact]
    public void Click_Toggle_OpensDropdown()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
        });

        cut.Find("button[aria-label='Abrir opções']").Click();

        Assert.Contains("role=\"listbox\"", cut.Markup);
        Assert.Contains("Laptop", cut.Markup);
        Assert.Contains("Desk Chair", cut.Markup);
    }

    [Fact]
    public void Click_Row_SelectsItem()
    {
        int? value = null;
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int?>(this, v => value = v));
        });

        cut.Find("button[aria-label='Abrir opções']").Click();
        cut.FindAll("tr[role='option']")[1].Click();

        Assert.Equal(2, value);
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    [Fact]
    public void Search_FiltersRows()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
        });

        cut.Find("button[aria-label='Abrir opções']").Click();
        cut.Find("input[placeholder='Buscar...']").Input("Laptop");

        Assert.Contains("Laptop", cut.Markup);
        Assert.DoesNotContain("Desk Chair", cut.Markup);
    }

    [Fact]
    public void Render_WithColumns_ShowsColumnHeaders()
    {
        var columns = new List<XFDropDownDataGridColumn<Product>>
        {
            new() { Title = "Name", Template = p => (RenderFragment)(b => b.AddContent(0, p.Name)) },
            new() { Title = "Category", Template = p => (RenderFragment)(b => b.AddContent(0, p.Category)) }
        };

        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
            parameters.Add(p => p.Columns, columns);
        });

        cut.Find("button[aria-label='Abrir opções']").Click();

        Assert.Contains("Name", cut.Markup);
        Assert.Contains("Category", cut.Markup);
        Assert.Contains("Electronics", cut.Markup);
    }

    [Fact]
    public void Render_EmptyItems_ShowsEmptyText()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, Array.Empty<Product>());
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.EmptyText, "No products found");
        });

        cut.Find("button[aria-label='Abrir opções']").Click();

        Assert.Contains("No products found", cut.Markup);
    }

    [Fact]
    public void Clear_WhenSelected_ClearsValue()
    {
        int? value = 1;
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
            parameters.Add(p => p.Value, value);
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int?>(this, v => value = v));
        });

        cut.Find("button[aria-label='Limpar']").Click();

        Assert.Null(value);
    }

    [Fact]
    public void Render_Disabled_DisablesToggle()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.Disabled, true);
        });

        Assert.Contains("disabled", cut.Find("button[aria-label='Abrir opções']").OuterHtml);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new ProductModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.ProductId);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Product is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFDropDownDataGrid<Product, int?>>(3);
                childBuilder.AddAttribute(4, "Value", model.ProductId);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<int?>(this, v => model.ProductId = v));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<int?>>)(() => model.ProductId));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Product is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void Keyboard_Escape_ClosesDropdown()
    {
        var cut = Render<XFDropDownDataGrid<Product, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, products);
            parameters.Add(p => p.TextSelector, p => p.Name);
            parameters.Add(p => p.ValueSelector, p => p.Id);
        });

        cut.Find("button[aria-label='Abrir opções']").Click();
        Assert.Contains("role=\"listbox\"", cut.Markup);

        cut.Find(".xf-dropdown-datagrid").KeyDown("Escape");
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    private sealed record Product(int Id, string Name, string Category);

    private sealed class ProductModel
    {
        [Required]
        public int? ProductId { get; set; }
    }
}
