// T050: Example DataGrid extension (stub).
namespace XForge.DesignSystem.Extensions;

/// <summary>Stub DataGrid extension used for testing the registry.</summary>
public sealed class XFDataGridExtension : IXFExtension
{
    public string Id => "DataGrid";
    public string Name => "XForge DataGrid";
    public bool IsEnabled { get; private set; }
    public void Activate(IServiceProvider services) => IsEnabled = true;
    public void Deactivate() => IsEnabled = false;
}