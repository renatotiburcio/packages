using XForge.DesignSystem.Services;
using Xunit;
namespace XForge.DesignSystem.Tests.Services;
public class HxMessageBoxServiceTests
{
    [Fact] public async Task ShowAsync_Information_Confirms_By_Default() { var svc = new HxMessageBoxService(); var r = await svc.ShowAsync(new XMessageBoxRequest("t","m",XMessageBoxType.Information)); Assert.True(r.Confirmed); Assert.Equal(1, svc.PendingCount); }
    [Fact] public async Task ShowAsync_Confirmation_Not_Confirmed_By_Default() { var svc = new HxMessageBoxService(); var r = await svc.ShowAsync(new XMessageBoxRequest("t","m",XMessageBoxType.Confirmation)); Assert.False(r.Confirmed); }
    [Fact] public void Enqueue_Increments_PendingCount() { var svc = new HxMessageBoxService(); svc.Enqueue(new XMessageBoxRequest("t","m",XMessageBoxType.Warning)); Assert.Equal(1, svc.PendingCount); }
    [Fact] public void TryDequeue_Returns_True_When_NonEmpty() { var svc = new HxMessageBoxService(); svc.Enqueue(new XMessageBoxRequest("t","m",XMessageBoxType.Error)); Assert.True(svc.TryDequeue(out _)); Assert.Equal(0, svc.PendingCount); }
}