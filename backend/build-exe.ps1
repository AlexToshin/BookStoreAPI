$outputDir = "..\lab-15-AlexToshin"
$projectPath = ".\DatabaseTask3\BookStore.API.csproj"

try {
    # Create output directory if not exists
    if (-not (Test-Path $outputDir)) {
        New-Item -Path $outputDir -ItemType Directory -Force -ErrorAction Stop
    }

    # Create bat-file with absolute paths
    $batchContent = @"
@echo off
echo Starting BookStore application...
cd /d "%~dp0backend"
dotnet run --project "$projectPath" --urls=http://localhost:5000
pause
"@

    $batPath = Join-Path $outputDir "RunBookStore.bat"
    Set-Content -Path $batPath -Value $batchContent -Encoding ASCII -ErrorAction Stop

    Write-Host "Launcher created successfully in parent folder!" -ForegroundColor Green
    Write-Host "To run: $batPath"
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}