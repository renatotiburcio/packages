using System.Net.Http;
using System.Text.Json;
namespace XForge.FigmaSync;
/// <summary>T142: XFFigmaNativeSync -- full bidirectional Figma Variables. Syncfusion. P3. 16h. Dep T103+T122.</summary>
public class FigmaSyncService
{
    private readonly HttpClient _http;
    public FigmaSyncService(HttpClient http) { _http = http; }
    public async Task<string> PullAsync(string fileKey) { var r = await _http.GetAsync($"https://api.figma.com/v1/files/{fileKey}/variables/local"); return await r.Content.ReadAsStringAsync(); }
    public async Task<string> PushAsync(string fileKey, object variables) { var r = await _http.PostAsync($"https://api.figma.com/v1/files/{fileKey}/variables", new StringContent(JsonSerializer.Serialize(variables))); return await r.Content.ReadAsStringAsync(); }
}