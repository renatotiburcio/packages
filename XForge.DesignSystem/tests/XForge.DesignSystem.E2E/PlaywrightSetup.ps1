# Playwright Setup Script for XForge Design System E2E Tests
# Run this script to install Playwright browsers

Write-Host "Installing Playwright browsers..." -ForegroundColor Green

# Build the test project first
dotnet build tests/XForge.DesignSystem.E2E/XForge.DesignSystem.E2E.csproj

# Install Playwright browsers
$playwrightPath = "tests/XForge.DesignSystem.E2E/bin/Debug/net10.0/playwright.ps1"
if (Test-Path $playwrightPath) {
    & $playwrightPath install
    Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
} else {
    Write-Host "Playwright script not found. Building project first..." -ForegroundColor Yellow
    dotnet build tests/XForge.DesignSystem.E2E/XForge.DesignSystem.E2E.csproj
    & "tests/XForge.DesignSystem.E2E/bin/Debug/net10.0/playwright.ps1" install
}

Write-Host "Setup complete!" -ForegroundColor Green
