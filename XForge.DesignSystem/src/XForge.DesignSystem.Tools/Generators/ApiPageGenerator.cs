using System.Collections.Generic;
using System.Text;
namespace XForge.DesignSystem.Tools.Generators;
public record ApiPageRequest(string EntityName, string Namespace, IEnumerable<string> Properties, string Title);
public class ApiPageGenerator
{
    public string GenerateRazor(ApiPageRequest req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"@page \\"/{req.EntityName.ToLowerInvariant()}\\"");
        sb.AppendLine($"@namespace {req.Namespace}");
        sb.AppendLine();
        sb.AppendLine($"<h3>{req.Title}</h3>");
        sb.AppendLine();
        sb.AppendLine("<XFForm Model=\"Model\" OnValidSubmit=\"HandleValidSubmit\">");
        foreach (var p in req.Properties) sb.AppendLine($"    <XFFormField Label=\"{p}\" @bind-Value=\"Model.{p}\" />");
        sb.AppendLine("    <button type=\"submit\">Save</button>");
        sb.AppendLine("</XFForm>");
        return sb.ToString();
    }
    public string GenerateCodeBehind(ApiPageRequest req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"using {req.Namespace};");
        sb.AppendLine("using Microsoft.AspNetCore.Components;");
        sb.AppendLine();
        sb.AppendLine($"namespace {req.Namespace}.Pages;");
        sb.AppendLine($"public partial class {req.EntityName}Page : ComponentBase");
        sb.AppendLine("{");
        sb.AppendLine($"    [SupplyParameterFromForm] public {req.EntityName} Model {{ get; set; }} = new();");
        sb.AppendLine("    private void HandleValidSubmit() { }");
        sb.AppendLine("}");
        return sb.ToString();
    }
    public int EstimateLines(ApiPageRequest req) => 6 + (req.Properties as IReadOnlyCollection<string>)?.Count * 2 ?? 6;
}