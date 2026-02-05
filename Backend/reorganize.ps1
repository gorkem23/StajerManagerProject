# Backend ve Frontend Klasörlerini Ayırma Script'i
# Bu script, StajerManager ve StajerManagerVite klasörlerini backend ve frontend olarak ayırır

Write-Host "=== Backend ve Frontend Ayırma İşlemi ===" -ForegroundColor Cyan
Write-Host ""

# Mevcut dizine git
$reposPath = "C:\Users\lercan\source\repos"
$newProjectPath = Join-Path $reposPath "StajerManagerProject"
$backendPath = Join-Path $reposPath "StajerManager"
$frontendPath = Join-Path $reposPath "StajerManagerVite"

# Kontrol: Klasörler mevcut mu?
if (-not (Test-Path $backendPath)) {
    Write-Host "HATA: StajerManager klasörü bulunamadı: $backendPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $frontendPath)) {
    Write-Host "HATA: StajerManagerVite klasörü bulunamadı: $frontendPath" -ForegroundColor Red
    exit 1
}

# Yeni klasör yapısını oluştur
Write-Host "1. Yeni klasör yapısı oluşturuluyor..." -ForegroundColor Yellow
$newBackendPath = Join-Path $newProjectPath "backend"
$newFrontendPath = Join-Path $newProjectPath "frontend"

New-Item -ItemType Directory -Path $newProjectPath -Force | Out-Null
New-Item -ItemType Directory -Path $newBackendPath -Force | Out-Null
New-Item -ItemType Directory -Path $newFrontendPath -Force | Out-Null

Write-Host "   ✓ Klasörler oluşturuldu" -ForegroundColor Green

# Backend'i taşı
Write-Host "2. Backend dosyaları taşınıyor..." -ForegroundColor Yellow
try {
    Get-ChildItem -Path $backendPath -Force | ForEach-Object {
        $destPath = Join-Path $newBackendPath $_.Name
        Move-Item -Path $_.FullName -Destination $destPath -Force
    }
    Write-Host "   ✓ Backend taşındı" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Backend taşıma hatası: $_" -ForegroundColor Red
    exit 1
}

# Frontend'i taşı
Write-Host "3. Frontend dosyaları taşınıyor..." -ForegroundColor Yellow
try {
    Get-ChildItem -Path $frontendPath -Force | ForEach-Object {
        $destPath = Join-Path $newFrontendPath $_.Name
        Move-Item -Path $_.FullName -Destination $destPath -Force
    }
    Write-Host "   ✓ Frontend taşındı" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Frontend taşıma hatası: $_" -ForegroundColor Red
    exit 1
}

# Eski boş klasörleri sil
Write-Host "4. Eski klasörler temizleniyor..." -ForegroundColor Yellow
try {
    if (Test-Path $backendPath) {
        Remove-Item -Path $backendPath -Recurse -Force -ErrorAction SilentlyContinue
    }
    if (Test-Path $frontendPath) {
        Remove-Item -Path $frontendPath -Recurse -Force -ErrorAction SilentlyContinue
    }
    Write-Host "   ✓ Eski klasörler temizlendi" -ForegroundColor Green
} catch {
    Write-Host "   ⚠ Bazı klasörler silinemedi (manuel kontrol edin)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== İşlem Tamamlandı! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Yeni yapı:" -ForegroundColor Cyan
Write-Host "  Backend:  $newBackendPath" -ForegroundColor White
Write-Host "  Frontend: $newFrontendPath" -ForegroundColor White
Write-Host ""
Write-Host "Sonraki adımlar:" -ForegroundColor Cyan
Write-Host "  1. Cursor/VS Code'da workspace'i yeniden açın" -ForegroundColor White
Write-Host "  2. Backend: cd '$newBackendPath\StajerManager' && dotnet run" -ForegroundColor White
Write-Host "  3. Frontend: cd '$newFrontendPath' && npm run dev" -ForegroundColor White
Write-Host ""

