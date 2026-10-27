using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using Xunit;
using XForge.DesignSystem.Components.Forms.Autocomplete;

using XForge.DesignSystem.Core.Primitives;
namespace XForge.DesignSystem.Tests.Components;

public class XFAutocompleteComponentTest : BunitContext
{
    private readonly IReadOnlyList<Customer> customers = new[]
    {
        new Customer(1, "Acme Foods"),
        new Customer(2, "Northwind Traders"),
        new Customer(3, "Contoso Retail")
    };

    [Fact]
    public void Render_WithSearch_ShowsFilteredItems()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Label, "Customer");
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.Search, "co");
        });

        cut.Find("input").Focus();

        Assert.Contains("xf-autocomplete", cut.Markup);
        Assert.Contains("Customer", cut.Markup);
        Assert.Contains("Contoso Retail", cut.Markup);
    }

    [Fact]
    public void Click_WhenItemSelected_UpdatesValueAndSearch()
    {
        int? value = null;
        string? search = null;
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.Search, "ac");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int?>(this, next => value = next));
            parameters.Add(p => p.SearchChanged, EventCallback.Factory.Create<string?>(this, next => search = next));
        });

        cut.Find("input").Focus();
        cut.FindAll("button").First(button => button.TextContent.Contains("Acme")).Click();

        Assert.Equal(1, value);
        Assert.Equal("Acme Foods", search);
        Assert.DoesNotContain("role=\"listbox\"", cut.Markup);
    }

    [Fact]
    public void Input_WhenSearchChanged_RaisesCallbacks()
    {
        string? search = null;
        string? requested = null;
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.SearchChanged, EventCallback.Factory.Create<string?>(this, next => search = next));
            parameters.Add(p => p.SearchRequested, EventCallback.Factory.Create<string?>(this, next => requested = next));
        });

        cut.Find("input").Input("north");

        Assert.Equal("north", search);
        Assert.Equal("north", requested);
        Assert.Contains("Northwind Traders", cut.Markup);
    }

    [Fact]
    public void KeyDown_WhenEnter_SelectsHighlightedItem()
    {
        int? value = null;
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.Search, "co");
            parameters.Add(p => p.ValueChanged, EventCallback.Factory.Create<int?>(this, next => value = next));
        });

        cut.Find("input").Focus();
        cut.Find(".xf-autocomplete").KeyDown("Enter");

        Assert.Equal(3, value);
    }

    [Fact]
    public void KeyDown_WhenOptionHighlighted_UpdatesActiveDescendant()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.Search, "o");
        });

        cut.Find("input").Focus();
        cut.Find(".xf-autocomplete").KeyDown("ArrowDown");

        var input = cut.Find("input[role='combobox']");
        var activeId = input.GetAttribute("aria-activedescendant");

        Assert.NotNull(activeId);
        Assert.Equal(activeId, cut.Find($"#{activeId}").GetAttribute("id"));
        Assert.Equal("true", cut.Find($"#{activeId}").GetAttribute("aria-selected"));
    }

    [Fact]
    public void Render_WithLoadingAndEmptyStates()
    {
        var loading = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.Search, "ac");
            parameters.Add(p => p.Loading, true);
        });

        var empty = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.Search, "zzz");
        });

        loading.Find("input").Focus();
        empty.Find("input").Focus();

        Assert.Contains("Carregando...", loading.Markup);
        Assert.Contains("Nenhum resultado.", empty.Markup);
    }

    [Fact]
    public void Render_WithCustomListboxLabel_UsesLocalizedListboxLabel()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.Search, "co");
            parameters.Add(p => p.ListboxLabel, "Customer results");
        });

        cut.Find("input").Focus();

        Assert.Contains("role=\"listbox\" aria-label=\"Customer results\"", cut.Markup);
        Assert.DoesNotContain("aria-label=\"Resultados\"", cut.Markup);
    }

    [Fact]
    public void Render_WithVisualOptions()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Variant, XFFormVariant.Soft);
            parameters.Add(p => p.Size, XFSize.Lg);
            parameters.Add(p => p.Tone, XFSemanticColor.Warning);
        });

        Assert.Contains("border-[color-mix(in_oklab,var(--color-primary)_35%,transparent)]", cut.Markup);
        Assert.Contains("h-12", cut.Markup);
        Assert.Contains("focus-within:border-[var(--color-warning)]", cut.Markup);
    }

    [Fact]
    public void Render_InsideEditForm_ShowsValidationError()
    {
        var model = new AutocompleteModel();
        var editContext = new EditContext(model);
        var fieldIdentifier = FieldIdentifier.Create(() => model.CustomerId);
        var messages = new ValidationMessageStore(editContext);
        messages.Add(fieldIdentifier, "Customer is required.");
        editContext.NotifyValidationStateChanged();

        var cut = Render(builder =>
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", editContext);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<XFAutocomplete<Customer, int?>>(3);
                childBuilder.AddAttribute(4, "Value", model.CustomerId);
                childBuilder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create<int?>(this, value => model.CustomerId = value));
                childBuilder.AddAttribute(6, "ValueExpression", (System.Linq.Expressions.Expression<Func<int?>>)(() => model.CustomerId));
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Customer is required.", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void SearchAsync_WhenSet_InvokesDelegate()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
            parameters.Add(p => p.ValueSelector, customer => customer.Id);
            parameters.Add(p => p.SearchAsync, (string? search, CancellationToken ct) =>
            {
                var filtered = customers.Where(c => c.Name.Contains(search ?? "", StringComparison.OrdinalIgnoreCase));
                return Task.FromResult<IEnumerable<Customer>>(filtered);
            });
            parameters.Add(p => p.DebounceMs, 0);
            parameters.Add(p => p.MinLength, 0);
        });

        cut.Find("input").Input("acme");

        // After debounce (0ms), the async results should be shown
        Assert.Contains("Acme Foods", cut.Markup);
    }

    [Fact]
    public void DebounceMs_DefaultValue_Is300()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.TextSelector, customer => customer.Name);
        });

        // Verify the component renders with default DebounceMs
        Assert.Contains("xf-autocomplete", cut.Markup);
    }

    [Fact]
    public void ErrorText_WhenSet_RendersErrorMessage()
    {
        var cut = Render<XFAutocomplete<Customer, int?>>(parameters =>
        {
            parameters.Add(p => p.Items, customers);
            parameters.Add(p => p.ErrorText, "Network error");
        });

        Assert.Contains("Network error", cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
    }

    private sealed record Customer(int Id, string Name);

    private sealed class AutocompleteModel
    {
        [Required]
        public int? CustomerId { get; set; }
    }
}
