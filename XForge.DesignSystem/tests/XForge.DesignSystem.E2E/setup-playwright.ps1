# XForge Design System - Playwright Setup Script
# Run this script to install Playwright browsers

Write-Host "=== XForge Design System - Playwright Setup ===" -ForegroundColor Cyan

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: dotnet not found. Please install .NET SDK." -ForegroundColor Red
    exit 1
}

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

# Install Playwright browsers
Write-Host "Installing Playwright browsers..." -ForegroundColor Yellow
$playwrightPath = "bin/Debug/net10.0/playwright.ps1"
if (Test-Path $playwrightPath) {
    & $playwrightPath install --with-deps
    Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
} else {
    Write-Host "Playwright script not found at $playwrightPath" -ForegroundColor Red
    Write-Host "Please build the project first: dotnet build" -ForegroundColor Yellow
}

# Run a quick test to verify setup
Write-Host "`nVerifying Playwright setup..." -ForegroundColor Yellow
dotnet test --filter "BasicE2ETests.HomePage_LoadsSuccessfully" --verbosity minimal

Write-Host "`n=== Setup Complete ===" -ForegroundColor Green
