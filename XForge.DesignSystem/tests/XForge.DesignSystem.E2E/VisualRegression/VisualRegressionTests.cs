using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XForge.DesignSystem.E2E.VisualRegression;

[TestClass]
public class VisualRegressionTests : PageTest
{
    private const string ScreenshotsDir = "Screenshots";

    [TestInitialize]
    public void Setup()
    {
        Directory.CreateDirectory(ScreenshotsDir);
    }

    [TestMethod]
    public async Task XFButton_VisualSnapshot()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div style='padding: 20px; font-family: system-ui;'>
                <h2>Button Component</h2>
                <div style='display: flex; gap: 8px; margin-top: 10px;'>
                    <button type='button' style='padding: 8px 16px; border: 1px solid #ccc; border-radius: 4px; background: white;'>Default</button>
                    <button type='button' style='padding: 8px 16px; border: none; border-radius: 4px; background: #3b82f6; color: white;'>Primary</button>
                    <button type='button' style='padding: 8px 16px; border: 1px solid #ccc; border-radius: 4px; background: white;' disabled>Disabled</button>
                </div>
            </div>
        ");
        
        await Page.ScreenshotAsync(new() { Path = Path.Combine(ScreenshotsDir, "button-variants.png") });
    }

    [TestMethod]
    public async Task XFScheduler_VisualSnapshot()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div style='padding: 20px; font-family: system-ui; border: 1px solid #e5e7eb; border-radius: 8px;'>
                <div style='display: flex; justify-content: space-between; margin-bottom: 16px;'>
                    <h2 style='margin: 0;'>Scheduler</h2>
                    <div style='display: flex; gap: 4px;'>
                        <button style='padding: 4px 8px; background: #3b82f6; color: white; border: none; border-radius: 4px;'>Week</button>
                        <button style='padding: 4px 8px; background: white; border: 1px solid #ccc; border-radius: 4px;'>Month</button>
                    </div>
                </div>
                <div style='display: grid; grid-template-columns: 60px repeat(7, 1fr); border: 1px solid #e5e7eb;'>
                    <div style='border-right: 1px solid #e5e7eb;'></div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 1</div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 2</div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 3</div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 4</div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 5</div>
                    <div style='border-right: 1px solid #e5e7eb; padding: 8px; text-align: center; font-size: 12px;'>Day 6</div>
                    <div style='padding: 8px; text-align: center; font-size: 12px;'>Day 7</div>
                </div>
            </div>
        ");
        
        await Page.ScreenshotAsync(new() { Path = Path.Combine(ScreenshotsDir, "scheduler-week-view.png") });
    }

    [TestMethod]
    public async Task XFGantt_VisualSnapshot()
    {
        await Page.GotoAsync("about:blank");
        await Page.SetContentAsync(@"
            <div style='padding: 20px; font-family: system-ui; border: 1px solid #e5e7eb; border-radius: 8px;'>
                <h2 style='margin: 0 0 16px 0;'>Gantt Chart</h2>
                <svg viewBox='0 0 800 200' style='width: 100%; border: 1px solid #e5e7eb; border-radius: 4px;'>
                    <rect x='50' y='20' width='150' height='24' rx='4' fill='#3b82f6' />
                    <text x='55' y='36' fill='white' font-size='12'>Task 1</text>
                    <rect x='120' y='60' width='200' height='24' rx='4' fill='#10b981' />
                    <text x='125' y='76' fill='white' font-size='12'>Task 2</text>
                    <rect x='200' y='100' width='180' height='24' rx='4' fill='#f59e0b' />
                    <text x='205' y='116' fill='white' font-size='12'>Task 3</text>
                    <line x1='200' y1='32' x2='120' y2='60' stroke='#9ca3af' stroke-width='1.5' marker-end='url(#arrowhead)' />
                    <line x1='320' y1='72' x2='200' y2='100' stroke='#9ca3af' stroke-width='1.5' marker-end='url(#arrowhead)' />
                    <defs>
                        <marker id='arrowhead' markerWidth='10' markerHeight='7' refX='9' refY='3.5' orient='auto'>
                            <polygon points='0 0, 10 3.5, 0 7' fill='#9ca3af' />
                        </marker>
                    </defs>
                </svg>
            </div>
        ");
        
        await Page.ScreenshotAsync(new() { Path = Path.Combine(ScreenshotsDir, "gantt-chart.png") });
    }
}
