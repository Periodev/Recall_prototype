# tools/setup_solution.ps1
param (
    [string]$SolutionName = "Recall"
)

Write-Host "=== Setting up solution: $SolutionName ===" -ForegroundColor Cyan

# Step 1: 移到專案根目錄 (腳本在 tools 資料夾)
$RootPath = Split-Path -Parent $PSScriptRoot
Set-Location $RootPath

# Step 2: 如果有舊的 sln 先刪掉
if (Test-Path "$SolutionName.sln") {
    Write-Host "Removing old $SolutionName.sln..."
    Remove-Item "$SolutionName.sln" -Force
}

# Step 3: 建立新的解決方案
Write-Host "Creating new solution..."
dotnet new sln -n $SolutionName

# Step 4: 搜尋所有 csproj 並加入解決方案
Write-Host "Adding projects to solution..."
$projects = Get-ChildItem -Recurse -Filter *.csproj
foreach ($proj in $projects) {
    Write-Host " -> Adding $($proj.FullName)"
    dotnet sln add $proj.FullName
}

# Step 5: 還原依賴
Write-Host "Restoring packages..."
dotnet restore

# Step 6: 執行測試
Write-Host "Running tests..."
dotnet test

Write-Host "=== Setup complete! ===" -ForegroundColor Green
