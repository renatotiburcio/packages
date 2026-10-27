using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace XForge.DesignSystem.Services;
public enum XMessageBoxType { Information, Warning, Error, Success, Confirmation }
public record XMessageBoxRequest(string Title, string Message, XMessageBoxType Type);
public record XMessageBoxResult(bool Confirmed);
public interface IHxMessageBoxService { Task<XMessageBoxResult> ShowAsync(XMessageBoxRequest request); }
public class HxMessageBoxService : IHxMessageBoxService
{
    private readonly ConcurrentQueue<XMessageBoxRequest> _queue = new();
    public int PendingCount => _queue.Count;
    public void Enqueue(XMessageBoxRequest r) => _queue.Enqueue(r);
    public bool TryDequeue(out XMessageBoxRequest? r) { r = null; return _queue.TryDequeue(out var x) && (r = x) != null; }
    public Task<XMessageBoxResult> ShowAsync(XMessageBoxRequest request)
    {
        Enqueue(request);
        return Task.FromResult(new XMessageBoxResult(Confirmed: request.Type != XMessageBoxType.Confirmation));
    }
}
